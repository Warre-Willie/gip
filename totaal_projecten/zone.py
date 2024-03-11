from machine import Pin
import time
import machine
import json
from neopixel import Neopixel
from network_setup import connect_wifi, connect_mqtt
import uasyncio as asyncio

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

#Incoming messages subscriptions
def callback(topic, msg):
    response = msg.decode('utf-8')
    response_dict = json.loads(response)
    if(response_dict['id'] == zone_id):
        # color_barometer(response_dict)
        loop.create_task(color_barometer(response_dict))

loop = asyncio.get_event_loop()

# set barometer incomming json: {"id": 1,"color": "{green, orange, red}"}        
async def color_barometer(response_dict):
    global previous_color
    #green
    if response_dict["color"] == "green":
        if(previous_color == "orange"):
            down(19, 9, orange, green)
        elif(previous_color == "green"): 
            pixels.set_pixel_line(0,9 , green)
            pixels.show()            
        else:
            down(30, 20,red, orange)
            down(19, 9, orange, green)  
    #orange
    elif response_dict["color"] == "orange":
        if(previous_color == "orange"):
            pixels.set_pixel_line(10,19 , orange)
            pixels.show()
        elif(previous_color == "green"):
            up(1,10,green,orange)
        else:
            down(29, 19, red, orange)
    #red
    else:
        if(previous_color == "orange"):
            up(11,20 ,orange, red)
        elif(previous_color == "green"):
            up(1,10,green,orange)
            up(11,20 ,orange, red)
        else:
            pixels.set_pixel_line(20,29 , red)
            pixels.show()

    previous_color = response_dict["color"]


connect_wifi()
client = connect_mqtt(callback)
# subscribe to topic
client.subscribe("/teller/barometer")

#get collor from database
client.publish("/teller/new_device", '{"id": ' + str(zone_id) + '}')

while True:
    client.check_msg()
    laser_state = not(laser.value())
    if(last_laser_state == True and laser_state == False):
        
        if(switch.value() == True):
            client.publish("/teller", '{"id": '+ str(zone_id) + ',"people": 1}')
        else:
            client.publish("/teller", '{"id": '+ str(zone_id) + ',"people": -1}')
    last_laser_state = laser_state 
