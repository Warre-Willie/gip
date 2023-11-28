import machine
import time

# Set up PWM on GPIO 21 with a frequency of 1000Hz and duty cycle of 50%
buzzer_pin = machine.Pin(18)
buzzer_pwm = machine.PWM(buzzer_pin)
buzzer_pwm.freq(1000)
buzzer_pwm.duty_u16(512)  # 50% duty cycle (adjust as needed)

# Function to play a confirmation sound
def play_confirmation_sound():
    buzzer_pwm.duty_u16(16383)  # 80% duty cycle (adjust as needed)
    time.sleep(0.1)
    buzzer_pwm.duty_u16(0)  # Turn off for a short pause
    time.sleep(0.1)
    buzzer_pwm.duty_u16(16383)
    time.sleep(0.1)
    buzzer_pwm.duty_u16(0)  # Turn off at the end

# Call the function to play the confirmation sound
play_confirmation_sound()
