"""
File: zone.py
Author: Warre Willeme & Jesse UijtdeHaag
Date: 29-04-2024
Description: This is the main script for the zone counter.
"""
from machine import Pin
import time
import machine
import json
from neopixel import Neopixel
from network_setup import connect_wifi, connect_mqtt

# set zone id
with open('zone_config.json', 'r') as f:
    config = json.load(f)

zone_id = config['zone_id']

# start-up switch
switch = Pin(20, Pin.IN)

# start-up Laser
laser = Pin(21, Pin.IN)
laser_state = False
last_laser_state = False

#reconnects if connection is lost MQTT
def reconnect():
    print('Failed to connect to the MQTT Broker. Reconnecting...')
    time.sleep(5)
    machine.reset()

# setup ledstrip
numpix = 30
pixels = Neopixel(numpix, 0, 28, "GRB")
x = int(0)
y = int(0)
old_user = ""

orange = (255, 50, 0)
green = (0, 255, 0)
red = (255, 0, 0)
none = (0,0,0)

pixels.brightness(50)

# def for LED-pixles
def down(pixel_count1, pixel_count2,color1, color2):
    x = pixel_count1
    y = pixel_count2
    while (x != pixel_count1-10):
        pixels.set_pixel_line(0, 29, none)
        pixels.set_pixel_line(pixel_count2, x-1, color1)
        pixels.set_pixel_line(y, pixel_count2, color2)
        time.sleep_ms(70)
        pixels.show()

        x -= 1
        y -= 1

def up(pixel_count1, pixel_count2,color1, color2):

    x = pixel_count1
    y = pixel_count2
    while (x != pixel_count1+10):
         pixels.set_pixel_line(0, 29, none)
         pixels.set_pixel_line(x , pixel_count2, color1)
         pixels.set_pixel_line(pixel_count2  , y  , color2)
         time.sleep_ms(70)
         pixels.show()

         x += 1
         y += 1

#Incoming messages subscriptions
def callback(topic, msg):
    print(msg)
    response = msg.decode('utf-8')
    response_dict = json.loads(response)
    if(response_dict['id'] == zone_id):
        color_barometer(response_dict)
        

# set barometer incomming json: {"id": 1,"color": "{green, orange, red}"}        
def color_barometer(response_dict):
    global old_user
    #green
    if response_dict["color"] == "green":
        if(old_user == "orange"):
            down(19, 9, orange, green)
        elif(old_user == "green"): 
            pixels.set_pixel_line(0,9 , green)
            pixels.show()            
        else:
            down(30, 20,red, orange)
            down(19, 9, orange, green)  
    #orange
    elif response_dict["color"] == "orange":
        if(old_user == "orange"):
            pixels.set_pixel_line(10,19 , orange)
            pixels.show()
        elif(old_user == "green"):
            up(1,10,green,orange)
        else:
            down(29, 19, red, orange)
    #red
    else:
        if(old_user == "orange"):
            up(11,20 ,orange, red)
        elif(old_user == "green"):
            up(1,10,green,orange)
            up(11,20 ,orange, red)
        else:
            pixels.set_pixel_line(20,29 , red)
            pixels.show()

    old_user = response_dict["color"]


connect_wifi()
if(switch.value() == True):
    client = connect_mqtt(callback, "Zone" + str(zone_id) + "ingang")
else:
    client = connect_mqtt(callback, "Zone" + str(zone_id) + "uitgang")

# Set last will message
client.set_last_will("gip/notification", '{ "isCounter": true, "id": ' + str(zone_id) + ', "isExit": ' + str(switch.value()) +', "category": "Alert" }', 2, False)


# subscribe to topic
client.subscribe("gip/teller/barometer")
client.publish("gip/teller/new_device", '{"id": ' + str(zone_id) + '}')
client.publish("gip/notification", '{ "isCounter": true, "id": ' + str(zone_id) + ', "isExit": ' + str(switch.value()) + ', "category": "Info" }', 2, False)

while True:
    client.check_msg()
    laser_state = not(laser.value())
    if(last_laser_state == True and laser_state == False):
        
        if(switch.value() == True):
            client.publish("gip/teller/counter", '{"id": '+ str(zone_id) + ',"people": 1}')
        else:
            client.publish("gip/teller/counter", '{"id": '+ str(zone_id) + ',"people": -1}')
    last_laser_state = laser_state 
