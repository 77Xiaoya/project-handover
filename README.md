# Mixed Reality River Water Quality Handover Package

This repository is a GitHub-ready handover package for the mixed reality river water quality visualization project.

## Core Technical Files

- `raspberry-pi-scripts/pi_input_sender.py`: final integrated Raspberry Pi runtime script
- `raspberry-pi-scripts/watch_gpio.py`: GPIO mapping debug tool
- `raspberry-pi-scripts/mcp_read_all.py`: ADC channel mapping debug tool
- `raspberry-pi-scripts/joy_click_test.py`: joystick and click debug tool
- `raspberry-pi-scripts/udp_test_send.py`: UDP path debug tool
- `unity/PiSystemBridge.cs`: Unity UDP receiver and input bridge
- `unity/WaterSystemManager.cs`: Unity-side data and chart controller
- `unity/mapswitch.cs`: Unity map layer switching logic
- `FINAL_DELIVERY_DOCUMENT_POLISHED.docx`: final polished submission document
- `generate_final_delivery_doc.ps1`: Word generation script for the final report

## Main Runtime Flow

1. Update the target Unity IP in `raspberry-pi-scripts/pi_input_sender.py` if needed.
2. Start `pi_input_sender.py` on Raspberry Pi.
3. Open the Unity scene and confirm `PiSystemBridge` is listening on UDP port `5005`.
4. Use joystick, sliders, and encoders to drive map style, river selection, menu focus, and data filtering.
5. Validate behavior using the screenshots and notes stored under `docs/`.

## Repository Layout

- `docs/`: handover notes, readable documentation, and report figures
- `docs/figures/`: high-resolution screenshots and hardware photos used in the report
- `raspberry-pi-scripts/`: Raspberry Pi-side runtime and debug scripts
- `unity/`: reviewer-friendly copies of the main Unity scripts

## Documentation Entry Points

- `docs/README.md`
- `docs/script-purposes.md`
- `docs/readable-bilingual/README.md`
- `docs/figures/README.md`

## Current Confirmed Setup

- Raspberry Pi hostname: `raspberrypi`
- Raspberry Pi user: `xlab`
- Raspberry Pi IP seen during collection: `192.168.50.38`
- Unity listener port: `5005`
- Confirmed main script: `pi_input_sender.py`

## Notes

- The `unity/` folder mirrors the key scripts so reviewers do not need to browse the entire Unity project tree.
- High-resolution screenshots are kept in Git for reference and reuse.
- This repository is intended to be easier to review than a raw Unity project folder alone.
