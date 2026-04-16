# Raspberry Pi to Unity Handover

This folder is a GitHub-ready handover package for the Raspberry Pi to Unity integration.

## What To Keep

- `raspberry-pi-scripts/pi_input_sender.py`: final integrated runtime script
- `raspberry-pi-scripts/watch_gpio.py`: GPIO mapping debug tool
- `raspberry-pi-scripts/mcp_read_all.py`: ADC channel mapping debug tool
- `raspberry-pi-scripts/joy_click_test.py`: joystick and click debug tool
- `raspberry-pi-scripts/udp_test_send.py`: UDP path debug tool
- `unity/PiSystemBridge.cs`: Unity UDP receiver and input handler

## Recommended User Flow

1. Update the target IP in `pi_input_sender.py`.
2. Start `pi_input_sender.py` on the Raspberry Pi.
3. Open the Unity project and scene containing `PiSystemBridge`.
4. Confirm Unity is listening on UDP port `5005`.
5. Test encoder, joystick, slider, and joystick click input.

## Debug Flow

1. If buttons or encoders do not react, run `watch_gpio.py`.
2. If slider or joystick axes are unclear, run `mcp_read_all.py`.
3. If joystick click is unclear, run `joy_click_test.py`.
4. If Unity receives nothing, run `udp_test_send.py` to test network delivery.

## Current Confirmed Setup

- Raspberry Pi hostname: `raspberrypi`
- Raspberry Pi user: `xlab`
- Raspberry Pi IP seen during collection: `192.168.50.38`
- Unity listener port: `5005`
- Confirmed main script: `pi_input_sender.py`

## Key Hardware Notes

- Detailed joystick wiring and explanation: `docs/joystick-wiring.md`
- Detailed slider wiring notes: `docs/slider-wiring.md`
- Detailed encoder wiring notes: `docs/encoder-wiring.md`
- Raspberry Pi bilingual document draft: `docs/raspberry-pi-section-bilingual.md`
- Full revised bilingual technical document: `docs/TECHNICAL_DOCUMENTATION_REVISED_BILINGUAL.md`

See `docs/script-purposes.md` for detailed explanations.
