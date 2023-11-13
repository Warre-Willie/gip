from machine import Pin
from time import sleep

laser1 = Pin(21, Pin.IN)

while(True):
    if(laser1.value()):
        print("Detected")
