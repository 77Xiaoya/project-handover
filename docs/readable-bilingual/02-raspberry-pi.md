# 02 Raspberry Pi / 02 树莓派部分

## Access Method / 访问方式

**English**  
Commands are entered from Windows PowerShell, then executed remotely on Raspberry Pi through SSH.

**中文**  
命令先在 Windows PowerShell 中输入，然后通过 SSH 在树莓派远程环境中执行。

## Runtime Software / 运行软件

**English**
- Raspberry Pi OS
- Python 3
- `gpiozero`
- `spidev`
- `socket`
- `threading`

**中文**
- Raspberry Pi OS
- Python 3
- `gpiozero`
- `spidev`
- `socket`
- `threading`

## Main Script / 主脚本

**English**  
The final integrated runtime script is `pi_input_sender.py`.

**中文**  
最终整合使用的主运行脚本为 `pi_input_sender.py`。

## MCP3008 / 模数转换

**English**  
Raspberry Pi cannot read analog voltage directly. MCP3008 converts analog joystick and slider signals into digital values through SPI.

Channel mapping:
- `CH1`: horizontal slider
- `CH3`: vertical slider
- `CH5`: joystick left-right
- `CH6`: joystick up-down

**中文**  
树莓派不能直接读取模拟电压，因此需要 MCP3008 通过 SPI 将摇杆和滑杆的模拟信号转换为数字值。

通道映射：
- `CH1`：横向滑杆
- `CH3`：纵向滑杆
- `CH5`：摇杆左右
- `CH6`：摇杆上下

## SPI Setup / SPI 配置

**English**
1. Run `sudo raspi-config`.
2. Open `Interface Options -> SPI -> Enable`.
3. Reboot using `sudo reboot`.
4. Check `ls /dev/spidev*`.
5. If needed, run `lsmod | grep spi`.
6. If SPI is still unavailable, confirm `dtparam=spi=on` in `/boot/config.txt`.

**中文**
1. 输入 `sudo raspi-config`。
2. 进入 `Interface Options -> SPI -> Enable`。
3. 使用 `sudo reboot` 重启。
4. 输入 `ls /dev/spidev*` 检查。
5. 如有需要，输入 `lsmod | grep spi`。
6. 如果 SPI 仍不可用，则确认 `/boot/config.txt` 中存在 `dtparam=spi=on`。

## Essential Wiring Summary / 核心接线汇总

**Joystick / 摇杆**
- `SW -> GPIO26`
- `VRx -> MCP3008 CH5`
- `VRy -> MCP3008 CH6`

**Sliders / 滑杆**
- Horizontal slider -> `CH1`
- Vertical slider -> `CH3`

**Encoders / 旋钮**
- Encoder 1: `S1 -> GPIO17`, `S2 -> GPIO22`, `KEY -> GPIO27`
- Encoder 2: `S1 -> GPIO18`, `S2 -> GPIO23`, `KEY -> GPIO25`
- Encoder 3: `S1 -> GPIO24`, `S2 -> GPIO16`, `KEY -> GPIO20`

## Encoder 3 Special Behavior / 旋钮 3 特殊行为

**English**  
`BTN3` toggles encoder 3 between zoom mode and rotate mode.

**中文**  
`BTN3` 用于在旋钮 3 的缩放模式与旋转模式之间切换。
