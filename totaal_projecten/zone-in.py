import network
from machine import Pin
import time
from umqtt.simple import MQTTClient
from neopixel import Neopixel

# start-up laser
laser = Pin(21, Pin.IN)
led = Pin('LED', Pin.OUT)
laser_state = bool()
last_laser = bool()

# start-up MQTT
# connection WiFi
wlan = network.WLAN(network.STA_IF)
wlan.active(True)
wlan.connect("TP-LINK_EE42","29487868")
time.sleep(5)
print(wlan.ifconfig())

# conection server
mqtt_server = 'broker.hivemq.com'
client_id = 'Laser_01'

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

# control the state of the laser
def control_laser():
    global last_laser  

    if not(laser.value()):
        led.value(True)
        laser_state = True
    else:
        led.value(False)
        laser_state = False

    if last_laser and not laser_state:
        client.publish("Count_update", "1")

    last_laser = laser_state

#def led flow
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

def sub_cb(topic, msg):
    global old_user
    print("New message on topic {}".format(topic.decode('utf-8')))
    msg = msg.decode('utf-8')
    if msg == "green":
        if(old_user == "orange"):
            down(19, 9, orange, green)
        elif(old_user == "green"): 
            pixels.set_pixel_line(0,9 , green)
            pixels.show()            
        else:
            down(30, 20,red, orange)
            down(19, 9, orange, green)  

    elif msg == "orange":
        if(old_user == "orange"):
            pixels.set_pixel_line(10,19 , orange)
            pixels.show()
        elif(old_user == "green"):
            up(1,10,green,orange)
        else:
            down(29, 19, red, orange)

    else:
        if(old_user == "orange"):
            up(11,20 ,orange, red)
        elif(old_user == "green"):
            up(1,10,green,orange)
            up(11,20 ,orange, red)
        else:
            pixels.set_pixel_line(20,29 , red)
            pixels.show()

    old_user = msg
    
#make connection mqtt    
def mqtt_connect():
    client = MQTTClient(client_id, mqtt_server, keepalive=3600)
    client.set_callback(sub_cb)
    client.connect()
    client.subscribe("Led_state")
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


    

while(True):
    control_laser()


