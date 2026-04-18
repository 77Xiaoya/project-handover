# SYSTEM DELIVERY AND TECHNICAL DOCUMENTATION
# 系统交付与技术文档

## Mixed Reality River Water Quality Visualization System
## 混合现实河流水质可视化系统

Prepared by: Zhang Xiaoya  
Supervisor: Dr. Lam Meng Chun  
Organization: FTSM / MYXLab, UKM  
Date: 17 April 2026

---

## 1. Project Overview / 1. 项目概述

**English**  
This project is a mixed reality river water quality visualization prototype. It runs in Unity on Meta Quest Pro. Joystick, slider, and encoder input is collected on Raspberry Pi, converted when necessary through MCP3008, transmitted to Unity through UDP, and used to drive map selection, filtering, and chart interaction.

The current interaction path is `mapstyle -> river -> ri -> data`.

**中文**  
本项目是一个混合现实河流水质可视化原型。系统运行在 Meta Quest Pro 的 Unity 环境中。摇杆、滑杆和旋钮输入首先在树莓派端采集，在需要时通过 MCP3008 进行模数转换，再通过 UDP 发送到 Unity，用于驱动地图选择、筛选和图表交互。

当前交互路径为 `mapstyle -> river -> ri -> data`。

[Insert Figure 1 here: Overall system architecture / 系统总览图]

## 2. System Architecture / 2. 系统架构

### 2.1 Main Components / 2.1 主要组成

**English**
- Meta Quest Pro: runtime headset
- Unity: mixed reality application
- Raspberry Pi Zero: physical control bridge
- MCP3008: analog-to-digital conversion for joystick and sliders
- CSV data: water quality source data

**中文**
- Meta Quest Pro：运行头显
- Unity：混合现实应用
- Raspberry Pi Zero：实体控制桥接设备
- MCP3008：摇杆和滑杆的模数转换器
- CSV 数据：水质原始数据

### 2.2 Data Flow / 2.2 数据流

```text
Physical input
-> Raspberry Pi Zero
-> MCP3008 if analog input is used
-> Python runtime script
-> UDP
-> Unity
-> map and data interaction
```

```text
实体输入
-> Raspberry Pi Zero
-> 若为模拟量则经过 MCP3008
-> Python 主脚本
-> UDP
-> Unity
-> 地图与数据交互
```

[Insert Figure 2 here: Data flow from Raspberry Pi to Unity / 树莓派到 Unity 的数据流图]

## 3. Raspberry Pi and Hardware / 3. 树莓派与硬件

### 3.1 Access Platform and Runtime Software / 3.1 访问平台与运行软件

**English**  
The Raspberry Pi workflow has three layers:
- Windows PowerShell: command entry platform on the PC
- SSH: remote access method
- Raspberry Pi OS and Python 3: actual execution environment

Main software and libraries:
- `Python 3`: runtime language
- `gpiozero`: reads digital input such as joystick press and encoder signals
- `spidev`: reads MCP3008 analog values through SPI
- `socket`: sends UDP messages to Unity
- `threading`: runs multiple input loops in parallel

**中文**  
树莓派工作流可分为三层：
- Windows PowerShell：电脑端输入命令的平台
- SSH：远程访问方式
- Raspberry Pi OS 和 Python 3：真正执行脚本的环境

主要软件和库：
- `Python 3`：运行语言
- `gpiozero`：读取摇杆按下和旋钮等数字输入
- `spidev`：通过 SPI 读取 MCP3008 的模拟量
- `socket`：向 Unity 发送 UDP 消息
- `threading`：并行运行多个输入循环

### 3.2 Main Script / 3.2 主脚本

**English**  
The final integrated Raspberry Pi script is `pi_input_sender.py`.

**中文**  
最终整合使用的树莓派主脚本是 `pi_input_sender.py`。

### 3.3 Main Script Explanation / 3.3 主脚本解析

