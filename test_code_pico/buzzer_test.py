import machine
import time

# Set up PWM on GPIO 21 with a frequency of 1000Hz and duty cycle of 50%
buzzer_pin = machine.Pin(18)
buzzer_pwm = machine.PWM(buzzer_pin)

# Function to play a confirmation sound
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

def play_error_sound():
    buzzer_pwm.freq(300)
    buzzer_pwm.duty_u16(16383)
    time.sleep(0.6)
    buzzer_pwm.duty_u16(0)
   
# Call the function to play the confirmation sound
play_error_sound()

