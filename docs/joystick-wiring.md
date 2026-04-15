# Joystick Wiring

This file records the joystick wiring in a form that is easier to reproduce than photographs alone.

## Purpose

The joystick provides:

- one analog signal for left and right movement
- one analog signal for up and down movement
- one digital push-button signal for press confirmation

In this project:

- joystick movement is used for menu navigation
- joystick press is used for confirmation in Unity

## Verified Signal Mapping

| Joystick pin | Breadboard note | Final destination | Function | Verification |
| --- | --- | --- | --- | --- |
| `SW` | not fully recorded on breadboard | Raspberry Pi physical pin `37` (`BCM26`) | joystick press button | pressing the joystick prints `JOYBTN` |
| `VRx` | breadboard `g37` | `MCP3008 CH5` | left and right movement | moving left or right changes `CH5` |
| `VRy` | breadboard `g36` | `MCP3008 CH6` | up and down movement | moving up or down changes `CH6` |
| `GND` | ground rail | Raspberry Pi `GND`; lab note recorded as rail connected near physical pin `18` | ground reference | required for stable reading |
| `VCC` | positive rail | Raspberry Pi `3.3V`; exact physical power pin should be rechecked before final hardware redraw | joystick power | required for stable reading |

## Working Principle

The joystick has two different signal types.

### Analog movement

- `VRx` outputs a changing analog voltage when the joystick moves left or right.
- `VRy` outputs a changing analog voltage when the joystick moves up or down.
- Raspberry Pi cannot read analog voltage directly.
- Therefore, both analog signals are sent to the `MCP3008` ADC.
- In this project:
- `VRx -> CH5`
- `VRy -> CH6`

### Digital press

- `SW` is not an analog signal.
- It is a built-in button switch inside the joystick.
- It only has two states: pressed or not pressed.
- Because of that, it is connected directly to a Raspberry Pi GPIO input.
- In this project, `SW -> BCM26` which is physical pin `37`.

## Software Behavior

The Raspberry Pi main script `pi_input_sender.py` interprets the joystick as follows:

- left movement sends `JOY:LEFT`
- right movement sends `JOY:RIGHT`
- up movement sends `JOY:UP`
- down movement sends `JOY:DOWN`
- press sends `JOYBTN`

Unity receives these UDP messages in `PiSystemBridge.cs`.

- directional messages are used for navigation
- `JOYBTN` is used as the confirmation action

## Validation Evidence

The joystick behavior was verified in the lab using runtime output.

Observed terminal output included:

```text
JOY:UP
JOY:DOWN
JOY:LEFT
JOY:RIGHT
JOYBTN
```

This confirms:

- the joystick directional axes are active
- the joystick press switch is active
- the Raspberry Pi script can send both analog-derived direction events and the digital press event

## Notes For Documentation

- Dense breadboard wiring made it difficult to visually trace every jumper wire in photographs.
- For that reason, final joystick mapping was verified primarily through runtime testing and ADC or GPIO diagnostics.
- The exact physical `3.3V` power pin on the Raspberry Pi should be double-checked if a final pin-accurate hardware drawing is required.
