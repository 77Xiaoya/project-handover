# Figure Placement Guide

Use this guide when placing images into `FINAL_Document.docx` and `FINAL_Document_commented (1).docx`.

The goal is one-to-one correspondence between:

1. tutorial step
2. image
3. caption
4. on-image labels

This file assumes that:

- hardware photos live in `docs/06-media/lab-photos/`
- screenshots live in `docs/06-media/screenshots/`

## Placement Rules

1. Use one image for one teaching purpose whenever possible.
2. Do not place multiple unrelated screenshots under one caption.
3. If one figure contains multiple sub-images, label them clearly as `(a)`, `(b)`, `(c)`.
4. Put the explanatory paragraph immediately below the figure caption.
5. Add labels on the image itself for pins, rails, channels, or file names when the image would otherwise be unclear.

## Section 1: Project Overview

### Figure 1. Overall system architecture

- File: `docs/06-media/lab-photos/overall.png`
- Word location: below `Figure 1. Overall system architecture`
- Purpose: show the full physical delivery path from controller hardware to Raspberry Pi and Unity
- Caption text:
  `Figure 1. Overall system architecture`
- Paragraph below the figure:
  `This figure summarizes the physical setup used in the project, including the Raspberry Pi, breadboard, MCP3008, joystick, sliders, encoders, and their role in the end-to-end interaction workflow.`
- On-image labels to add:
  - `Raspberry Pi Zero`
  - `Breadboard`
  - `MCP3008`
  - `Joystick`
  - `Horizontal slider`
  - `Vertical slider`
  - `Encoder 1`
  - `Encoder 2`
  - `Encoder 3`

## Section 2: System Architecture

### Figure 2. Data flow from Raspberry Pi to Unity

This figure should be built as a 2 x 3 screenshot group if space allows.

- Place these files under one grouped figure:
  - `docs/06-media/lab-photos/physical controllers overview.jpg`
  - `docs/06-media/screenshots/channel reading on Raspberry Pi.png`
  - `docs/06-media/screenshots/terminal output.png`
  - `docs/06-media/screenshots/UDP message .png`
  - `docs/06-media/screenshots/PiSystemBridge listening on port 5005 in Unity.png`
  - `docs/06-media/screenshots/final Unity response in map, river, RI, and data view1.png`
- Word location: below `Figure 2. Data flow from Raspberry Pi to Unity`
- Recommended sub-labels:
  - `(a) Physical controllers overview`
  - `(b) Channel reading on Raspberry Pi`
  - `(c) Python runtime output`
  - `(d) UDP message examples`
  - `(e) PiSystemBridge listening on port 5005`
  - `(f) Final Unity response`
- Caption text:
  `Figure 2. Data flow from Raspberry Pi to Unity`
- Paragraph below the figure:
  `This figure shows the runtime data path from physical controller movement, to Raspberry Pi acquisition and message generation, to Unity-side UDP reception and final visual response.`

## Section 3: Raspberry Pi and Hardware

### Figure 3. Key sections of pi_input_sender.py

- Use these existing screenshots:
  - `docs/06-media/screenshots/figure-03-network-and-send.png`
  - `docs/06-media/screenshots/figure-03-button-callbacks.png`
  - `docs/06-media/screenshots/figure-03-encoder-loop.png`
  - `docs/06-media/screenshots/figure-03-adc-and-thresholds.png`
  - `docs/06-media/screenshots/figure-03-joystick-and-slider-loop.png`
- Word location: below `Figure 3. Key sections of pi_input_sender.py`
- Recommended sub-labels:
  - `(a) Network configuration and send()`
  - `(b) Button callback registration`
  - `(c) encoder_loop() implementation`
  - `(d) read_adc() and ADC thresholds`
  - `(e) joystick_loop() and slider_loop()`
- Caption text:
  `Figure 3. Key sections of pi_input_sender.py`

### Figure 4. MCP3008 analog conversion flow

Use a three-part figure here.

- Files:
  - `docs/06-media/screenshots/SPI wiring reference between Raspberry Pi and MCP3008.png`
  - `docs/06-media/lab-photos/sliders.png`
  - `docs/06-media/screenshots/mcp_read_all.py can read changing values.png`
- Word location: below `Figure 4. MCP3008 analog conversion flow`
- Recommended sub-labels:
  - `(a) SPI wiring reference between Raspberry Pi and MCP3008`
  - `(b) Analog input sources from sliders and joystick`
  - `(c) ADC runtime verification output`
