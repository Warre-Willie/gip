from mfrc522 import MFRC522
from machine import Pin, I2C, PWM, UART
from lcd_api import LcdApi
from pico_i2c_lcd import I2cLcd
import time
import network
from umqtt.simple import MQTTClient
import ubinascii
import urandom
import json
import uasyncio as asyncio

wlan = network.WLAN(network.STA_IF)
wlan.active(True)
wlan.connect("TP-LINK_EE42","29487868")
while wlan.isconnected() == False:
        print('Waiting for connection...')
        time.sleep(1)
print(wlan.ifconfig())

led_red_pin = Pin(10, Pin.OUT)
led_green_pin = Pin(11, machine.Pin.OUT)

buzzer_pin = machine.Pin(18)
buzzer_pwm = machine.PWM(buzzer_pin)

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

db_response = {}

def callback(topic, msg):
    response = msg.decode('utf-8')
    response_dict = json.loads(response)
    global db_response
    if (len(response_dict) == 1):
        db_response = response_dict[0]
    else:
        db_response = response_dict

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

def play_confirmation_sound():
    buzzer_pwm.freq(1200)
    buzzer_pwm.duty_u16(16383)
    time.sleep(0.1)
    buzzer_pwm.duty_u16(0)

    buzzer_pwm.freq(1300)
    buzzer_pwm.duty_u16(16383)
    time.sleep(0.1)
    buzzer_pwm.duty_u16(0)

    buzzer_pwm.freq(1400)
    buzzer_pwm.duty_u16(16383)
    time.sleep(0.1)
    buzzer_pwm.duty_u16(0)
 
async def play_error_sound():
    buzzer_pwm.freq(300)
    buzzer_pwm.duty_u16(16383)
    await asyncio.sleep_ms(600)
    buzzer_pwm.duty_u16(0)

async def red_led_error():
    x = 7
    while x > 0:
        led_red_pin.on()
        await asyncio.sleep(0.1)
        led_red_pin.off()
        await asyncio.sleep(0.07)
        x -= 1

def execute_query(query, returnData):
    mqtt_msg_dict = {}
    uuid = gen_uuid()
    mqtt_msg_dict['UUID'] = uuid
    mqtt_msg_dict['returnData'] = returnData
    mqtt_msg_dict['query'] = query
    json_string = json.dumps(mqtt_msg_dict)
    if returnData:
        client.subscribe(uuid)
    client.publish("gip/queries", json_string)

async def synching_process(barcode):
    global db_response    
    if bool(db_response):
        if db_response['RFID'] == None and db_response['barcode'] == str(int(barcode)):
            play_confirmation_sound()
            lcd.clear()
            lcd.move_to(0, 0)
            lcd.putstr("Scan RFID badge")
            led_green_pin.on()

            while True:
                reader.init()
                (stat, tag_type) = reader.request(reader.REQIDL)

                if stat == reader.OK:
                    (stat, uid) = reader.SelectTagSN()
                    if stat == reader.OK:
                        badge = int.from_bytes(bytes(uid),"little",False)

                        query = f'SELECT * FROM tickets WHERE RFID="{str(badge)}";'
                        execute_query(query, True)
                        client.wait_msg()
                        if not bool(db_response):
                            play_confirmation_sound()

                            lcd.clear()
                            lcd.move_to(0, 0)
                            lcd.putstr("Synchronisatie")
                            lcd.move_to(0, 1)
                            lcd.putstr("Succesvol")

                            query = f"UPDATE tickets SET RFID=%{str(badge)}% WHERE barcode=%{str(int(barcode))}%;"
                            execute_query(query, False)
                            db_response = {}
                            led_green_pin.off()
                            break
                        else:
                            await asyncio.gather(play_error_sound(), red_led_error())
                            lcd.clear()
                            lcd.move_to(0, 0)
                            lcd.putstr("Badge al")
                            lcd.move_to(0, 1)
                            lcd.putstr("gekoppeld")
                            

                if uart.any():
                    barcode = uart.read().decode()
                    if barcode != None and barcode != db_response['barcode']:
                        db_response = {}
                        query = f'SELECT * FROM tickets WHERE barcode="{str(int(barcode))}";'
                        execute_query(query, True)
                        client.wait_msg()
                        led_green_pin.off()
                        asyncio.run(synching_process(str(int(barcode))))
        else:
            await asyncio.gather(play_error_sound(), red_led_error())
            lcd.clear()
            lcd.move_to(0, 0)
            lcd.putstr("Ticket al")
            lcd.move_to(0, 1)
            lcd.putstr("gescand")
            
    else:
        await asyncio.gather(play_error_sound(), red_led_error())
        lcd.clear()
        lcd.move_to(0, 0)
        lcd.putstr("Ticket niet")
        lcd.move_to(0, 1)
        lcd.putstr("gevonden")


client = MQTTClient(b"", "broker.hivemq.com")
client.set_callback(callback)
client.connect()


def main():
    while True:
        if uart.any():
            barcode = uart.read().decode()
            if barcode != "":
                query = f'SELECT * FROM tickets WHERE barcode=%{str(int(barcode))}%;'
                execute_query(query, True)
                client.wait_msg()
                asyncio.run(synching_process(str(int(barcode))))
                

if __name__ == "__main__":
    main()
