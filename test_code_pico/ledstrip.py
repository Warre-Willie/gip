import time
from neopixel import Neopixel
 
numpix = 30
pixels = Neopixel(numpix, 0, 28, "GRB")
 
x = int(0)
y = int(0)
old_user = ""

orange = (255, 50, 0)
green = (0, 255, 0)
red = (255, 0, 0)
none = (0,0,0)

pixels.brightness(100)


def down(pixel_count1, pixel_count2,color1, color2):

    x = pixel_count1
    y = pixel_count2
    while (x != pixel_count1-10):
        pixels.set_pixel_line(0, 29, none)
        pixels.set_pixel_line(pixel_count2, x-1, color1)
        pixels.set_pixel_line(y, pixel_count2, color2)
        time.sleep_ms(70)
        pixels.show()

        x -= 1
        y -= 1

def up(pixel_count1, pixel_count2,color1, color2):

    x = pixel_count1
    y = pixel_count2
    while (x != pixel_count1+10):
         pixels.set_pixel_line(0, 29, none)
         pixels.set_pixel_line(x , pixel_count2, color1)
         pixels.set_pixel_line(pixel_count2  , y  , color2)
         time.sleep_ms(70)
         pixels.show()

         x += 1
         y += 1
        
while True:
    x = 0
    user = input("geef een getal 1(groen) 2(oranje) 3(rood):")
    
    match user:
        case "1":
            if(old_user == "2"):
                down(19, 9, orange, green)
            elif(old_user == "1"): 
                pixels.set_pixel_line(0,9 , green)
                pixels.show()            
            else:
                down(30, 20,red, orange)
                down(19, 9, orange, green)
        case "2":
            if(old_user == "2"):
                pixels.set_pixel_line(10,19 , orange)
                pixels.show()
            elif(old_user == "1"):
                up(1,10,green,orange)
            else:
                down(29, 19, red, orange)
        case "3":
            if(old_user == "2"):
                up(11,20 ,orange, red)
            elif(old_user == "1"):
                up(1,10,green,orange)
                up(11,20 ,orange, red)
            else:
                pixels.set_pixel_line(20,29 , red)
                pixels.show()
                
         case _:
             input("Wrong input try again. press enter to continue.")
             
    old_user = user 

