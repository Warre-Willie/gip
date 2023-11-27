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


# Make connection with database
db = mysql.connector.connect(
    host="localhost",
    user="root",
    passwd="gip-WJ",
    database="crowd_management"
    )
mycursor = db.cursor(dictionary=True) # Dictionary true for ease of processing respones

# MQTT settings
broker_address = "broker.hivemq.com"
port = 1883

# Callback when the client connects to the broker
# def on_connect(client, userdata, flags, rc):
#     print("")

# Callback when a message is received from the broker
def on_message(client, userdata, msg):
    response = json.loads(msg.payload.decode())
    print("\nRFID: " + response['RFID'])
    client.unsubscribe(str(myuuid))

# Create MQTT client instance with no client_id
client = mqtt.Client(client_id="", clean_session=True)

# Set last will message
will_topic = "disconnected"
will_message = "Connection lost unexpectedly"
client.will_set(will_topic, will_message, 2, False)

# Set callback functions
# client.on_connect = on_connect
client.on_message = on_message

# Connect to the broker
client.connect(broker_address, port, 60)

# Start the network loop
while True:
    client.loop_start()
    barcode = input("Enter barcode:")
    print("")
    myuuid = uuid.uuid4()
    msg = '{"UUID": "' + str(myuuid) + '", "returnData": true, "query": "SELECT * FROM tickets WHERE barcode=%' + barcode + '%"}'
    client.subscribe(str(myuuid))
    client.publish("queries", msg)