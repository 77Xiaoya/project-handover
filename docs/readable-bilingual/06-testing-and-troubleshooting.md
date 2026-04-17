# 06 Testing and Troubleshooting / 06 测试与排查

## Validation Summary / 验证概述

**English**  
The Raspberry Pi to Unity control path was validated in the lab environment. Evidence includes GPIO and ADC mapping checks, terminal output, and matching Unity-side logic.

**中文**  
树莓派到 Unity 的控制路径已在实验室环境中完成验证。证据包括 GPIO 与 ADC 映射检查、终端输出以及 Unity 侧对应逻辑。

## Main Test Scripts / 主要测试脚本

- `watch_gpio.py`: check digital input changes  
  `watch_gpio.py`：检查数字输入变化
- `mcp_read_all.py`: check ADC channel changes  
  `mcp_read_all.py`：检查 ADC 通道变化
- `joy_click_test.py`: check joystick movement and press  
  `joy_click_test.py`：检查摇杆移动和按下
- `udp_test_send.py`: check UDP path  
  `udp_test_send.py`：检查 UDP 通路

## Troubleshooting Platforms / 排查平台

**English**
- Windows PowerShell before SSH
- Raspberry Pi remote shell after SSH
- Unity Console

**中文**
- SSH 前的 Windows PowerShell
- SSH 后的树莓派远程 Shell
- Unity Console

## Common Problems / 常见问题

| Problem | Check |
| --- | --- |
| Cannot find Raspberry Pi IP | `arp -a` |
| Cannot connect by SSH | power, IP, network |
| SPI device missing | `ls /dev/spidev*` |
| SPI module missing | `lsmod | grep spi` |
| No analog response | `mcp_read_all.py` |
| No joystick or encoder response | `watch_gpio.py` |
| Unity receives nothing | check `listenPort = 5005` and Console |

| 问题 | 检查方式 |
| --- | --- |
| 找不到树莓派 IP | `arp -a` |
| 无法 SSH 连接 | 电源、IP、网络 |
| 找不到 SPI 设备 | `ls /dev/spidev*` |
| SPI 模块未加载 | `lsmod | grep spi` |
| 模拟输入无反应 | `mcp_read_all.py` |
| 摇杆或旋钮无反应 | `watch_gpio.py` |
| Unity 未收到消息 | 检查 `listenPort = 5005` 和 Console |
