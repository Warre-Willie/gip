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
threshold_green = 2
threshold_orange = 3
threshold_red = 5


# Make connection with database
# db = mysql.connector.connect(
#     host="localhost",
#     user="root",
#     passwd="gip-WJ",
#     database="crowd_management"
#     )
# mycursor = db.cursor(dictionary=True) # Dictionary true for ease of processing respones

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
        client.publish("Led_state", "green")
    elif counter < threshold_orange:
        client.publish("Led_state", "orange")
    else:
        client.publish("Led_state", "red")
        
# Create MQTT client instance with no client_id
client = mqtt.Client(client_id="", clean_session=True)


# Set callback functions
# client.on_connect = on_connect
client.on_message = on_message

# Connect to the broker
client.connect(broker_address, port, 60)

# Start the network loop
while True:
    client.loop_start()
    client.subscribe("Count_update")
