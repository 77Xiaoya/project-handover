# Script Purposes

## Raspberry Pi Runtime Script

| Script | Purpose | When To Use | Notes |
| --- | --- | --- | --- |
| `pi_input_sender.py` | Final integrated Raspberry Pi sender | Normal project operation | Sends encoder, joystick, slider, and button input to Unity over UDP |

## Raspberry Pi Debug Scripts

| Script | Purpose | When To Use | Expected Output |
| --- | --- | --- | --- |
| `watch_gpio.py` | Shows changing GPIO pins | Identify active button or encoder pins | Prints GPIO pin state changes |
| `mcp_read_all.py` | Shows MCP3008 ADC channels | Identify slider and joystick channel mapping | Prints channel values |
| `joy_click_test.py` | Tests joystick axes and click button | Verify joystick movement and click counter | Prints joystick and click test values |
| `udp_test_send.py` | Tests UDP delivery only | Check if Unity can receive network packets | Sends simple test messages to port `5005` |

## Unity Runtime Scripts

| Script | Purpose | Notes |
| --- | --- | --- |
| `PiSystemBridge.cs` | Receives UDP messages in Unity and maps them to menu, slider, zoom, and river selection behavior | Current receiver listens on port `5005` |
| `WaterSystemManager.cs` | Manages river state, filtering UI, parameter selection, and chart updates | Main data-side controller after Unity receives input |
| `mapswitch.cs` | Switches between default, satellite, and terrain map layers | Used by map style controls |
| `IntersectionRiverDetector.cs` | Supports river intersection and hover or selection helper logic | Scene interaction support |
| `waterQualityP.cs` | Supports water-quality presentation behavior | Scene-specific helper logic |

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
- `BTN1`
- `BTN2`
- `BTN3`
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

- Update the target Unity PC IP in Raspberry Pi scripts if the lab computer changes.
- Confirm Unity is listening on port `5005`.
- Confirm the scene contains `PiSystemBridge`.
- Confirm the Raspberry Pi and Unity PC are on the same network.
- Confirm Inspector references are assigned for `PiSystemBridge`, `WaterSystemManager`, and map layers.
