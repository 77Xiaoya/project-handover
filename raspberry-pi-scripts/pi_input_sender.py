import socket
import time
import threading
import spidev

from gpiozero import Button, DigitalInputDevice
from signal import pause

# =========================
# Unity network settings
# =========================
UNITY_IP = "192.168.50.204"
UNITY_PORT = 5005

sock = socket.socket(socket.AF_INET, socket.SOCK_DGRAM)


def send(msg):
    sock.sendto(msg.encode("utf-8"), (UNITY_IP, UNITY_PORT))
    print(msg)


# =========================
# GPIO mapping
# Encoder 1: GPIO17 / GPIO22, click GPIO27
# Encoder 2: GPIO18 / GPIO23, click GPIO25
# Encoder 3: GPIO24 / GPIO16, click GPIO20
# Joystick click: GPIO26
# =========================
ENC1_S1 = 17
ENC1_S2 = 22
ENC1_KEY = 27

ENC2_S1 = 18
ENC2_S2 = 23
ENC2_KEY = 25

ENC3_S1 = 24
ENC3_S2 = 16
ENC3_KEY = 20

JOY_KEY = 26


# =========================
# Buttons
# =========================
btn1 = Button(ENC1_KEY, pull_up=True, bounce_time=0.05)
btn2 = Button(ENC2_KEY, pull_up=True, bounce_time=0.05)
btn3 = Button(ENC3_KEY, pull_up=True, bounce_time=0.05)
joy_btn = Button(JOY_KEY, pull_up=True, bounce_time=0.08)

btn1.when_pressed = lambda: send("BTN1")
btn2.when_pressed = lambda: send("BTN2")
btn3.when_pressed = lambda: send("BTN3")
joy_btn.when_pressed = lambda: send("JOYBTN")


# =========================
# Encoders
# Listen on S1 edge and use S2 to determine direction
# =========================
enc1_a = DigitalInputDevice(ENC1_S1, pull_up=True)
enc1_b = DigitalInputDevice(ENC1_S2, pull_up=True)

enc2_a = DigitalInputDevice(ENC2_S1, pull_up=True)
enc2_b = DigitalInputDevice(ENC2_S2, pull_up=True)

enc3_a = DigitalInputDevice(ENC3_S1, pull_up=True)
enc3_b = DigitalInputDevice(ENC3_S2, pull_up=True)

last1 = enc1_a.value
last2 = enc2_a.value
last3 = enc3_a.value


def encoder_loop():
    global last1, last2, last3

    while True:
        cur1 = enc1_a.value
        if cur1 != last1 and cur1 == 0:
            send("ENC1:1" if enc1_b.value == 1 else "ENC1:-1")
        last1 = cur1

        cur2 = enc2_a.value
        if cur2 != last2 and cur2 == 0:
            send("ENC2:1" if enc2_b.value == 1 else "ENC2:-1")
        last2 = cur2

        cur3 = enc3_a.value
        if cur3 != last3 and cur3 == 0:
            send("ENC3:1" if enc3_b.value == 1 else "ENC3:-1")
        last3 = cur3

        time.sleep(0.001)


# =========================
# MCP3008 ADC
# slider: ADC1 / ADC3
# joystick XY: ADC5 / ADC6
# =========================
spi = spidev.SpiDev()
spi.open(0, 0)
spi.max_speed_hz = 1350000


def read_adc(channel):
    if channel < 0 or channel > 7:
        return 0
    r = spi.xfer2([1, (8 + channel) << 4, 0])
    return ((r[1] & 3) << 8) + r[2]


JOY_LOW = 300
JOY_HIGH = 700
JOY_CENTER_LOW = 430
JOY_CENTER_HIGH = 590

joy_x_locked = False
joy_y_locked = False


def joystick_loop():
    global joy_x_locked, joy_y_locked

    while True:
        x = read_adc(5)
        y = read_adc(6)

        if not joy_x_locked:
            if x < JOY_LOW:
                send("JOY:LEFT")
                joy_x_locked = True
            elif x > JOY_HIGH:
                send("JOY:RIGHT")
                joy_x_locked = True
        else:
            if JOY_CENTER_LOW <= x <= JOY_CENTER_HIGH:
                joy_x_locked = False

        if not joy_y_locked:
            if y < JOY_LOW:
                send("JOY:DOWN")
                joy_y_locked = True
            elif y > JOY_HIGH:
                send("JOY:UP")
                joy_y_locked = True
        else:
            if JOY_CENTER_LOW <= y <= JOY_CENTER_HIGH:
                joy_y_locked = False

        time.sleep(0.03)


last_slider1 = -1
last_slider2 = -1
SLIDER_THRESHOLD = 12


def slider_loop():
    global last_slider1, last_slider2

    while True:
        slider1 = read_adc(1)
        slider2 = read_adc(3)

        if abs(slider1 - last_slider1) >= SLIDER_THRESHOLD:
            send(f"SLIDER1:{slider1}")
            last_slider1 = slider1

        if abs(slider2 - last_slider2) >= SLIDER_THRESHOLD:
            send(f"SLIDER2:{slider2}")
            last_slider2 = slider2

        time.sleep(0.05)


threading.Thread(target=encoder_loop, daemon=True).start()
threading.Thread(target=joystick_loop, daemon=True).start()
threading.Thread(target=slider_loop, daemon=True).start()

print("Pi input sender started.")
pause()
