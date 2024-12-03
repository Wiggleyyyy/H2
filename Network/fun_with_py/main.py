import qrcode

img = qrcode.make("du er så gay det er crazy")
type(img)
img.save("./test_1.png")    