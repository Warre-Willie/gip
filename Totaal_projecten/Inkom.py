import network
from machine import Pin
from time import sleep
from umqtt.simple import MQTTClient

# start-up laser
laser = Pin(21, Pin.IN)
led = machine.Pin('LED', machine.Pin.OUT)
laser_state = True
global last_laser = False

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

# control the state of the laser
def control_laser():
    global last_laser  

    if not laser.value():
        led.value(True)
        laser_state = True
    else:
        led.value(False)
        laser_state = False

    if last_laser and not laser_state:
        client.publish("Count_update", "1")

    last_laser = laser_state

    
    
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


while(True):
    control_laser()
