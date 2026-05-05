# Media Guide

## Folder Use

- `lab-photos/`: phone photos of devices, cables, room layout
- `screenshots/`: desktop screenshots of scripts, Inspector, Console, terminal
- Video download: [set up video.mp4](https://github.com/77Xiaoya/project-handover/releases/download/media-assets/set%20up%20video.mp4)

## Capture Order

To match the revised tutorial structure, collect media in this order:

1. overall hardware and lab layout
2. Raspberry Pi, breadboard, MCP3008, and power rails
3. joystick, sliders, and encoders as separate modules
4. PC-side setup screens such as `ipconfig`, `arp -a`, and SSH login
5. Raspberry Pi package installation, SPI setup, and script runtime
6. Unity receiver setup and runtime validation
7. final GitHub navigation walkthrough

## File Naming

Use readable names with a date prefix.

Examples:

- `2026-04-15-pi-terminal-startup.png`
- `2026-04-15-unity-pisystembridge-inspector.png`
- `2026-04-15-cable-layout-photo.jpg`

Suggested names for the newly added figure files:

- `2026-04-15-overall-hardware-layout.png`
- `2026-04-15-joystick-wiring.png`
- `2026-04-15-sliders-wiring.png`
- `2026-04-15-encoder-1-wiring.png`
- `2026-04-15-encoder-2-wiring.png`
- `2026-04-15-encoder-3-wiring.png`

## Rule

If a picture explains a step better than text, save the picture here and link it from the matching markdown file.

## Annotation Rule

For hardware figures, add labels for:

- component name
- pin name
- power rail or ground rail
- signal type: analog or digital
- destination: GPIO pin or MCP3008 channel
- verification result shown in terminal or Unity