- Caption text:
  `Figure 4. MCP3008 analog conversion flow`
- Paragraph below the figure:
  `This figure explains how analog voltage from the joystick and sliders is converted by MCP3008 into digital channel values that can be read by Python through SPI.`
- On-image labels to add:
  - `CH1`
  - `CH3`
  - `CH5`
  - `CH6`
  - `VDD`
  - `VREF`
  - `AGND`
  - `CLK`
  - `DOUT`
  - `DIN`
  - `CS`
  - `horizontal slider`
  - `vertical slider`
  - `joystick X`
  - `joystick Y`

### Figure 5. Raspberry Pi and breadboard overview

- File: `docs/06-media/lab-photos/overall.png`
- Word location: below `Figure 5. Raspberry Pi and breadboard overview`
- Purpose: show the physical prototype layout clearly before module-by-module mapping
- Caption text:
  `Figure 5. Raspberry Pi and breadboard overview`
- On-image labels to add:
  - `breadboard center gap`
  - `a-e side`
  - `f-j side`
  - `3.3V rail`
  - `5V rail`
  - `GND rail`
  - `Raspberry Pi Zero`
  - `MCP3008`

### Figure 6. Joystick, slider, and encoder mapping

This should be a multi-image hardware mapping figure.

- Files:
  - `docs/06-media/lab-photos/joystick.png`
  - `docs/06-media/lab-photos/sliders.png`
  - `docs/06-media/lab-photos/Rotary Knob1.png`
  - `docs/06-media/lab-photos/Rotary Knob2.png`
  - `docs/06-media/lab-photos/Rotary Knob3.png`
- Word location: below `Figure 6. Joystick, slider, and encoder mapping`
- Recommended sub-labels:
  - `(a) Joystick mapping`
  - `(b) Slider mapping`
  - `(c) Encoder 1 mapping`
  - `(d) Encoder 2 mapping`
  - `(e) Encoder 3 mapping`
- Caption text:
  `Figure 6. Joystick, slider, and encoder mapping`
- Required labels:
  - Joystick:
    - `VCC`
    - `GND`
    - `VRx -> CH5`
    - `VRy -> CH6`
    - `SW -> GPIO26`
  - Sliders:
    - `Horizontal slider OUT -> CH1`
    - `Vertical slider OUT -> CH3`
    - `VCC`
    - `GND`
  - Encoder 1:
    - `S1 -> GPIO17`
    - `S2 -> GPIO22`
    - `KEY -> GPIO27`
  - Encoder 2:
    - `S1 -> GPIO18`
    - `S2 -> GPIO23`
    - `KEY -> GPIO25`
  - Encoder 3:
    - `S1 -> GPIO24`
    - `S2 -> GPIO16`
    - `KEY -> GPIO20`
    - `BTN3 toggles zoom / rotate`

## Section 4: Unity Environment and Scripts

### Figure 7. Unity project and XR package setup

- Files:
  - `docs/06-media/screenshots/figure-07-package-manager.png`
  - `docs/06-media/screenshots/figure-07-xr-plugin-management.png`
- Word location: below `Figure 7. Unity project and XR package setup`
- Caption text:
  `Figure 7. Unity project and XR package setup`

### Figure 8. Key Unity scripts and scene relationship

- Files:
  - `docs/06-media/screenshots/figure-08-water-system-manager-upper.png`
  - `docs/06-media/screenshots/figure-08-water-system-manager-lower.png`
  - `docs/06-media/screenshots/figure-08-map-switcher.png`
- Word location: below `Figure 8. Key Unity scripts and scene relationship`
- Caption text:
  `Figure 8. Key Unity scripts and scene relationship`

### Figure 9. PiSystemBridge Inspector screenshot

- Files:
  - `docs/06-media/screenshots/figure-09-pisystembridge-upper.png`
  - `docs/06-media/screenshots/figure-09-pisystembridge-lower.png`
- Word location: below `Figure 9. PiSystemBridge Inspector screenshot`
- Caption text:
  `Figure 9. PiSystemBridge Inspector screenshot`
- Key labels to mention in nearby text:
  - `listenPort = 5005`
  - `mapSwitcher`
  - `waterSystemManager`
  - `mapRoot`

## Section 5: Setup and Operation

