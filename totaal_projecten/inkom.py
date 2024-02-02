from mfrc522 import MFRC522
import feedback_alerts
from machine import Pin, I2C, PWM, UART
from pico_i2c_lcd import I2cLcd
import time
import ubinascii
import urandom
import json
import uasyncio as asyncio
from network_setup import connect_wifi, connect_mqtt

led_red_pin = Pin(10, Pin.OUT)
led_green_pin = Pin(11, Pin.OUT)

buzzer_pwm = PWM(Pin(18))

uart = UART(0, baudrate=9600, tx=Pin(0), rx=Pin(1))
uart.init(bits=8, parity=None, stop=1)
trigger_command_bytes = bytes.fromhex("7E000801000201ABCD")

reader = MFRC522(spi_id=0,sck=6,miso=4,mosi=7,cs=5,rst=22)

I2C_ADDR = 0x27
I2C_NUM_ROWS = 2
I2C_NUM_COLS = 16
i2c = I2C(1, sda=Pin(26), scl=Pin(27), freq=400000)
lcd = I2cLcd(i2c, I2C_ADDR, I2C_NUM_ROWS, I2C_NUM_COLS)

lcd.backlight_on()

db_response = {}
current_barcode = ""

def callback(topic, msg):
    response = msg.decode('utf-8')
    global db_response
    db_response = json.loads(response)

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

async def lcd_succeed_cooldown():
    time.sleep(5)
    feedback_alerts.lcd_display(lcd, "Scan barcode...", "")

def barcode_scanned(scanned_barcode):
    global current_barcode
    current_barcode = scanned_barcode
    query = f'SELECT * FROM tickets WHERE barcode="{scanned_barcode}";'
    execute_query(query, True)
    client.wait_msg()
    asyncio.run(synching_process())
                                 
async def synching_process():
    global db_response
    global current_barcode
    if bool(db_response):
        if db_response[0]['RFID'] == None and db_response[0]['barcode'] == current_barcode:
            uart.write(trigger_command_bytes)
            await feedback_alerts.play_confirmation(buzzer_pwm)

            feedback_alerts.lcd_display(lcd, "Scan RFID badge", "")
            led_green_pin.on()
            while True:
                reader.init()
                (stat, tag_type) = reader.request(reader.REQIDL)

                if stat == reader.OK:
                    (stat, uid) = reader.SelectTagSN()
                    if stat == reader.OK:
                        badge = str(int.from_bytes(bytes(uid),"little",False))

                        query = f'SELECT * FROM tickets WHERE RFID="{badge}";'
                        execute_query(query, True)
                        client.wait_msg()
                        if not bool(db_response):
                            
                            query = f'UPDATE tickets SET RFID="{badge}" WHERE barcode="{current_barcode}";'
                            execute_query(query, False)

                            await feedback_alerts.play_confirmation(buzzer_pwm)
                            feedback_alerts.lcd_display(lcd, "Synchronisatie", "Succesvol")

                            db_response = {}
                            led_green_pin.off()
                            # asyncio.run(lcd_succeed_cooldown())
                            break
                        else:
                            feedback_alerts.lcd_display(lcd, "Badge al", "gekoppeld")
                            await asyncio.gather(feedback_alerts.play_error(buzzer_pwm), feedback_alerts.error_blink(led_red_pin))

                if uart.any():
                    scanned_barcode = uart.read().decode()
                    scanned_barcode = ''.join(filter(str.isdigit, scanned_barcode)) #remove unwanted characters in the barcode
                    if scanned_barcode and scanned_barcode != db_response[0]['barcode'] and scanned_barcode != "31":
                        barcode_scanned(scanned_barcode)
                        led_green_pin.off()
                        return
        else:
            feedback_alerts.lcd_display(lcd, "Ticket al", "gescand")
            # asyncio.run(lcd_succeed_cooldown())
            await asyncio.gather(feedback_alerts.play_error(buzzer_pwm), feedback_alerts.error_blink(led_red_pin))
            uart.write(trigger_command_bytes)
    else:
        feedback_alerts.lcd_display(lcd, "Ticket niet", "gevonden")
        # asyncio.run(lcd_succeed_cooldown())
        await asyncio.gather(feedback_alerts.play_error(buzzer_pwm), feedback_alerts.error_blink(led_red_pin))
        uart.write(trigger_command_bytes)

connect_wifi()
client = connect_mqtt(callback)

def main():
    feedback_alerts.lcd_display(lcd, "Scan barcode...", "")
    uart.write(trigger_command_bytes)

    while True:
        global current_barcode
        if uart.any():
            scanned_barcode = uart.read().decode()
            scanned_barcode = ''.join(filter(str.isdigit, scanned_barcode)) #remove unwanted characters in the barcode
            if bool(scanned_barcode) and scanned_barcode != "31":
                barcode_scanned(scanned_barcode)       
                
if __name__ == "__main__": 
    main()