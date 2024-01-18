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

mycursor.close()
#print(thresholds[1]['green'])



# MQTT settings
broker_address = "broker.hivemq.com"
port = 1883


# Callback when a message is received from the broker
# The incomming data will be in the format of json: {"id": 1,"people": -1}
def on_message(client, userdata, msg):
    counter = 0
    response = json.loads(msg.payload.decode())

    mycursor = db.cursor(dictionary=True) # Dictionary true for ease of processing respones
    mycursor.execute(f"SELECT `people_count`, `barometer_lock` FROM `zones` WHERE `id` = '{response['id']}'")
    for row in mycursor:
        if row["people_count"] != None or row["people_count"] != 0:
            counter = int(row["people_count"])
            counter += response["people"]
            mycursor.execute(f"UPDATE `zones` SET `people_count`= '{str(counter)}' WHERE `id` = '{response['id']}'")
            db.commit()
            print(counter)
        else:
            print("No data")

        if row["barometer_lock"] == 0 and row["people_count"] != None:
            if counter <= thresholds[response['id']]['green']:
                client.publish(f"barometer", '{"id": ' + str(response['id']) + ', "color": "green"}')
                mycursor.execute(f"UPDATE `zones` SET `barometer_color`= 'green' WHERE `id` = '{response['id']}'")
                db.commit()
            elif counter <= thresholds[response['id']]['orange']:
                client.publish(f"barometer", '{"id": ' + str(response['id']) + ', "color": "orange"}')
                mycursor.execute(f"UPDATE `zones` SET `barometer_color`= 'orange' WHERE `id` = '{response['id']}'")
                db.commit()
            elif counter >= thresholds[response['id']]['red']:
                client.publish(f"barometer", '{"id": ' + str(response['id']) + ', "color": "red"}')
                mycursor.execute(f"UPDATE `zones` SET `barometer_color`= 'red' WHERE `id` = '{response['id']}'")
                db.commit()
        else:
            print("Barometer locked")
      
# Create MQTT client instance with no client_id
client = mqtt.Client(client_id="", clean_session=True)


# Set callback functions
# client.on_connect = on_connect
client.on_message = on_message

# Connect to the broker
client.connect(broker_address, port, 60)

client.subscribe("Jesse")
# Start the network loop
while True:
    client.loop_start()
    