#  MQTT publish example
# {
#   "UUID": "34df97b8-9b47-4cdd-b11b-09b18e32dbd7",
#   "returnData": true,
#   "query": "SELECT * FROM tickets WHERE barcode=\"123456789\";"
# }

import paho.mqtt.client as mqtt
import mysql.connector
import json

# Global varible
thresholds = {}
current_color = ""

# Make connection with database
db = mysql.connector.connect(
    host="localhost",
    user="root",
    passwd="gip-WJ",
    database="crowd_management"
    )

def database_update():
    mycursor = db.cursor(dictionary=True) # Dictionary true for ease of processing respones
    mycursor.execute("SELECT * FROM `zones`")

    # Set thresholds
    for row in mycursor:
        if row["people_count"] != None:
            threshold = {"green": row["threshold_green"], "orange": row["threshold_orange"], "red": row["threshold_red"]}
            thresholds[row["id"]] = (threshold)


def count_request(msg):
    counter = 0
    response = json.loads(msg.payload.decode())

    mycursor = db.cursor(dictionary=True) # Dictionary true for ease of processing respones
    mycursor.execute(f"SELECT `people_count`, `barometer_lock`, `barometer_color` FROM `zones` WHERE `id` = '{response['id']}'")
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
            # global current_color
            print(row["barometer_color"])
            if row["barometer_lock"] == 0:
                if counter <= thresholds[response['id']]['green'] and str(row['barometer_color']) != "green":
                    client.publish("barometer", '{"id": ' + str(response['id']) + ', "color": "green"}')
                    mycursor.execute(f"UPDATE `zones` SET `barometer_color`= 'green' WHERE `id` = '{response['id']}'")
                    db.commit()
                    # current_color = "green"
                elif counter > thresholds[response['id']]['green'] and counter <= thresholds[response['id']]['orange'] and str(row['barometer_color']) != "orange":
                    client.publish("barometer", '{"id": ' + str(response['id']) + ', "color": "orange"}')
                    mycursor.execute(f"UPDATE `zones` SET `barometer_color`= 'orange' WHERE `id` = '{response['id']}'")
                    db.commit()
                    # current_color = "orange"
                elif counter >= thresholds[response['id']]['orange'] and str(row['barometer_color']) != "red":
                    client.publish("barometer", '{"id": ' + str(response['id']) + ', "color": "red"}')
                    mycursor.execute(f"UPDATE `zones` SET `barometer_color`= 'red' WHERE `id` = '{response['id']}'")
                    db.commit()
                    # current_color = "red"
                # print(current_color)
            else:
                print("Barometer locked")
        else:
            print("No count zone")
    mycursor.close()


# MQTT settings
broker_address = "broker.hivemq.com"
port = 1883


# Callback when a message is received from the broker
# The incomming data will be in the format of json: {"id": 1,"people": -1}
def on_message(client, userdata, msg):
    match = msg.topic
    if match == "Jesse":
        count_request(msg)
    elif match == "db_update":
        database_update()
        print("Database updated")
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

client.subscribe("Jesse")
client.subscribe("db_update")
# Start the network loop
while True:
    client.loop_start()
    
