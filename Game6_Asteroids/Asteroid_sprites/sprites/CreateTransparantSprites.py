import os
import cv2


pathToSprites = 'asteroid_sprites'
pathToSave = 'asteroid_sprites_png'


if not os.path.exists(pathToSave): os.mkdir(pathToSave)
allImgPath = [pathToSprites + f"/{filename}" for filename in os.listdir(pathToSprites)]

for imgPath in allImgPath:
    spriteName = imgPath.split("/")[-1]

    img = cv2.imread(imgPath)
    color_top = img[0, 0]

    image = cv2.cvtColor(img, cv2.COLOR_BGR2BGRA)   # adds a fourth layer, for png
    for i in range(image.shape[0]):
        for j in range(image.shape[1]):
            if all(img[i, j] == color_top):
                image[i, j] = [image[i, j][0], image[i, j][1], image[i, j][2], 0]

    cv2.imwrite(pathToSave + f"/{spriteName[:-4]}.png", image)
