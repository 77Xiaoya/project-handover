import time
import json
import socket
import RPi.GPIO as GPIO
import spidev

spi = spidev.SpiDev()
spi.open(0, 0)
spi.max_speed_hz = 1350000


def read_adc(ch: int) -> int:
    if ch < 0 or ch > 7:
        return 0
    r = spi.xfer2([1, (8 + ch) << 4, 0])
    return ((r[1] & 3) << 8) + r[2]


UDP_IP = "192.168.50.204"
UDP_PORT = 5005
sock = socket.socket(socket.AF_INET, socket.SOCK_DGRAM)

PIN_CLICK = 24
GPIO.setmode(GPIO.BCM)
GPIO.setup(PIN_CLICK, GPIO.IN, pull_up_down=GPIO.PUD_UP)

click24Count = 0


def send_snapshot(ch5, ch6):
    msg = json.dumps({
        "ch5": ch5,
        "ch6": ch6,
        "click24Count": click24Count
    })
    sock.sendto(msg.encode(), (UDP_IP, UDP_PORT))
    print("SEND:", msg)


def on_click(channel):
    global click24Count
    click24Count += 1
    ch5 = read_adc(5)
    ch6 = read_adc(6)
    send_snapshot(ch5, ch6)


GPIO.add_event_detect(PIN_CLICK, GPIO.FALLING, callback=on_click, bouncetime=220)

print("Joystick+Click TEST running...")
print("Move joystick (ch5/ch6), press key on GPIO24->GND to increment click24Count")

last5, last6 = -1, -1
last_send = 0

try:
    while True:
        ch5 = read_adc(5)
        ch6 = read_adc(6)

        if abs(ch5 - last5) > 10 or abs(ch6 - last6) > 10 or (time.time() - last_send) > 0.2:
            last5, last6 = ch5, ch6
            last_send = time.time()
            send_snapshot(ch5, ch6)

        time.sleep(0.02)

except KeyboardInterrupt:
    GPIO.cleanup()
