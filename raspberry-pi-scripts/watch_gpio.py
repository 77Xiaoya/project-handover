import time
import RPi.GPIO as GPIO

GPIO.setmode(GPIO.BCM)

pins = [2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16, 17, 18, 19, 20, 21, 22, 23, 24, 25, 26, 27]

active = []
for p in pins:
    try:
        GPIO.setup(p, GPIO.IN, pull_up_down=GPIO.PUD_UP)
        active.append(p)
    except:
        pass

print("Watching BCM pins:", active)
last = {p: GPIO.input(p) for p in active}

try:
    while True:
        changed = []
        for p in active:
            v = GPIO.input(p)
            if v != last[p]:
                changed.append((p, last[p], v))
                last[p] = v

        if changed:
            for p, a, b in changed:
                print(f"BCM{p}: {a} -> {b}")
        time.sleep(0.01)

except KeyboardInterrupt:
    pass
finally:
    GPIO.cleanup()
