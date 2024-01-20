import network
from machine import Pin
import time
import machine
import json
from umqtt.simple import MQTTClient
from neopixel import Neopixel

# start-up laser
laser = Pin(21, Pin.IN)
led = machine.Pin('LED', machine.Pin.OUT)
laser_state = bool()
last_laser = bool()

numpix = 30
pixels = Neopixel(numpix, 0, 28, "GRB")
x = int(0)
y = int(0)
old_user = ""

orange = (255, 50, 0)
green = (0, 255, 0)
red = (255, 0, 0)
none = (0,0,0)

pixels.brightness(100)

# start-up MQTT
# connection WiFi
wlan = network.WLAN(network.STA_IF)
wlan.active(True)
wlan.connect("JUWiFi","pasword")
time.sleep(5)
print(wlan.isconnected())

# conection server
mqtt_server = 'broker.hivemq.com'
client_id = 'Laser'


def sub_cb(topic, msg):
    print("New message on topic {}".format(topic.decode('utf-8')))
    msg = msg.decode('utf-8')
    print(msg)
    

def mqtt_connect():
    client = MQTTClient(client_id, mqtt_server, keepalive=3600)
    client.connect()
    print('Connected to %s MQTT Broker'%(mqtt_server))
    return client

def reconnect():
    print('Failed to connect to the MQTT Broker. Reconnecting...')
    time.sleep(5)
    machine.reset()

try:
    client = mqtt_connect()
except OSError as e:
    reconnect()



while True:
    client.subscribe(topic_sub)
    client.publish(topic_pub, topic_msg)
    

