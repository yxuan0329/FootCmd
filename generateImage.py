# -*- coding: utf-8 -*-
from PIL import Image, ImageDraw, ImageFont

grey = (210, 210, 210)
def draw_circles(file_path):
    input_file_path = file_path + "/strokeList.txt"
    with open(input_file_path, 'r') as f:
        lines = f.readlines()
    font_size = 22
    font = ImageFont.truetype("arial.ttf", font_size)

    # define initial position
    lower = 50
    upper = 350
    areas = {
        1: (lower, lower),
        2: (lower, upper),
        3: (upper, lower),
        4: (upper, upper),
    }

    offset = 50
    
    cnt = 0
    for line in lines:
        image = Image.new("RGB", (450, 400), "white")
        draw = ImageDraw.Draw(image)
        
        # square 
        square_position = (lower, lower, upper, upper)
        draw.rectangle(square_position, outline=grey, width=3)

        # draw four circles at the four corners
        for i in range(1, 5):
            x, y = areas[i]
            draw.ellipse([x-25, y-25, x+25, y+25], outline=grey, fill="white", width=4)

        hasIcon = [int(0)] * 5
        
        data = line.strip()
        num = 0
        for c in data:
            i = get_number(c)
            x, y = areas[i]
            
            # draw grey circle
            draw.ellipse([x-25 + hasIcon[i] * offset, y-25, x+25 + hasIcon[i] * offset, y+25], outline="black", fill="white", width=3)
            
            # check order
            if ord(c) <= 55:
                num += 1
            
            # label the number in the circle
            text_width, text_height = draw.textsize(str(num), font)
            draw.text(((x + x - text_width) // 2  + hasIcon[i] * offset, (y + y - text_height) // 2), str(num), fill="black", font=font)
            hasIcon[i] += 1

        # image.save(f"D:/_xuan/strokes/img_{data}.png")
        image.save(file_path + f"/strokes/{data}.png")
        cnt += 1
    print(f"{cnt} pictures created.")

def get_number(c):
    if ord(c) <= 55:
        return int(ord(c) - 48)
    else:
        return ord(c) - ord('a') + 1

if __name__ == "__main__":
    file_path = "D:/_xuan/UserStudy1"
    draw_circles(file_path)