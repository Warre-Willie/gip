from machine import Pin, UART
import time

uart = UART(0, baudrate=9600, tx=Pin(0), rx=Pin(1))
uart.init(bits=8, parity=None, stop=1)

# Define the trigger command in hexadecimal
trigger_command_hex = "7E000801000201ABCD"

# Convert the hexadecimal string to bytes
trigger_command_bytes = bytes.fromhex(trigger_command_hex)

# Send the trigger command
uart.write(trigger_command_bytes)

while True:
    if uart.any():
        data = uart.read()
        if data != "":
            data = data.decode()
            print(data)
            time.sleep(5)
            uart.write(trigger_command_bytes)
