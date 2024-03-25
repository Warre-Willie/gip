from mfrc522 import MFRC522
from machine import Pin, I2C, PWM
from pico_i2c_lcd import I2cLcd
import urandom, ubinascii, json, time
import uasyncio as asyncio
from network_setup import connect_wifi, connect_mqtt
import feedback_alerts

# set zone id
with open('toegang_config.json', 'r') as f:
    config = json.load(f)

zone_id = config['zone_id']
zone_name = ""

lcd_start_time = time.time()

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
    feedback_alerts.lcd_display(lcd, "", "Laden...")

    mqtt_msg_dict = {}
    uuid = gen_uuid()
    mqtt_msg_dict['UUID'] = uuid
    mqtt_msg_dict['returnData'] = returnData
    mqtt_msg_dict['query'] = query
    json_string = json.dumps(mqtt_msg_dict)
    if returnData:
        client.subscribe(uuid)
    client.publish("gip/queries", json_string)


# This function generates a unique UUID (Universally Unique Identifier).
# It does this by generating random bytes, converting them to a hexadecimal string, and concatenating them together.
# The resulting UUID consists of 4 groups of hexadecimal strings, separated by hyphens.
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

# The main function is defined as an asynchronous function.
async def main():
    global lcd_start_time
    while True:
        if lcd_start_time is not None and lcd_start_time + 2 < time.time():
            feedback_alerts.lcd_display(lcd, zone_name, "Scan RFID badge")
            lcd_start_time = None
        reader.init()
        (stat, tag_type) = reader.request(reader.REQIDL)
        if stat == reader.OK:
            (stat, uid) = reader.SelectTagSN()
            if stat == reader.OK:
                badge = int.from_bytes(bytes(uid),"little",False)
                execute_query(f"""SELECT t.id, t.RFID, brt.badge_right_id, brt.ticket_id, br.id, brz.badge_right_id, brz.zone_id 
                              FROM tickets t 
                              JOIN badge_rights_tickets brt ON brt.ticket_id = t.id 
                              JOIN badge_rights br ON brt.badge_right_id = br.id 
                              JOIN badge_rights_zones brz ON brz.badge_right_id = br.id 
                              WHERE t.RFID = '{badge}'""", True)
                client.wait_msg()
                if db_response:
                    # led_green_pin.on()
                    # led_red_pin.off()
                    feedback_alerts.lcd_display(lcd, zone_name, "Verleend")
                    await feedback_alerts.play_confirmation(buzzer_pwm)
                else:
                    # led_green_pin.off()
                    feedback_alerts.lcd_display(lcd, zone_name, "Geweigerd")
                    await asyncio.gather(feedback_alerts.play_error(buzzer_pwm), feedback_alerts.error_blink(led_red_pin))
                lcd_start_time = time.time()

if __name__ == "__main__": 
    execute_query(f"SELECT name FROM zones WHERE id={zone_ID}", True)
    client.wait_msg()
    if db_response:
        zone_name = db_response[0]['name']
        feedback_alerts.lcd_display(lcd, zone_name, "Scan RFID badge")
        db_response = {}
        asyncio.run(main())
    else:
        feedback_alerts.lcd_display(lcd, "Ongeldige zone", "")

