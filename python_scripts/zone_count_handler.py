import paho.mqtt.client as mqtt
import mysql.connector
import json

# Global varible
thresholds = {}

db_mqtt_settings = None
with open('db_mqtt_config.json', 'r') as f:
        db_mqtt_settings = json.load(f)    

# Make connection with database
db = mysql.connector.connect(
    host=db_mqtt_settings['db']['host'],
    user=db_mqtt_settings['db']['username'],
    passwd=db_mqtt_settings['db']['password'],
    database=db_mqtt_settings['db']['database']
    )
mycursor = db.cursor(dictionary=True) # Dictionary true for ease of processing respones

# MQTT settings
if db_mqtt_settings["isDevlopment"] == True:
    broker_address = db_mqtt_settings['mqtt']["mqttDev"]['broker']
    port = db_mqtt_settings['mqtt']["mqttDev"]['port']
else:
    broker_address = db_mqtt_settings['mqtt']["mqttProd"]['broker']
    port = db_mqtt_settings['mqtt']["mqttProd"]['port']

def database_update():
    mycursor = db.cursor(dictionary=True) # Dictionary true for ease of processing respones
    mycursor.execute("SELECT * FROM `zones`")

    # Set thresholds
    for row in mycursor:
        if row["people_count"] != None:
            threshold = {"green": row["threshold_green"], "orange": row["threshold_orange"]}
            thresholds[row["id"]] = (threshold)


def count_request(msg):
    counter = 0
    response = json.loads(msg.payload.decode())

    mycursor = db.cursor(dictionary=True) # Dictionary true for ease of processing respones
    mycursor.execute(f"SELECT `people_count`, `lockdown`, `barometer_color` FROM `zones` WHERE `id` = '{response['id']}'")
    for row in mycursor:
        if row["people_count"] != None:
            counter = int(row["people_count"])
            counter += response["people"]
            if counter < 0:
                counter = 0
                mycursor.execute(f"UPDATE `zones` SET `people_count`= '{str(counter)}' WHERE `id` = '{response['id']}'")
                db.commit()
                print(counter)
            else:
                mycursor.execute(f"UPDATE `zones` SET `people_count`= '{str(counter)}' WHERE `id` = '{response['id']}'")
                db.commit()
                print(counter)
            if row["lockdown"] == 0:
                new_color= ""
                if counter <= thresholds[response['id']]['green'] and str(row['barometer_color']) != "green":
                    new_color = "green"
                elif counter > thresholds[response['id']]['green'] and counter <= thresholds[response['id']]['orange'] and str(row['barometer_color']) != "orange":
                    new_color = "orange"
                elif counter >= thresholds[response['id']]['orange'] and str(row['barometer_color']) != "red":
                    new_color = "red"
                if new_color != "":
                    client.publish("/gip/teller/barometer", '{"id": ' + str(response['id']) + ', "color": "' + new_color + '"}')
                    mycursor.execute(f"UPDATE `zones` SET `barometer_color`= '{new_color}' WHERE `id` = '{response['id']}'")
                    db.commit()
            else:
                print("Barometer locked")
        else:
            print("No count zone")
    mycursor.close()

#new device request
def new_device(msg):
    response = json.loads(msg.payload.decode())
    mycursor = db.cursor(dictionary=True) # Dictionary true for ease of processing respones
    mycursor.execute("SELECT barometer_color FROM zones WHERE id = " + str(response["id"]) + ";")
    for row in mycursor:
        client.publish("/gip/teller/barometer", '{"id": ' + str(response["id"]) + ', "color": "'+ str(row["barometer_color"]) + '"}')
    mycursor.close()


# Callback when a message is received from the broker
# The incomming data will be in the format of json: {"id": 1,"people": -1}
def on_message(client, userdata, msg):
    match = msg.topic
    if match == "/gip/teller/counter":
        count_request(msg)
    elif match == "/gip/teller/db_update":
        database_update()
        print("Database updated")
    elif match == "/gip/teller/new_device":
        new_device(msg)
    else:
        print("No match")
        
      
# Create MQTT client instance with no client_id
client = mqtt.Client(client_id="", clean_session=True)


# Set callback functions
# client.on_connect = on_connect
client.on_message = on_message

#First database update to get the thresholds
database_update()

# Connect to the broker
client.connect(broker_address, port, 60)

client.subscribe("/gip/teller/counter")
client.subscribe("/gip/teller/db_update")
client.subscribe("/gip/teller/new_device")
# Start the network loop
while True:
    client.loop_start()
    
