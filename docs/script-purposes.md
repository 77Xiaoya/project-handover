# Script Purposes

## Final Runtime Script

| Script | Purpose | When To Use | Notes |
| --- | --- | --- | --- |
| `pi_input_sender.py` | Final integrated Raspberry Pi sender | Normal project operation | Sends encoder, joystick, slider, and button input to Unity over UDP |

## Debug Scripts

| Script | Purpose | When To Use | Expected Output |
| --- | --- | --- | --- |
| `watch_gpio.py` | Shows changing GPIO pins | Identify which button or encoder pin is active | Prints `BCMxx: old -> new` |
| `mcp_read_all.py` | Shows all MCP3008 ADC channels | Identify slider and joystick channel mapping | Prints `CH0` to `CH7` values |
| `joy_click_test.py` | Tests joystick axes and click button | Verify joystick movement and click counter | Sends JSON with `ch5`, `ch6`, and `click24Count` |
| `udp_test_send.py` | Tests UDP delivery only | Check if Unity can receive network packets | Sends simple test messages to port `5005` |

## Unity Script

| Script | Purpose | Notes |
| --- | --- | --- |
| `PiSystemBridge.cs` | Receives UDP messages in Unity and maps them to menu, slider, zoom, and river selection behavior | Current receiver listens on port `5005` |

## Main Raspberry Pi Message Types

`pi_input_sender.py` sends these message formats:

- `ENC1:1` or `ENC1:-1`
- `ENC2:1` or `ENC2:-1`
- `ENC3:1` or `ENC3:-1`
- `JOY:LEFT`
- `JOY:RIGHT`
- `JOY:UP`
- `JOY:DOWN`
- `JOYBTN`
- `SLIDER1:<value>`
- `SLIDER2:<value>`

## Current Hardware Mapping From Main Script

| Input | Mapping |
| --- | --- |
| Encoder 1 A/B | GPIO17 / GPIO22 |
| Encoder 1 click | GPIO27 |
| Encoder 2 A/B | GPIO18 / GPIO23 |
| Encoder 2 click | GPIO25 |
| Encoder 3 A/B | GPIO24 / GPIO16 |
| Encoder 3 click | GPIO20 |
| Joystick click | GPIO26 |
| Slider 1 | ADC channel 1 |
| Slider 2 | ADC channel 3 |
| Joystick X | ADC channel 5 |
| Joystick Y | ADC channel 6 |

## Important Configuration To Check Before Demo

- Update the target Unity PC IP in Raspberry Pi scripts.
- Confirm Unity is listening on port `5005`.
- Confirm the scene contains `PiSystemBridge`.
- Confirm the Raspberry Pi and Unity PC are on the same network.

## Detailed Wiring Notes

- `joystick-wiring.md`: detailed joystick pin, breadboard, ADC, and GPIO notes
- `slider-wiring.md`: detailed horizontal and vertical slider wiring notes
- `encoder-wiring.md`: detailed encoder pin, power, and GPIO notes
- `raspberry-pi-section-bilingual.md`: bilingual Raspberry Pi documentation draft for report writing
- `TECHNICAL_DOCUMENTATION_REVISED_BILINGUAL.md`: full revised bilingual technical document aligned to meeting comments
