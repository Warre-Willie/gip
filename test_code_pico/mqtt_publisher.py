import network
import time
from machine import Pin
from umqtt.simple import MQTTClient

wlan = network.WLAN(network.STA_IF)
wlan.active(True)
wlan.connect("TP-LINK_EE42","29487868")
time.sleep(5)
print(wlan.ifconfig())

button_on = Pin(0, Pin.IN, Pin.PULL_DOWN)
button_off = Pin(1, Pin.IN, Pin.PULL_DOWN)

mqtt_server = '192.168.0.101'
client_id = 'test'
topic_pub = b'led'
topic_on_msg = b'on'
topic_off_msg = b'off'

def mqtt_connect():
    client = MQTTClient(client_id, mqtt_server, keepalive=3600)
    client.connect()
    print('Connected to %s MQTT Broker'%(mqtt_server))
    return client

def reconnect():
    print('Failed to connect to the MQTT Broker. Reconnecting...')
    time.sleep(5)
    # machine.reset()

try:
    client = mqtt_connect()
except OSError as e:
    reconnect()
    

while True:
    if button_on.value() == 1:
        client.publish(topic_pub, topic_on_msg)
    elif button_off.value() == 1:
        client.publish(topic_pub, topic_off_msg)