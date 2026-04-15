import socket
import time

QUEST_IP = "192.168.50.132"
PORT = 5005

sock = socket.socket(socket.AF_INET, socket.SOCK_DGRAM)


def send(msg: str):
    sock.sendto(msg.encode("utf-8"), (QUEST_IP, PORT))
    print("sent", msg)


for i in [0, 1, 2, 0, 2, 1]:
    send(f"IDX:{i}")
    time.sleep(1.0)

for s in range(0, 7):
    send(f"SEL:{s}")
    time.sleep(0.6)

for v in [0.0, 0.25, 0.5, 0.75, 1.0, 0.5, 0.0]:
    send(f"VAL:{v:.3f}")
    time.sleep(0.6)

print("done")
