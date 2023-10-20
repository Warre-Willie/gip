import time
from neopixel import Neopixel
 
numpix = 30
pixels = Neopixel(numpix, 0, 28, "GRB")
 
x = int(0)
y = int(0)

orange = (255, 50, 0)
green = (0, 255, 0)
red = (255, 0, 0)
none = (0,0,0)

pixels.brightness(50)



while False:
    x = 0
    user = input("geef een getal 1(groen) 2(oranje) 3(rood):")
    print(user)
    if user == "1":
        if(old_user == "2"):
            down(20,10)
        else:
            down(30,10)
            
    elif user == "2":
        if(old_user == "1"):
            up(10,20)
        else:
            down(30,20)
        
    else:
        if(old_user == "1"):
            up(10,30)
        else:
            up(20,30)
    user = old_user        


#def up(pixel_count1,pixel_count2):
    
    
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
        print(f"x: {x}   y:{y}")
        
down(30, 20,red, orange)

down(20, 10, orange, green)
    
    

