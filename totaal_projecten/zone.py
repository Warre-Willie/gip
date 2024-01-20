import network
from machine import Pin
import time
import machine
import json
from umqtt.simple import MQTTClient
from neopixel import Neopixel

# set zone id
zone_id = "1"

# start-up switch
switch = Pin(20, Pin.IN)

# start-up Laser
laser = Pin(21, Pin.IN)
laser_state = False
last_laser = False

# start-up LED
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

#Incoming messages subscriptions
def sub_cb(topic, msg):
    print("New message on topic {}".format(topic.decode('utf-8')))
    msg = msg.decode('utf-8')
    print(msg)


#check laser
def laser_check(): 
    

# MQTT connection
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


client.subscribe("barometer")

while True:

    laser_state = not(laser.value())

    if(last_laser == True and laser_state == False):
        if(switch.value() == True):
            client.publish("Jesse", "{'id':"+ zone_id + ",'poeple': 1}")
        else:
            client.publish("Jesse", "{'id':"+ zone_id + ",'poeple': -1}")    

    last_laser_state = laser_state 

    
    