#### 3.3.1 `UNITY_IP` and `UNITY_PORT`

**English**  
These variables define where the Raspberry Pi sends UDP messages. If the Unity computer changes, `UNITY_IP` must be updated.

**中文**  
这两个变量定义了树莓派发送 UDP 消息的目标位置。如果运行 Unity 的电脑发生变化，就必须更新 `UNITY_IP`。

#### 3.3.2 `send()`

**English**  
This function is the single message output point. It packages the message string and sends it to Unity through UDP.

**中文**  
这个函数是统一的消息发送出口。它将消息字符串打包后，通过 UDP 发送给 Unity。

#### 3.3.3 `encoder_loop()`

**English**  
This loop monitors encoder phase changes and sends messages such as `ENC1:1`, `ENC2:-1`, or `ENC3:1`.

**中文**  
这个循环持续监测旋钮的相位变化，并发送例如 `ENC1:1`、`ENC2:-1` 或 `ENC3:1` 这样的消息。

#### 3.3.4 `joystick_loop()`

**English**  
This loop reads joystick analog values from `CH5` and `CH6`, compares them with thresholds, and converts them into directional messages such as `JOY:LEFT`, `JOY:RIGHT`, `JOY:UP`, and `JOY:DOWN`.

**中文**  
这个循环从 `CH5` 和 `CH6` 读取摇杆模拟值，结合阈值判断后，将其转换为 `JOY:LEFT`、`JOY:RIGHT`、`JOY:UP` 和 `JOY:DOWN` 等方向消息。

#### 3.3.5 `slider_loop()`

**English**  
This loop reads `CH1` and `CH3` and sends `SLIDER1:<value>` and `SLIDER2:<value>` when the values change enough to pass the threshold.

**中文**  
这个循环读取 `CH1` 和 `CH3`，当数值变化超过设定阈值时，就发送 `SLIDER1:<value>` 和 `SLIDER2:<value>` 消息。

#### 3.3.6 Button Callbacks

**English**  
Button callbacks send `BTN1`, `BTN2`, `BTN3`, and `JOYBTN`. In the current system, `BTN3` is especially important because it changes encoder 3 between zoom mode and rotate mode.

**中文**  
按钮回调会发送 `BTN1`、`BTN2`、`BTN3` 和 `JOYBTN`。在当前系统中，`BTN3` 特别重要，因为它会切换旋钮 3 的缩放模式与旋转模式。

[Insert Figure 3 here: Key sections of pi_input_sender.py / pi_input_sender.py 关键代码截图]

### 3.4 MCP3008 and SPI / 3.4 MCP3008 与 SPI

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

#### 3.4.1 Why SPI Must Be Enabled / 3.4.1 为什么必须启用 SPI

**English**  
If SPI is not enabled, Raspberry Pi cannot communicate with MCP3008, and analog channels cannot be read correctly.

**中文**  
如果没有启用 SPI，树莓派就无法与 MCP3008 通信，也就无法正确读取模拟通道。

#### 3.4.2 SPI Setup / 3.4.2 SPI 配置

**English**
1. Connect to Raspberry Pi through SSH.
2. Run `sudo raspi-config`.
3. Open `Interface Options -> SPI -> Enable`.
4. Reboot using `sudo reboot`.
5. Reconnect and check `ls /dev/spidev*`.
6. If needed, run `lsmod | grep spi`.
7. If SPI is still unavailable, open `sudo nano /boot/config.txt` and confirm `dtparam=spi=on` exists.

**中文**
1. 先通过 SSH 连接树莓派。
2. 输入 `sudo raspi-config`。
3. 进入 `Interface Options -> SPI -> Enable`。
4. 使用 `sudo reboot` 重启。
5. 重新连接后输入 `ls /dev/spidev*` 检查。
6. 如有需要，输入 `lsmod | grep spi`。
7. 如果 SPI 仍不可用，则打开 `sudo nano /boot/config.txt`，确认存在 `dtparam=spi=on`。

