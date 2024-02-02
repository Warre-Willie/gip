connect_wifi()
client = connect_mqtt(callback)import feedback_alerts

def callback(topic, msg):
    response = msg.decode('utf-8')

connect_wifi()
client = connect_mqtt(callback)