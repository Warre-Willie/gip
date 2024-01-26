import network
import time
from machine import Pin, reset
from umqtt.simple import MQTTClient

wlan = network.WLAN(network.STA_IF)
wlan.active(True)
wlan.connect("TP-LINK_EE42", "29487868")
time.sleep(5)
print(wlan.ifconfig())

topic_sub = b"my_topic"
broker_url= b""
username = b""
password = b""


def on_msg(topic, msg):
    print("New message on topic {}".format(topic.decode('utf-8')))
    msg = msg.decode('utf-8')
    print(msg)
    if msg == "on":
        LED.on()
    elif msg == "off":
        LED.off()

def connectMQTT():
    client = MQTTClient(
        client_id=b"",
        server=broker_url,
        port=8883,
        user=username,
        password=password,
        keepalive=7200,
        ssl=True,
        ssl_params={'server_hostname': broker_url}
    )
    print("Connected to MQTT Broker.")
    return client

def reconnect():
    print('Failed to connect to MQTT Broker. Reconnecting...')
    time.sleep(5)
    reset()

try:
    client = connectMQTT()
    client.connect()
    client.set_callback(on_msg)
except MQTTClient.MQTTException as e:
    reconnect()
    
client.subscribe(topic_sub)
while True:
    client.check_msg()
    time.sleep(1)
