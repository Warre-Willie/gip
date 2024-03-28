from machine import Pin,UART
import time
uart = UART(0, baudrate=9600, tx=Pin(0), rx=Pin(1))
uart.init(bits=8, parity=None, stop=1)

# The while loop continuously checks for any data available on the UART connection.
# If data is available, it reads the data, decodes it, and prints it.
# The loop then pauses for 1 second before checking for data again.
while True:
    if uart.any(): 
        data = uart.read()
        if data != "":
            data = data.decode()
            print(data)
    time.sleep(1)
