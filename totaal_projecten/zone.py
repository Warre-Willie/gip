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


# start-up MQTT
# connection WiFi
wlan = network.WLAN(network.STA_IF)
wlan.active(True)
wlan.connect("TP-LINK_EE42","29487868")
while wlan.isconnected() == False:
        print('Waiting for connection...')
        time.sleep(1)
print(wlan.ifconfig())

# conection server
client = MQTTClient(b"", "broker.hivemq.com")
client.connect()

#Incoming messages subscriptions
def callback(topic, msg):
    response = msg.decode('utf-8')
    response_dict = json.loads(response)
    if(response_dict['id'] == zone_id):
        x = 0
    user = input("geef een getal 1(groen) 2(oranje) 3(rood):")
    if user == "1":
        if(old_user == "2"):
            down(19, 9, orange, green)
        elif(old_user == "1"): 
            pixels.set_pixel_line(0,9 , green)
            pixels.show()            
        else:
            down(30, 20,red, orange)
            down(19, 9, orange, green)  

    elif user == "2":
        if(old_user == "2"):
            pixels.set_pixel_line(10,19 , orange)
            pixels.show()
        elif(old_user == "1"):
            up(1,10,green,orange)
        else:
            down(29, 19, red, orange)

    else:
        if(old_user == "2"):
            up(11,20 ,orange, red)
        elif(old_user == "1"):
            up(1,10,green,orange)
            up(11,20 ,orange, red)
        else:
            pixels.set_pixel_line(20,29 , red)
            pixels.show()

    old_user = user

    

def reconnect():
    print('Failed to connect to the MQTT Broker. Reconnecting...')
    time.sleep(5)
    machine.reset()


client.subscribe("barometer")

while True:

    laser_state = not(laser.value())

    if(last_laser == True and laser_state == False):
        if(switch.value() == True):
            client.publish("Jesse", "{'id':"+ zone_id + ",'poeple': 1}")
        else:
            client.publish("Jesse", "{'id':"+ zone_id + ",'poeple': -1}")    

    last_laser_state = laser_state 

    
    

