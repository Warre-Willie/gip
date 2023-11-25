import network
import time
from machine import Pin
from umqtt.simple import MQTTClient

wlan = network.WLAN(network.STA_IF)
wlan.active(True)
wlan.connect("TP-LINK_EE42","29487868")
time.sleep(5)
print(wlan.ifconfig())

LED = Pin(0, Pin.OUT)

mqtt_server = '192.168.0.101'
client_id = 'test2'
topic_sub = b'led'
counter = 0
old_msg = ""


def sub_cb(topic, msg):
    print("New message on topic {}".format(topic.decode('utf-8')))
    msg = msg.decode('utf-8')
    
    if msg != old_msg:
        match msg:
            case "-":
                counter - 1
            case "+":
                counter + 1
    
    old_msg = msg
def mqtt_connect():
    client = MQTTClient(client_id, mqtt_server, keepalive=60)
    client.set_callback(sub_cb)
    client.connect()
    print('Connected to %s MQTT Broker'%(mqtt_server))
    return client

def reconnect():
    print('Failed to connect to MQTT Broker. Reconnecting...')
    time.sleep(5)
    machine.reset()
    
try:
    client = mqtt_connect()
except OSError as e:
    reconnect()


while True:
    client.subscribe(topic_sub)
    time.sleep(1)
