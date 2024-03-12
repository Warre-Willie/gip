import paho.mqtt.client as mqtt

# Define callback functions
def on_connect(client, userdata, flags, rc):
    print(f"Connected with result code {rc}")
    # Subscribe to the topic when connected
    # client.subscribe("Jesse")
    client.subscribe("/teller")

def on_message(client, userdata, msg):
    print(f"Received message on topic {msg.topic}: {msg.payload.decode()}")

# Create an MQTT client instance
client = mqtt.Client()

# Set callback functions
client.on_connect = on_connect
client.on_message = on_message

# Connect to the MQTT broker (replace 'broker_address' and 'port' with your broker's address and port)
client.connect("broker.hivemq.com", port=1883, keepalive=60)

# Start the MQTT loop to listen for messages
client.loop_forever()
