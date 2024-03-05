import paho.mqtt.client as mqtt
import mysql.connector
import json

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
broker_address = db_mqtt_settings['mqtt']['broker']
port = db_mqtt_settings['mqtt']['port']
topic = "gip/queries"

# Callback when the client connects to the broker
def on_connect(client, userdata, flags, rc):
    client.subscribe(topic)

# Callback when a message is received from the broker
def on_message(client, userdata, msg):
    mqtt_payload = json.loads(msg.payload.decode())

    db.reconnect()
    if mqtt_payload['returnData']:
        
        mycursor.execute(mqtt_payload['query'])
        myresult = mycursor.fetchall()
        if myresult:
            response = []
            for row in myresult:
                response.append(row)
            client.publish(mqtt_payload['UUID'], json.dumps(response))
        else:
            client.publish(mqtt_payload['UUID'], "{}")
    else:
        mycursor.execute(mqtt_payload['query'])
        db.commit()


# Create MQTT client instance with no client_id
client = mqtt.Client(client_id="", clean_session=True)

# Set last will message
will_topic = "disconnected"
will_message = "Connection lost unexpectedly"
client.will_set(will_topic, will_message, 2, False)

# Set callback functions
client.on_connect = on_connect
client.on_message = on_message

# Connect to the broker
client.connect(broker_address, port, 60)

# Start the network loop
while True:
    client.loop_start()