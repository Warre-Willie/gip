#  MQTT publish example
# {
#   "UUID": "34df97b8-9b47-4cdd-b11b-09b18e32dbd7",
#   "returnData": true,
#   "query": "SELECT * FROM tickets WHERE barcode=\"123456789\";"
# }

import paho.mqtt.client as mqtt
import json
import uuid


# MQTT settings
broker_address = "broker.hivemq.com"
port = 1883

# Callback when the client connects to the broker
# def on_connect(client, userdata, flags, rc):
#     print("Connected to broker")


message_send = False

# Callback when a message is received from the broker
def on_message(client, userdata, msg):
    global message_send
    response = json.loads(msg.payload.decode())
    for row in response:
        print(row)
    
    # if len(response) == 0:
    #     print("No ticket found")
    # else:
    #     print("RFID: " + response['RFID'])
    
    client.unsubscribe(str(myuuid))
    message_send = False

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
    if not message_send:
        message_send = True
        barcode = input("Enter barcode:") 
        myuuid = uuid.uuid4()
        msg = '{"UUID": "' + str(myuuid) + '", "returnData": true, "query": "SELECT * FROM tickets"}'
        client.subscribe(str(myuuid))
        client.publish("gip/queries", msg)