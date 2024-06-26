"""
File: zone_count_handler.py
Author: Warre Willeme & Jesse UijtdeHaag
Date: 29-04-2024
Description: This script is used to make the connection btween the count divices and the database and to update the barometer.
"""
import paho.mqtt.client as mqtt
import mysql.connector
import json
import datetime

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
    username = db_mqtt_settings['mqtt']["mqttDev"]['username']
    password = db_mqtt_settings['mqtt']["mqttDev"]['password']
else:
    broker_address = db_mqtt_settings['mqtt']["mqttProd"]['broker']
    port = db_mqtt_settings['mqtt']["mqttProd"]['port']
    username = db_mqtt_settings['mqtt']["mqttProd"]['username']
    password = db_mqtt_settings['mqtt']["mqttProd"]['password']


def count_request(msg):
    counter = 0
    response = json.loads(msg.payload.decode())
    try:
        db.reconnect()
        mycursor = db.cursor(dictionary=True) # Dictionary true for ease of processing respones
        mycursor.execute(f"SELECT * FROM zones WHERE id = '{response['id']}'")
    except:
        print("Error selecting zone")
    for row in mycursor:
        if row["people_count"] != None:
            counter = int(row["people_count"])
            counter += response["people"]
            try:
                if counter < 0:
                    counter = 0
                    mycursor.execute(f"UPDATE zones SET people_count= '{str(counter)}' WHERE id = '{response['id']}'")
                    print(counter)
                else:
                    mycursor.execute(f"UPDATE zones SET people_count= '{str(counter)}' WHERE id = '{response['id']}'")
                    print(counter)
                db.commit()
            except:
                print("Error updating count")

            counter_prcentage = (counter / row["max_people"]) * 100
            if row["lockdown"] == 0:
                new_color= ""
                if counter_prcentage <= row["threshold_green"] and str(row['barometer_color']) != "green":
                    new_color = "green"
                elif counter_prcentage > row["threshold_green"] and counter_prcentage <= row["threshold_orange"] and str(row['barometer_color']) != "orange":
                    new_color = "orange"
                elif counter_prcentage > row["threshold_orange"] and str(row['barometer_color']) != "red":
                    new_color = "red"
                if new_color != "":
                    try:
                        client.publish("gip/teller/barometer", '{"id": ' + str(response['id']) + ', "color": "' + new_color + '"}')
                        mycursor.execute(f"UPDATE zones SET barometer_color = '{new_color}' WHERE id = '{response['id']}'")
                        db.commit()
                        mycursor.execute(f"INSERT INTO barometer_logbook (zone_id, color) VALUES ({response['id']}, '{new_color}')")
                        db.commit()
                        mycursor.execute(f"INSERT INTO website_logbook(category, user, description) VALUES ('Barometer', 'System', 'Zone: {response['id']} verandert naar {new_color}')")
                        db.commit()
                    except:
                        print("Error updating barometer")
            else:
                print("Barometer locked")
        else:
            print("No count zone")
    mycursor.close()

#new device request
def new_device(msg):
    try:
        response = json.loads(msg.payload.decode())
        db.reconnect()
        mycursor = db.cursor(dictionary=True) # Dictionary true for ease of processing respones
        mycursor.execute("SELECT barometer_color FROM zones WHERE id = " + str(response["id"]) + ";")
        for row in mycursor:
            client.publish("gip/teller/barometer", '{"id": ' + str(response["id"]) + ', "color": "'+ str(row["barometer_color"]) + '"}')
        mycursor.close()
    except:
        print("Error new device")

def insert_population():
    try:
        db.reconnect()
        mycursor = db.cursor(dictionary=True)
        mycursor.execute("SELECT id, people_count FROM zones WHERE people_count <> 0")
        for row in mycursor:
            mycursor.execute(f"INSERT INTO zone_population_data (zone_id, people_count) VALUES ({row['id']},{row['people_count']})")
            db.commit()
        mycursor.close()
    except:
        print("Error inserting population")
    
# Callback when a message is received from the broker
# The incomming data will be in the format of json: {"id": 1,"people": -1}
def on_message(client, userdata, msg):
    match = msg.topic
    if match == "gip/teller/counter":
        count_request(msg)
    elif match == "gip/teller/new_device":
        new_device(msg)
    else:
        print("No match")
        
      
# Create MQTT client instance with no client_id
client = mqtt.Client(client_id="", clean_session=True)

# Set callback functions
client.on_message = on_message

# Set last will message
client.will_set("gip/notification", '{ "isCounter": false, "message": "Verbinding met <b>Count handler server</b> verloren", "category": "Alert" }', 2, False)

# Connect to the broker
try:
    client.username_pw_set(username, password)
    client.connect(broker_address, port, 60)
    client.publish("gip/notification", '{ "isCounter": false, "message": "Verbinding met <b>Count handler server</b> gemaakt", "category": "Info" }')
except:
    print("MQTT connection failed")

client.subscribe("gip/teller/counter")
client.subscribe("gip/teller/new_device")

current_minute = datetime.datetime.now().minute
# Start the network loop
while True:
    client.loop_start()

    now = datetime.datetime.now()
    #insert population every 5 minutes
    if now.minute % 5 == 0 and now.minute != current_minute and now.second > 30:
        current_minute = now.minute
        insert_population()
        old_time = now + datetime.timedelta(minutes=5)
        print("Population inserted")
