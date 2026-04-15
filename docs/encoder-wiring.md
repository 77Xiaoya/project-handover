# Encoder Wiring

This file records the currently verified encoder wiring.

## Encoder 1

| Encoder 1 pin | Destination | Function | Verification |
| --- | --- | --- | --- |
| `VCC` | `5V` positive rail, breadboard row `3` | encoder power | required for stable reading |
| `GND` | `5V` negative rail, breadboard row `4` | ground reference | required for stable reading |
| `KEY` | Raspberry Pi `GPIO27` | encoder push button | matches `ENC1_KEY = 27` in `pi_input_sender.py` |
| `S2` | Raspberry Pi `GPIO22` | encoder quadrature phase B | matches `ENC1_S2 = 22` in `pi_input_sender.py` |
| `S1` | Raspberry Pi `GPIO17` | encoder quadrature phase A | matches `ENC1_S1 = 17` in `pi_input_sender.py` |

## Working Principle

- `S1` and `S2` are the two quadrature outputs used to determine rotation direction.
- The Raspberry Pi script watches the `S1` edge and reads `S2` to decide whether to send a positive or negative step.
- `KEY` is the built-in push button of the encoder.

In the current main script:

- `ENC1_S1 = 17`
- `ENC1_S2 = 22`
- `ENC1_KEY = 27`

## Software Behavior

`pi_input_sender.py` sends:

- `ENC1:1` or `ENC1:-1` when the encoder rotates
- `BTN1` when the encoder button is pressed

## Notes

- Encoder 1 rail notes were rechecked in the lab and should be treated as the corrected version.
- Additional encoder wiring can be added to this file once fully verified.
