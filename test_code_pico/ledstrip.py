import time
from neopixel import Neopixel

numpix = 30
pixels = Neopixel(numpix, 0, 28, "GRB")

x = int(0)
y = int(0)
old_color = ""

orange = (255, 50, 0)
green = (0, 255, 0)
red = (255, 0, 0)
none = (0,0,0)

pixels.brightness(100)

# The 'down' and 'up' function animates a downward movement on an LED strip. It takes two pixel addresses and two colors as arguments.
# It lights up a line of pixels between the two addresses with the specified colors, then shifts the line down one pixel at a time.
def down(pixel_addres_1, pixel_addres_2, color_1, color_2):

    x = pixel_addres_1
    y = pixel_addres_2
    while (x != pixel_addres_1 - 10):
        pixels.set_pixel_line(0, 29, none)
        pixels.set_pixel_line(pixel_addres_2, x - 1, color_1)
        pixels.set_pixel_line(y, pixel_addres_2, color_2)
        time.sleep_ms(70)
        pixels.show()

        x -= 1
        y -= 1

def up(pixel_addres_1, pixel_addres_2, color_1, color_2):

    x = pixel_addres_1
    y = pixel_addres_2
    while (x != pixel_addres_1 + 10):
         pixels.set_pixel_line(0, 29, none)
         pixels.set_pixel_line(x , pixel_addres_2, color_1)
         pixels.set_pixel_line(pixel_addres_2 ,y , color_2)
         time.sleep_ms(70)
         pixels.show()

         x += 1
         y += 1

# The infinite while loop takes user input to change the LED strip color. 
#Depending on the input and the previous color, it calls the 'up' or 'down' function to animate the color transition.
while True:
    color = input("geef een getal 1(groen) 2(oranje) 3(rood):")
    if color == "1":
        if(old_color == "2"):
            down(19, 9, orange, green)        
        elif(old_color == "3"):
            down(30, 20,red, orange)
            down(19, 9, orange, green)  

    elif color == "2":
        if(old_color == "1"):
            up(1, 10, green, orange)
        elif(old_color == "3"):
            down(29, 19, red, orange)

    elif color == "3":
        if(old_color == "2"):
            up(11, 20, orange, red)
        elif(old_color == "1"):
            up(1, 10, green, orange)
            up(11, 20, orange, red)

    old_color = color