### Figure 10. Raspberry Pi setup and SSH access

- Files:
  - `docs/06-media/screenshots/Windows PowerShell window.png`
  - `docs/06-media/screenshots/arp-a pi ip checking.png`
  - `docs/06-media/screenshots/SSH login success screen.png`
  - `docs/06-media/screenshots/python3 --version output.png`
- Word location: below `Figure 10. Raspberry Pi setup and SSH access`
- Recommended sub-labels:
  - `(a) Windows PowerShell`
  - `(b) arp -a IP lookup`
  - `(c) SSH login success`
  - `(d) Python version check`
- Caption text:
  `Figure 10. Raspberry Pi setup and SSH access`

### Inserted setup screenshots in Section 5.1

These can stay inline instead of receiving new figure numbers if the page gets crowded.

- `docs/06-media/screenshots/pi_input_sender.py line showing UNITY_IP and UNITY_PORT(pc).png`
  - Place after the paragraph that explains `UNITY_IP` and `UNITY_PORT`
- `docs/06-media/screenshots/installation command output.png`
  - Place after package installation step
- `docs/06-media/screenshots/Imports OK output.png`
  - Place after the import verification step
- `docs/06-media/screenshots/Open Interface Options .png`
  - Place in SPI setup steps
- `docs/06-media/screenshots/- SPI - Enable.png`
  - Place in SPI setup steps
- `docs/06-media/screenshots/- Enable.png`
  - Place in SPI setup steps
- `docs/06-media/screenshots/Reboot using sudo reboot.png`
  - Place after reboot instruction
- `docs/06-media/screenshots/- ls devspidev result- lsmod  grep spi result.png`
  - Place in SPI verification steps

### Figure 11. Raspberry Pi terminal output for encoders, sliders, and joystick

- Files:
  - `docs/06-media/screenshots/terminal output.png`
  - `docs/06-media/screenshots/UDP message .png`
  - `docs/06-media/screenshots/channel reading on Raspberry Pi.png`
- Word location: below `Figure 11. Raspberry Pi terminal output for encoders, sliders, and joystick`
- Recommended sub-labels:
  - `(a) Main runtime output`
  - `(b) UDP message examples`
  - `(c) ADC channel reading`
- Caption text:
  `Figure 11. Raspberry Pi terminal output for encoders, sliders, and joystick`

### Figure 12. MR runtime interface and data interaction

- Files:
  - `docs/06-media/screenshots/final Unity response in map, river, RI, and data view1.png`
  - `docs/06-media/screenshots/final Unity response in map, river, RI, and data view2.png`
  - `docs/06-media/screenshots/figure-12-mr-runtime.png`
- Word location: below `Figure 12. MR runtime interface and data interaction`
- Caption text:
  `Figure 12. MR runtime interface and data interaction`
- On-image labels to add if needed:
  - `map style`
  - `selected river`
  - `RI focus`
  - `data panel`

## Section 6: Testing, Limitations, and References

### GitHub repository screenshots

These can be grouped into a final repository figure if you want to add one.

- Place after `6.4 GitHub Repository`
- Suggested files:
  - root README screenshot
  - raspberry-pi-scripts folder screenshot
  - wiring notes screenshot
  - PiSystemBridge.cs location screenshot
- Suggested caption if grouped:
  `Figure 13. GitHub repository navigation for reconstruction and review`

## File Naming Cleanup Recommendation

Several current filenames are descriptive but hard to reuse in Word and in the final report. Before final submission, consider renaming them to stable names such as:

- `2026-05-04-powershell-window.png`
- `2026-05-04-arp-a.png`
- `2026-05-04-ssh-login.png`
- `2026-05-04-python-version.png`
- `2026-05-04-package-install.png`
- `2026-05-04-imports-ok.png`
- `2026-05-04-spi-menu.png`
- `2026-05-04-spi-verification.png`
- `2026-05-04-pi-unity-ip-port.png`
- `2026-05-04-pi-terminal-runtime.png`
- `2026-05-04-udp-messages.png`
- `2026-05-04-adc-reading.png`

## Final Check Before Word Layout

Before placing figures into Word, confirm for each image:

1. the image is readable at document scale
2. the filename is understandable
3. the figure has only one teaching purpose
4. the surrounding text explicitly refers to the image
5. the image has enough labels to make reproduction possible without verbal explanation
