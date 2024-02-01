import paho.mqtt.client as mqtt
import mysql.connector
import json

# MQTT settings
broker_address = "broker.hivemq.com"
port = 1883

def on_message(client, userdata, msg):
	response = json.loads(msg.payload.decode())
	print(response)
	
	
# Create MQTT client instance with no client_id
client = mqtt.Client(client_id="", clean_session=True)


# Set callback functions
# client.on_connect = on_connect
client.on_message = on_message

# Connect to the broker
client.connect(broker_address, port, 60)

client.subscribe("barometer")

# Start the network loop
while True:
    client.loop_start()