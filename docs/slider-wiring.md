# Slider Wiring

This file records the two slider connections used in the project.

## Purpose

The project uses two analog slider signals:

- one horizontal slider
- one vertical slider

Both slider outputs are analog, so they are read through the `MCP3008` ADC instead of directly through Raspberry Pi GPIO pins.

## Verified Signal Mapping

| Slider | Breadboard output note | Final destination | Power note | Ground note | Verification |
| --- | --- | --- | --- | --- | --- |
| Horizontal slider | breadboard `g41` | `MCP3008 CH1` | `VCC -> 3.3V positive rail`, corrected lab note near row `7` | `GND -> 3.3V negative rail`, corrected lab note near row `43` | moving the horizontal slider changes `CH1` |
| Vertical slider | breadboard `g39` | `MCP3008 CH3` | `VCC -> 3.3V positive rail`, corrected lab note near row `6` | `GND -> 3.3V negative rail`, corrected lab note near row `48` | moving the vertical slider changes `CH3` |

## Working Principle

- Each slider outputs a changing analog voltage depending on its position.
- Raspberry Pi cannot read analog voltage directly.
- Therefore, the slider output is routed to the `MCP3008` ADC.
- In this project:
- horizontal slider -> `CH1`
- vertical slider -> `CH3`

## Software Behavior

The Raspberry Pi main script `pi_input_sender.py` reads the slider values from the ADC and sends them to Unity.

- horizontal slider sends `SLIDER1:<value>`
- vertical slider sends `SLIDER2:<value>`

Unity receives these values in `PiSystemBridge.cs`.

- `SLIDER1` updates the horizontal slider position in Unity
- `SLIDER2` updates the vertical slider position in Unity
- the intersection of both slider positions is used for river selection logic

## Validation Evidence

The slider mapping was verified through ADC testing and runtime behavior.

- horizontal movement changes `CH1`
- vertical movement changes `CH3`
- the main script sends `SLIDER1` and `SLIDER2` messages when values change enough to pass the threshold

## Notes For Documentation

- Dense breadboard wiring made the exact jumper path difficult to capture clearly in photographs.
- For that reason, the final slider mapping was confirmed through breadboard notes plus runtime ADC validation.
- The slider rail row notes were corrected after a later lab re-check. Earlier power and ground row notes were off by `+2` and should not be reused.
