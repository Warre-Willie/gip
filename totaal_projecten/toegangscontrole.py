from mfrc522 import MFRC522
import feedback_alerts
from machine import Pin, I2C, PWM, UART
from pico_i2c_lcd import I2cLcd
import time
import urandom
import ubinascii
import json
import uasyncio as asyncio
from network_setup import connect_wifi, connect_mqtt

#set the zone
zone = "2"

led_red_pin = Pin(10, Pin.OUT)
led_green_pin = Pin(11, Pin.OUT)

buzzer_pwm = PWM(Pin(18))

reader = MFRC522(spi_id=0,sck=6,miso=4,mosi=7,cs=5,rst=22)

I2C_ADDR = 0x27
I2C_NUM_ROWS = 2
I2C_NUM_COLS = 16
i2c = I2C(1, sda=Pin(26), scl=Pin(27), freq=400000)
lcd = I2cLcd(i2c, I2C_ADDR, I2C_NUM_ROWS, I2C_NUM_COLS)

lcd.backlight_on()

db_response = {}

def callback(topic, msg):
    response = msg.decode('utf-8')
    global db_response
    db_response = json.loads(response)

def execute_query(query, returnData):
    feedback_alerts.lcd_display(lcd, "Laden...", "")

    mqtt_msg_dict = {}
    uuid = gen_uuid()
    mqtt_msg_dict['UUID'] = uuid
    mqtt_msg_dict['returnData'] = returnData
    mqtt_msg_dict['query'] = query
    json_string = json.dumps(mqtt_msg_dict)
    if returnData:
        client.subscribe(uuid)
    client.publish("gip/queries", json_string)


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


connect_wifi()
client = connect_mqtt(callback)

while True:
    lcd.putstr("Scan RFID badge...")
    reader.init()
    (stat, tag_type) = reader.request(reader.REQIDL)
    if stat == reader.OK:
        (stat, uid) = reader.SelectTagSN()
        if stat == reader.OK:
            card = int.from_bytes(bytes(uid),"little",False)
            execute_query("SELECT * FROM `tickets` WHERE `rfid` = '" + card + "'", True)
            while not db_response:
                client.check_msg()
                time.sleep(0.1)
            if db_response:
                if db_response[0]['access'] == zone:
                    await feedback_alerts.play_confirmation(buzzer_pwm)
                else:
                    await feedback_alerts.play_error(buzzer_pwm)
                    await feedback_alerts.error_blink(led_red_pin)
            else:
                led_red_pin.value(1)
                buzzer_pwm.freq(880)
                buzzer_pwm.duty_u16(32768)
                time.sleep(0.1)
                buzzer_pwm.duty_u16(0)
                led_red_pin.value(0)
            db_response = {}
            time.sleep(0.5)
    time.sleep(0.1)

