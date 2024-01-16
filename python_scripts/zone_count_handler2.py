#  MQTT publish example
# {
#   "UUID": "34df97b8-9b47-4cdd-b11b-09b18e32dbd7",
#   "returnData": true,
#   "query": "SELECT * FROM tickets WHERE barcode=\"123456789\";"
# }

import paho.mqtt.client as mqtt
import mysql.connector
import json
import uuid

# Global varible
counter = 0
thresholds = {}



# Make connection with database
db = mysql.connector.connect(
    host="localhost",
    user="root",
    passwd="gip-WJ",
    database="crowd_management"
    )
mycursor = db.cursor(dictionary=True) # Dictionary true for ease of processing respones
mycursor.execute("SELECT * FROM `zones`")

# Set thresholds
for row in mycursor:
    if row["people_count"] != None:
        threshold = {"green": row["threshold_green"], "orange": row["threshold_orange"], "red": row["threshold_red"]}
        thresholds[row["id"]] = (threshold)

print(thresholds[1]['red'])
exit()


# MQTT settings
broker_address = "broker.hivemq.com"
port = 1883


# Callback when a message is received from the broker
def on_message(client, userdata, msg):
    global counter
    response = int(msg.payload.decode())
    counter += response
    print(counter)
    if counter < threshold_green:
        client.publish("jesse", "green")
    elif counter < threshold_orange:
        client.publish("jesse", "orange")
        print("test")
    else:
        client.publish("jesse", "red")
        
# Create MQTT client instance with no client_id
client = mqtt.Client(client_id="", clean_session=True)


# Set callback functions
# client.on_connect = on_connect
client.on_message = on_message

# Connect to the broker
client.connect(broker_address, port, 60)

client.subscribe("Count_update")
# Start the network loop
while True:
    client.loop_start()
    
    client.publish("Led_state","test")
