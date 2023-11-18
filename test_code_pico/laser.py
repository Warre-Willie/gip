from machine import Pin
from time import sleep

laser1 = Pin(21, Pin.IN)
led = machine.Pin('LED', machine.Pin.OUT)

while(True):
   if(laser1.value()):
        led.value(True)
   else:
       led.value(False)