#### 3.4.3 SPI Verification / 3.4.3 SPI 验证

**English**
SPI can be treated as available when:
- a device such as `/dev/spidev0.0` appears
- `lsmod | grep spi` shows `spi_bcm2835` and `spidev`
- `mcp_read_all.py` can read changing values

**中文**
当满足以下条件时，可以认为 SPI 已可用：
- 出现类似 `/dev/spidev0.0` 的设备路径
- `lsmod | grep spi` 显示 `spi_bcm2835` 和 `spidev`
- `mcp_read_all.py` 能读到变化数值

[Insert Figure 4 here: MCP3008 analog conversion flow / MCP3008 模拟量转换流程图]

### 3.5 Hardware Mapping / 3.5 硬件映射

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

**Encoder 3 special behavior / 旋钮 3 特殊行为**
- `BTN3` toggles encoder 3 between zoom mode and rotate mode
- `BTN3` 用于在旋钮 3 的缩放模式与旋转模式之间切换

[Insert Figure 5 here: Raspberry Pi and breadboard overview / 树莓派与面包板整体图]
[Insert Figure 6 here: Joystick, slider, and encoder mapping / 摇杆滑杆旋钮映射图]

## 4. Unity Environment and Scripts / 4. Unity 环境与脚本

### 4.1 Unity Environment / 4.1 Unity 环境

**English**  
The project should be opened with Unity `6000.0.62f1`. The environment is configured for Meta Quest Pro mixed reality and depends on Meta XR and Unity XR packages.

Main scene:
- `Assets/310.unity`

Important package groups:
- Meta XR SDK
- OpenXR
- Android XR OpenXR
- AR Foundation
- XR Hands
- XR Interaction Toolkit

**中文**  
项目应使用 Unity `6000.0.62f1` 打开。当前环境面向 Meta Quest Pro 混合现实，并依赖 Meta XR 和 Unity XR 相关包。

主场景：
- `Assets/310.unity`

重要包组：
- Meta XR SDK
- OpenXR
- Android XR OpenXR
- AR Foundation
- XR Hands
- XR Interaction Toolkit

#### 4.1.1 What This Means for Rebuild / 4.1.1 这对环境重建意味着什么

**English**  
When reconstructing the Unity environment, package compatibility and Android XR support must be checked before editing scripts or scene logic.

**中文**  
在重建 Unity 环境时，应先检查包兼容性和 Android XR 支持，而不是先修改脚本或场景逻辑。

[Insert Figure 7 here: Unity project and package setup / Unity 项目与包配置截图]

### 4.2 Key Scripts / 4.2 关键脚本

| Script | Purpose | What to check |
| --- | --- | --- |
| `PiSystemBridge.cs` | receives UDP messages and maps them to Unity actions | port, object references, slider ranges |
| `WaterSystemManager.cs` | controls river state, filtering, and charts | toggles, dropdowns, chart references |
| `mapswitch.cs` | switches map layers | default, satellite, and terrain map references |

| 脚本 | 用途 | 重点检查 |
| --- | --- | --- |
| `PiSystemBridge.cs` | 接收 UDP 消息并映射为 Unity 动作 | 端口、对象引用、滑杆范围 |
| `WaterSystemManager.cs` | 控制河流状态、筛选和图表 | toggles、dropdowns、图表引用 |
| `mapswitch.cs` | 切换地图图层 | 默认、卫星和地形地图引用 |

### 4.3 Unity Script Explanation / 4.3 Unity 脚本解析

#### 4.3.1 `PiSystemBridge.cs`

**English**  
This script is the Unity-side bridge. It listens on UDP port `5005`, receives Raspberry Pi messages, and converts them into Unity actions.

Main message behavior:
- `JOYBTN`: confirm action
- `JOY:LEFT/RIGHT/UP/DOWN`: move selection
- `SLIDER1/SLIDER2`: update slider state
- `BTN3`: switch encoder 3 mode
- `ENC3:*`: zoom or rotate the map depending on current mode

