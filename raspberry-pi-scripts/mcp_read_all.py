import spidev
import time

spi = spidev.SpiDev()
spi.open(0, 0)
spi.max_speed_hz = 1350000


def read_ch(ch):
    adc = spi.xfer2([1, (8 + ch) << 4, 0])
    return ((adc[1] & 3) << 8) + adc[2]


try:
    while True:
        vals = [read_ch(i) for i in range(8)]
        print(" ".join([f"CH{i}:{vals[i]:4d}" for i in range(8)]))
        time.sleep(0.2)
except KeyboardInterrupt:
    pass
finally:
    spi.close()
