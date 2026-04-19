# Mixed Reality River Water Quality Handover Package

This repository is the public handover package for the mixed reality river water quality visualization project.

It now includes the complete Unity project files, the Raspberry Pi runtime scripts, a small set of public-facing handover notes, and the final Word submission.

## Public Repository Contents

- `Assets/`: complete Unity project assets, scenes, scripts, prefabs, materials, and package content used by the project
- `Packages/`: Unity package manifest and package configuration
- `ProjectSettings/`: Unity project settings required to reopen the project correctly
- `raspberry-pi-scripts/`: Raspberry Pi runtime and debug scripts used for controller input and UDP delivery
- `unity/`: reviewer-friendly copies of the most important Unity scripts
- `docs/`: concise public handover notes and wiring references
- `FINAL_Document.docx`: final submission document

## Main Technical Entry Points

### Raspberry Pi Side

- `raspberry-pi-scripts/pi_input_sender.py`: final integrated Raspberry Pi runtime script
- `raspberry-pi-scripts/watch_gpio.py`: GPIO mapping debug tool
- `raspberry-pi-scripts/mcp_read_all.py`: ADC channel mapping debug tool
- `raspberry-pi-scripts/joy_click_test.py`: joystick and click debug tool
- `raspberry-pi-scripts/udp_test_send.py`: UDP path debug tool

### Unity Side

- `unity/PiSystemBridge.cs`: Unity UDP receiver and Raspberry Pi input bridge
- `unity/WaterSystemManager.cs`: Unity-side data, filtering, and chart controller
- `unity/mapswitch.cs`: Unity map layer switching logic
- `Assets/310.unity`: main Unity scene used by the project

## Main Runtime Flow

1. Update the target Unity IP in `raspberry-pi-scripts/pi_input_sender.py` if needed.
2. Start `pi_input_sender.py` on Raspberry Pi.
3. Open the Unity project and load the main scene.
4. Confirm `PiSystemBridge` is listening on UDP port `5005`.
5. Use joystick, sliders, encoders, and buttons to drive map style, river selection, menu focus, filtering, and chart interaction.

## Public Documentation

- `docs/README.md`: public documentation index
- `docs/script-purposes.md`: Raspberry Pi and Unity script reference
- `docs/raspberry-pi-section-bilingual.md`: Raspberry Pi bilingual handover note
- `docs/joystick-wiring.md`: joystick wiring reference
- `docs/slider-wiring.md`: slider wiring reference
- `docs/encoder-wiring.md`: encoder wiring reference

## Notes On Scope

- The full Unity project is now public in this repository under `Assets/`, `Packages/`, and `ProjectSettings/`.
- The `unity/` folder is kept as a lighter script mirror so reviewers can inspect the main scripts quickly.
- Internal draft materials, large screenshot sets, and readability-focused split report drafts were archived outside the public repository.
- The Word generation script was intentionally removed from the public repository because the final `.docx` is the public deliverable.

## Current Confirmed Setup

- Raspberry Pi hostname: `raspberrypi`
- Raspberry Pi user: `xlab`
- Raspberry Pi IP seen during collection: `192.168.50.38`
- Unity listener port: `5005`
- Confirmed main Raspberry Pi script: `pi_input_sender.py`
