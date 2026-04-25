# Mixed Reality River Water Quality Handover Package

This repository is the public handover package for the mixed reality river water quality visualization project.

It is intended to help a reviewer, supervisor, or future developer understand three things quickly:

1. what the project does,
2. which files matter most,
3. how the Raspberry Pi input path connects to the Unity scene.

## What This Project Is

This project combines:

- a Raspberry Pi hardware controller,
- a UDP message bridge,
- a Unity mixed reality application,
- and a water quality exploration interface for river selection, filtering, and chart viewing.

The hardware side sends encoder, button, joystick, and slider input to Unity. Unity then converts those inputs into map style changes, river selection, menu navigation, parameter filtering, and chart updates.

## Quick Start For Reviewers

If you only need the most important files, start with these:

1. `FINAL_Document.docx`: final submission document
2. `raspberry-pi-scripts/pi_input_sender.py`: main Raspberry Pi runtime script
3. `unity/PiSystemBridge.cs`: Unity-side UDP receiver and control bridge
4. `unity/WaterSystemManager.cs`: Unity-side data and chart logic
5. `unity/mapswitch.cs`: Unity-side map layer switching logic

## Repository Contents

### Complete Unity Project

- `Assets/`: scenes, scripts, prefabs, materials, XR setup, and project assets
- `Packages/`: Unity package manifest and package configuration
- `ProjectSettings/`: Unity project settings required to reopen the project correctly

### Raspberry Pi Runtime And Debug Scripts

- `raspberry-pi-scripts/pi_input_sender.py`: final integrated runtime script used in normal operation
- `raspberry-pi-scripts/watch_gpio.py`: checks GPIO activity for encoder and button troubleshooting
- `raspberry-pi-scripts/mcp_read_all.py`: checks MCP3008 ADC channel readings
- `raspberry-pi-scripts/joy_click_test.py`: tests joystick click and movement behavior
- `raspberry-pi-scripts/udp_test_send.py`: tests whether Unity can receive UDP traffic

### GitHub-Friendly Unity Script Mirror

- `unity/`: lightweight copies of the most important Unity scripts for easier browser review

### Public Documentation

- `docs/README.md`: public documentation index
- `docs/script-purposes.md`: script responsibilities and message reference
- `docs/raspberry-pi-section-bilingual.md`: Raspberry Pi bilingual handover note
- `docs/joystick-wiring.md`: joystick wiring reference
- `docs/slider-wiring.md`: slider wiring reference
- `docs/encoder-wiring.md`: encoder wiring reference

## Main Runtime Flow

### Raspberry Pi Side

1. Physical inputs are read from GPIO and MCP3008.
2. `pi_input_sender.py` converts those inputs into messages such as `ENC`, `BTN`, `JOY`, and `SLIDER`.
3. Messages are sent over UDP to the Unity machine.

### Unity Side

1. `PiSystemBridge.cs` listens on UDP port `5005`.
2. Incoming messages are interpreted as map, river, menu, zoom, or filtering actions.
3. `WaterSystemManager.cs` updates the UI state and chart content.
4. `mapswitch.cs` updates the visible map layer.

## Where To Look In Unity

For a reviewer opening the Unity project, these are the main checkpoints:

1. Open the main scene, including `Assets/310.unity` if that is the active working scene.
2. Inspect the `PiSystemBridge` object and confirm it is listening on port `5005`.
3. Check that references for `WaterSystemManager`, map root, slider containers, and river objects are assigned.
4. Review `WaterSystemManager` to understand how class selection, data selection, dropdowns, and charts are connected.
5. Review `mapswitch.cs` to see how default, satellite, and terrain layers are switched.

## Current Confirmed Setup

- Raspberry Pi hostname: `raspberrypi`
- Raspberry Pi user: `xlab`
- Raspberry Pi IP seen during collection: `192.168.50.38`
- Unity listener port: `5005`
- Confirmed main Raspberry Pi script: `pi_input_sender.py`

## Notes On Scope

- The full Unity project is public in this repository under `Assets/`, `Packages/`, and `ProjectSettings/`.
- The `unity/` folder is kept because it is faster to browse than the full Unity project tree.
- Internal draft materials, large screenshot sets, and readability-focused split report drafts were archived outside the public repository.
- The Word generation script was intentionally removed from the public repository because the final `.docx` is the public deliverable.
