# network_setup.py
import network
import time
import json
from umqtt.simple import MQTTClient

def load_credentials():
    with open('network_config.json', 'r') as f:
        return json.load(f)

def connect_wifi():
    credentials = load_credentials()
    ssid = credentials['wifi']['ssid']
    password = credentials['wifi']['password']

    wlan = network.WLAN(network.STA_IF)
    wlan.active(True)
    if not wlan.isconnected():
        print('connecting to network...')
        wlan.connect(ssid, password)
        while not wlan.isconnected():
            time.sleep(1)
    print('network config:', wlan.ifconfig())

def connect_mqtt(callback):
    credentials = load_credentials()
    server = credentials['mqtt']['server']
    port = credentials['mqtt']['port']
    user = credentials['mqtt']['user']
    password = credentials['mqtt']['password']

    client = MQTTClient(b"", server)
    client.set_callback(callback)
    client.connect()
    print('connected to MQTT server')
    return client