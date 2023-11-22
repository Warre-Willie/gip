from machine import Pin,UART
import time
uart = UART(1, baudrate=9600, tx=Pin(4), rx=Pin(5))
uart.init(bits=8, parity=None, stop=1)

while True:
    if uart.any(): 
        data = uart.read()
        if data != "":
            data = data.decode()
            print(data)
    time.sleep(1)
