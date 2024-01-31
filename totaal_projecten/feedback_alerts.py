import uasyncio as asyncio

async def play_confirmation(buzzer_pwm):
    buzzer_pwm.freq(1200)
    buzzer_pwm.duty_u16(16383)
    await asyncio.sleep(0.1)
    buzzer_pwm.duty_u16(0)

    buzzer_pwm.freq(1300)
    buzzer_pwm.duty_u16(16383)
    await asyncio.sleep(0.1)
    buzzer_pwm.duty_u16(0)

    buzzer_pwm.freq(1400)
    buzzer_pwm.duty_u16(16383)
    await asyncio.sleep(0.1)
    buzzer_pwm.duty_u16(0)
 
async def play_error(buzzer_pwm):
    buzzer_pwm.freq(300)
    buzzer_pwm.duty_u16(16383)
    await asyncio.sleep_ms(600)
    buzzer_pwm.duty_u16(0)

async def error_blink(led):
    for i in range(7):
        led.on()
        await asyncio.sleep(0.1)
        led.off()
        await asyncio.sleep(0.07)

def lcd_display(lcd, line_1, line_2):
    lcd.clear()
    lcd.move_to(0, 0)
    lcd.putstr(line_1)
    lcd.move_to(0, 1)
    lcd.putstr(line_2)