**中文**  
这个脚本是 Unity 侧的通信桥梁。它在 UDP 端口 `5005` 上监听，接收树莓派消息，并将其转换为 Unity 动作。

主要消息行为：
- `JOYBTN`：确认动作
- `JOY:LEFT/RIGHT/UP/DOWN`：移动选择
- `SLIDER1/SLIDER2`：更新滑杆状态
- `BTN3`：切换旋钮 3 模式
- `ENC3:*`：根据当前模式执行地图缩放或旋转

#### 4.3.2 `WaterSystemManager.cs`

**English**  
This script manages selected river state, filtering logic, and chart refresh. It is the main data-side controller after Unity receives the input.

**中文**  
这个脚本负责管理已选河流状态、筛选逻辑和图表刷新。它是 Unity 收到输入后的主要数据逻辑控制器。

#### 4.3.3 `mapswitch.cs`

**English**  
This script switches the visible map layer between default, satellite, and terrain.

**中文**  
这个脚本负责在默认、卫星和地形地图之间切换显示图层。

[Insert Figure 8 here: Key Unity scripts and scene relationship / Unity 关键脚本与场景关系图]

### 4.4 PiSystemBridge Inspector / 4.4 PiSystemBridge Inspector

Must check:
- `listenPort = 5005`
- `mapSwitcher`
- `waterSystemManager`
- `mapRoot`
- `sliderXContainer`
- `sliderYContainer`
- `intersectionMarker`
- river references
- `classPanel`
- `dataPanel`

必须检查：
- `listenPort = 5005`
- `mapSwitcher`
- `waterSystemManager`
- `mapRoot`
- `sliderXContainer`
- `sliderYContainer`
- `intersectionMarker`
- 河流引用
- `classPanel`
- `dataPanel`

If these references are missing, Unity may still enter Play mode, but the Raspberry Pi integration will not behave correctly.

如果这些引用缺失，Unity 虽然可能仍然可以进入 Play 模式，但树莓派集成不会按预期工作。

[Insert Figure 9 here: PiSystemBridge Inspector screenshot / PiSystemBridge Inspector 截图]

## 5. Setup and Operation / 5. 配置与操作

### 5.1 Raspberry Pi Setup / 5.1 树莓派配置

1. Open Windows PowerShell.  
   打开 Windows PowerShell。  
2. Run `arp -a`.  
   输入 `arp -a`。  
3. Connect using `ssh xlab@192.168.50.38`.  
   使用 `ssh xlab@192.168.50.38` 连接树莓派。  
4. Confirm `pi_input_sender.py` is present.  
   确认 `pi_input_sender.py` 存在。  
5. Edit it if needed using `nano pi_input_sender.py`.  
   如有需要，使用 `nano pi_input_sender.py` 修改。  
6. Confirm `UNITY_IP` and `UNITY_PORT`.  
   确认 `UNITY_IP` 和 `UNITY_PORT`。  
7. Enable SPI using `sudo raspi-config`.  
   使用 `sudo raspi-config` 启用 SPI。  
8. Reboot if needed.  
   如有需要，重启树莓派。  
9. Check `ls /dev/spidev*`.  
   输入 `ls /dev/spidev*` 检查。  
10. If needed, check `lsmod | grep spi`.  
    如有需要，输入 `lsmod | grep spi`。  
11. Run `python3 pi_input_sender.py`.  
    输入 `python3 pi_input_sender.py` 运行脚本。  

[Insert Figure 10 here: Raspberry Pi setup and SSH access / 树莓派配置与 SSH 访问截图]

### 5.2 Unity Setup / 5.2 Unity 配置

1. Open `Assets/310.unity`.  
   打开 `Assets/310.unity`。  
2. Select the `PiSystemBridge` GameObject.  
   选中 `PiSystemBridge` GameObject。  
