"""
File: mysql_api.py
Author: Warre Willeme & Jesse UijtdeHaag
Date: 29-04-2024
Description: This is the start of the network setup library.
"""
# network_setup.py
import network
import time
import json
from umqtt.simple import MQTTClient

def load_network_config():
    with open('network_config.json', 'r') as f:
        return json.load(f)

def connect_wifi():
    network_config = load_network_config()
    ssid = network_config['wifi']['ssid']
    password = network_config['wifi']['password']

    wlan = network.WLAN(network.STA_IF)
    wlan.active(True)
    if not wlan.isconnected():
        print('connecting to network...')
        wlan.connect(ssid, password)
        while not wlan.isconnected():
            time.sleep(1)
    print('network config:', wlan.ifconfig())

def connect_mqtt(callback):
    network_config = load_network_config()
    if network_config["isDevelopment"]:
        server = network_config['mqtt']["mqttDev"]['broker']
        # port = network_config['mqtt']["mqttDev"]['port']
        # user = network_config['mqtt']["mqttDev"]['user']
        # password = network_config['mqtt']["mqttDev"]['password']
    else:
        server = network_config['mqtt']["mqttProd"]['broker']
        # port = network_config['mqtt']["mqttProd"]['port']
        # user = network_config['mqtt']["mqttProd"]['user']
        # password = network_config['mqtt']["mqttProd"]['password']
        
    client = MQTTClient(b"", server)
    client.set_callback(callback)
    client.connect()
    print('connected to MQTT server')
    return client