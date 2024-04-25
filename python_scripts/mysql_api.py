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
if db_mqtt_settings["isDevlopment"] == True:
    broker_address = db_mqtt_settings['mqtt']["mqttDev"]['broker']
    port = db_mqtt_settings['mqtt']["mqttDev"]['port']
else:
    broker_address = db_mqtt_settings['mqtt']["mqttProd"]['broker']
    port = db_mqtt_settings['mqtt']["mqttProd"]['port']

topic = "gip/queries"

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
client = mqtt.Client(callback_api_version=mqtt.CallbackAPIVersion.VERSION2,client_id="", clean_session=True)

# Set last will message
will_topic = "disconnected"
will_message = "Connection lost unexpectedly"
client.will_set(will_topic, will_message, 2, False)

# Set callback functions
client.on_message = on_message

# Connect to the broker
client.connect(broker_address, port, 60)

client.subscribe(topic)
# Start the network loop
while True:
    client.loop_start()