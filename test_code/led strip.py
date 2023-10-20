import time
from neopixel import Neopixel
 
numpix = 30
pixels = Neopixel(numpix, 0, 28, "GRB")
 
x = int(0)

orange = (255, 50, 0)
green = (0, 255, 0)
red = (255, 0, 0)

pixels.brightness(50)

while True:
    x = 0
    user = input("geef een getal 1(groen) 2(oranje) 3(rood):")
    print(user)
    if user == "1":
        while (x <= 29):
            pixels.set_pixel_line(0, x, green)
            pixels.show()
            x += 1
            time.sleep_ms(50)
            
    elif user == "2":
        while (x <= 29):
            pixels.set_pixel_line(0, x, orange)
            x += 1
            time.sleep_ms(50)
            pixels.show()
        
    else:
        while (x <= 29):
            pixels.set_pixel_line(0, x, red)
            x += 1
            time.sleep_ms(100)
            pixels.show()
