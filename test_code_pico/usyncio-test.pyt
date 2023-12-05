import uasyncio
from machine import Pin, PWM

buzzer_pin = Pin(18)
buzzer_pwm = PWM(buzzer_pin)

async def play_error_sound():
    buzzer_pwm.freq(300)
    buzzer_pwm.duty_u16(16383)
    await uasyncio.sleep(3)
    buzzer_pwm.duty_u16(0)

async def test():
    loop.create_task(play_error_sound())
    


async def main():
    # Start the asynchronous function in the background
    # loop.create_task(test())
    await test()

    # Continue with the main program and print a simple string
    while True:
        print("Main program working...")
        await uasyncio.sleep(0.5)
    
    # Wait for the asynchronous function to complete
    # await task

# Run the event loop
loop = uasyncio.get_event_loop()
loop.run_until_complete(main())
