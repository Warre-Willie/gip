import time
import ubinascii
import urandom
import json
import network
from umqtt.simple import MQTTClient

wlan = network.WLAN(network.STA_IF)
wlan.active(True)
wlan.connect("TP-LINK_EE42","29487868")
while wlan.isconnected() == False:
        print('Waiting for connection...')
        time.sleep(1)
print(wlan.ifconfig())

def callback(topic, msg):
    response = msg.decode('utf-8')
    response_dict = json.loads(response)
    print(response_dict['barcode'])

client = MQTTClient(b"", "broker.hivemq.com")
client.set_callback(callback)
client.connect()

def gen_uuid():
     uuid = ""
     x = 4
     while x != 0:
        random_bytes = urandom.getrandbits(8 * 4).to_bytes(4, 'big')
        random_hex_string = ubinascii.hexlify(random_bytes).decode('utf-8')
        uuid += random_hex_string
        x -= 1
        if(x != 0):
             uuid += "-"
     return uuid
     
mqtt_msg_dict = {}
uuid = gen_uuid()
mqtt_msg_dict['UUID'] = uuid
mqtt_msg_dict['returnData'] = True
mqtt_msg_dict['query'] = "SELECT * FROM tickets WHERE barcode=%8720017991611%;"
json_string = json.dumps(mqtt_msg_dict)

client.subscribe(uuid)
client.publish("queries", json_string)
while True:
     client.wait_msg()