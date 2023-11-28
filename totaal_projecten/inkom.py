from mfrc522 import MFRC522
import utime

from machine import Pin, I2C, PWM
from lcd_api import LcdApi
from pico_i2c_lcd import I2cLcd

from machine import Pin,UART
import time

led_red_pin = Pin(10, Pin.OUT)
led_green_pin = Pin(11, machine.Pin.OUT)
buzzer_pin = PWM(Pin(18))
buzzer_pin.freq(1000)

uart = UART(0, baudrate=9600, tx=Pin(0), rx=Pin(1))
uart.init(bits=8, parity=None, stop=1)

reader = MFRC522(spi_id=0,sck=6,miso=4,mosi=7,cs=5,rst=22)

I2C_ADDR = 0x27
I2C_NUM_ROWS = 2
I2C_NUM_COLS = 16
i2c = I2C(1, sda=machine.Pin(26), scl=machine.Pin(27), freq=400000)
lcd = I2cLcd(i2c, I2C_ADDR, I2C_NUM_ROWS, I2C_NUM_COLS)

lcd.backlight_on()
lcd.putstr("Scan barcode")
 
def buzz(frequency, duration):
    buzzer_pin.freq(frequency)
    buzzer_pin.duty_u16(32767)
    utime.sleep_ms(duration)
    buzzer_pin.duty_u16(0)
 
barcode = "8720017991611"
while True:
    buzzer_pin.duty_u16(65535)
    if uart.any(): 
        data = uart.read()
        if data != "":
            data = data.decode()
            if data.startswith(barcode):
                lcd.clear()
                lcd.move_to(0,0)
                lcd.putstr("Scan RFID badge")
                barcode_scanned = True
                while barcode_scanned:
                    reader.init()
                    (stat, tag_type) = reader.request(reader.REQIDL)
                    if stat == reader.OK:
                        (stat, uid) = reader.SelectTagSN()
                        if stat == reader.OK:
                            card = int.from_bytes(bytes(uid),"little",False)
                            barcode_scanned = False
                            lcd.clear()
                            lcd.move_to(0,0)
                            lcd.putstr("Sycronisatie")
                            lcd.move_to(0,1)
                            lcd.putstr("Succesvol")
                print(f"Barcode: {barcode} is sync to RFID-badge: {str(card)}")
            else:
                lcd.clear()
                lcd.move_to(0,0)
                lcd.putstr("Barcode niet")
                lcd.move_to(0,1)
                lcd.putstr("gevonden")
utime.sleep_ms(500) 





