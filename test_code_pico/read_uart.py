from machine import Pin,UART
import time
uart = UART(0, baudrate=9600, tx=Pin(0), rx=Pin(1))
uart.init(bits=8, parity=None, stop=1)

while True:
    if uart.any(): 
        data = uart.read()
        if data != "":
            data = data.decode()
            print(data)
    time.sleep(1)