3. Check Inspector references.  
   检查 Inspector 引用。  
4. Confirm `listenPort = 5005`.  
   确认 `listenPort = 5005`。  
5. Press Play.  
   点击 Play。  

### 5.3 Runtime Flow / 5.3 运行流程

1. `ENC1` switches map style.  
   `ENC1` 切换地图样式。  
2. Sliders position the river-selection intersection.  
   滑杆控制河流选择交点位置。  
3. `JOYBTN` confirms the hovered river.  
   `JOYBTN` 确认当前悬停河流。  
4. `ENC2` changes menu focus.  
   `ENC2` 切换菜单焦点。  
5. Joystick directions move inside the active menu.  
   摇杆方向输入在当前菜单内移动。  
6. `JOYBTN` confirms the current menu selection.  
   `JOYBTN` 确认当前菜单选择。  
7. `BTN3` switches encoder 3 mode if needed.  
   如有需要，`BTN3` 切换旋钮 3 模式。  
8. `ENC3` zooms or rotates the map.  
   `ENC3` 执行地图缩放或旋转。  

## 6. Testing, Limitations, and References / 6. 测试、限制与参考资料

### 6.1 Validation / 6.1 验证

**English**  
The Raspberry Pi to Unity control path was validated in the lab environment. Evidence includes:
- GPIO and ADC mapping checks
- terminal output
- matching Unity-side logic

**中文**  
树莓派到 Unity 的控制路径已在实验室环境中完成验证。证据包括：
- GPIO 与 ADC 映射检查
- 终端输出
- Unity 侧对应逻辑

[Insert Figure 11 here: Raspberry Pi terminal output / 树莓派终端输出截图]
[Insert Figure 12 here: Unity Console validation / Unity Console 验证截图]

### 6.2 Troubleshooting / 6.2 故障排查

Main platforms:
- Windows PowerShell before SSH
- Raspberry Pi remote shell after SSH
- Unity Console

主要排查平台：
- SSH 前的 Windows PowerShell
- SSH 后的树莓派远程 Shell
- Unity Console

Key checks:
- `arp -a`
- `ls /dev/spidev*`
- `lsmod | grep spi`
- `mcp_read_all.py`
- `watch_gpio.py`
- Unity `listenPort = 5005`

关键检查项：
- `arp -a`
- `ls /dev/spidev*`
- `lsmod | grep spi`
- `mcp_read_all.py`
- `watch_gpio.py`
- Unity `listenPort = 5005`

### 6.3 Known Limitations / 6.3 已知限制

**English**
- dense breadboard wiring reduces visual clarity
- system depends on stable local network communication
- analog input may require threshold tuning
- the current system should be treated as a validated prototype, not a production-hardened deployment

**中文**
- 面包板接线密集，降低了可视清晰度
- 系统依赖稳定的局域网通信
- 模拟输入可能需要阈值调节
- 当前系统应被视为已验证原型，而不是生产级部署版本

### 6.4 GitHub Repository / 6.4 GitHub 仓库

Repository:
- `https://github.com/77Xiaoya/project-handover`

Stored there:
- Raspberry Pi scripts
- wiring notes
- revised bilingual documentation
- Unity bridge script copy

仓库中保存：
- 树莓派脚本
- 接线记录
- 修订版双语文档
- Unity 桥接脚本副本

[Insert Figure 13 here: GitHub repository screenshot / GitHub 仓库截图]

### 6.5 Conclusion / 6.5 结论

**English**  
The current prototype successfully integrates Raspberry Pi physical input with a Unity mixed reality application on Meta Quest Pro. The system has been documented as a delivery-oriented technical package that supports reconstruction, testing, and future maintenance.

**中文**  
当前原型已成功将树莓派实体输入与运行在 Meta Quest Pro 上的 Unity 混合现实应用集成在一起。该系统现已被整理为一套面向交付的技术文档包，可支持后续重建、测试和维护。
