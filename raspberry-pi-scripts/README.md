# Raspberry Pi Scripts

This folder contains the Raspberry Pi-side runtime and debug scripts used by the project.

- `pi_input_sender.py`: final integrated runtime script
- `watch_gpio.py`: GPIO debug helper
- `mcp_read_all.py`: MCP3008 ADC test
- `joy_click_test.py`: joystick click test
- `udp_test_send.py`: UDP send path test

These scripts match the handover package and are intended to be easier to review than screenshots alone.

## Recommended Reading Order

1. `pi_input_sender.py`
2. `watch_gpio.py`
3. `mcp_read_all.py`
4. `joy_click_test.py`
5. `udp_test_send.py`

If you only need the final runtime logic, start with `pi_input_sender.py`.
