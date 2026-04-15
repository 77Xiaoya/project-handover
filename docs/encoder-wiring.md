# Encoder Wiring

This file records the currently verified encoder wiring.

## Encoder 1

| Encoder 1 pin | Destination | Function | Verification |
| --- | --- | --- | --- |
| `VCC` | `5V` positive rail, breadboard row `1` | encoder power | required for stable reading |
| `GND` | `5V` negative rail, breadboard row `2` | ground reference | required for stable reading |
| `KEY` | Raspberry Pi `GPIO27` | encoder push button | matches `ENC1_KEY = 27` in `pi_input_sender.py` |
| `S2` | Raspberry Pi `GPIO22` | encoder quadrature phase B | matches `ENC1_S2 = 22` in `pi_input_sender.py` |
| `S1` | Raspberry Pi `GPIO17` | encoder quadrature phase A | matches `ENC1_S1 = 17` in `pi_input_sender.py` |

## Encoder 2

| Encoder 2 pin | Destination | Function | Verification |
| --- | --- | --- | --- |
| `VCC` | `5V` positive rail, breadboard row `13` | encoder power | recorded from lab wiring note |
| `GND` | `5V` negative rail, breadboard row `10` | ground reference | recorded from lab wiring note |
| `KEY` | Raspberry Pi `GPIO25` | encoder push button | matches `ENC2_KEY = 25` in `pi_input_sender.py` |
| `S2` | Raspberry Pi `GPIO23` | encoder quadrature phase B | matches `ENC2_S2 = 23` in `pi_input_sender.py` |
| `S1` | Raspberry Pi `GPIO18` | encoder quadrature phase A | matches `ENC2_S1 = 18` in `pi_input_sender.py` |

## Encoder 3

| Encoder 3 pin | Destination | Function | Verification |
| --- | --- | --- | --- |
| `VCC` | `5V` positive rail, breadboard row `22` | encoder power | recorded from lab wiring note |
| `GND` | `5V` negative rail, breadboard row `19`; fourth section first position in lab note | ground reference | recorded from lab wiring note |
| `KEY` | Raspberry Pi `GPIO20` | encoder push button | matches `ENC3_KEY = 20` in `pi_input_sender.py` |
| `S2` | Raspberry Pi `GPIO16` | encoder quadrature phase B | matches `ENC3_S2 = 16` in `pi_input_sender.py` |
| `S1` | Raspberry Pi `GPIO24` | encoder quadrature phase A | matches `ENC3_S1 = 24` in `pi_input_sender.py` |

## Working Principle

- `S1` and `S2` are the two quadrature outputs used to determine rotation direction.
- The Raspberry Pi script watches the `S1` edge and reads `S2` to decide whether to send a positive or negative step.
- `KEY` is the built-in push button of the encoder.

In the current main script:

- `ENC1_S1 = 17`
- `ENC1_S2 = 22`
- `ENC1_KEY = 27`
- `ENC2_S1 = 18`
- `ENC2_S2 = 23`
- `ENC2_KEY = 25`
- `ENC3_S1 = 24`
- `ENC3_S2 = 16`
- `ENC3_KEY = 20`

## Software Behavior

`pi_input_sender.py` sends:

- `ENC1:1` or `ENC1:-1` when the encoder rotates
- `BTN1` when the encoder button is pressed
- `ENC2:1` or `ENC2:-1` when encoder 2 rotates
- `BTN2` when encoder 2 is pressed
- `ENC3:1` or `ENC3:-1` when encoder 3 rotates
- `BTN3` when encoder 3 is pressed

## Notes

- Encoder 1 rail notes were corrected from the earlier temporary version. The row `1` and row `2` values should be treated as the final recorded version.
- Encoder 2 and encoder 3 power rail rows were added from direct lab notes and should be rechecked only if a full hardware redraw is later required.
