# Raspberry Pi Section (Bilingual)

## 1. Raspberry Pi Zero Overview / 1. 树莓派 Zero 概述

**English**  
The Raspberry Pi Zero acts as the hardware input controller of the system. It reads joystick, slider, and rotary encoder inputs, processes them in Python, and sends control messages to Unity through UDP.

**中文**  
树莓派 Zero 在本系统中充当硬件输入控制器。它负责读取摇杆、滑杆和旋钮编码器的输入，在 Python 中完成处理后，再通过 UDP 将控制消息发送到 Unity。

## 2. Software Used on Raspberry Pi / 2. 树莓派所用软件

**English**  
The Raspberry Pi side uses Raspberry Pi OS and Python 3. The main Python libraries used are:
- `gpiozero` for digital input reading
- `spidev` for reading analog values through MCP3008 via SPI
- `socket` for UDP communication
- `threading` for parallel input loops

The final integrated runtime script is `pi_input_sender.py`.

**中文**  
树莓派端使用 Raspberry Pi OS 和 Python 3。项目中主要使用以下 Python 库：
- `gpiozero`：读取数字输入
- `spidev`：通过 SPI 与 MCP3008 通信并读取模拟量
- `socket`：实现 UDP 通信
- `threading`：并行运行多个输入读取循环

最终整合使用的主脚本为 `pi_input_sender.py`。

| Software / Library | Purpose | 中文说明 |
| --- | --- | --- |
| Raspberry Pi OS | Operating system | 树莓派操作系统 |
| Python 3 | Main runtime language | 主运行语言 |
| gpiozero | Read digital input | 读取数字输入 |
| spidev | Read MCP3008 via SPI | 通过 SPI 读取 MCP3008 |
| socket | Send UDP packets | 发送 UDP 数据包 |
| threading | Parallel loops | 并行输入循环 |

## 3. Physical Input Types / 3. 物理输入类型

**English**  
The project uses two types of physical input:
- digital input
- analog input

Digital inputs include joystick press and rotary encoder signals. Analog inputs include joystick movement and slider movement.

Digital inputs are read directly from Raspberry Pi GPIO pins, while analog inputs are first converted by MCP3008 before being processed in Python.

**中文**  
本项目使用两类物理输入：
- 数字输入
- 模拟输入

数字输入包括摇杆按下和旋钮编码器信号。模拟输入包括摇杆方向移动和滑杆位移。

数字输入可直接通过树莓派 GPIO 引脚读取，而模拟输入则需要先经过 MCP3008 转换，之后才能在 Python 中处理。

## 4. MCP3008 Analog Conversion / 4. MCP3008 模拟量转换

**English**  
Raspberry Pi cannot directly read analog voltage. Therefore, the project uses MCP3008, an analog-to-digital converter (ADC), to convert slider and joystick analog signals into digital values. These values are then read through SPI using the `spidev` library.

Channel mapping:
- `CH1`: horizontal slider
- `CH3`: vertical slider
- `CH5`: joystick left-right axis
- `CH6`: joystick up-down axis

Process flow:

```text
Analog signal from joystick/slider
-> MCP3008
-> SPI
-> Raspberry Pi Python script
-> UDP message
-> Unity
```

**中文**  
树莓派本身不能直接读取模拟电压，因此本项目使用 MCP3008 模拟数字转换器（ADC），将滑杆和摇杆输出的模拟信号转换为数字值。转换后的数据通过 SPI 接口，由 `spidev` 库读取。

通道映射如下：
- `CH1`：横向滑杆
- `CH3`：纵向滑杆
- `CH5`：摇杆左右方向
- `CH6`：摇杆上下方向

流程如下：

```text
摇杆/滑杆模拟信号
-> MCP3008
-> SPI
-> 树莓派 Python 脚本
-> UDP 消息
-> Unity
```

## 5. UDP Communication to Unity / 5. 发送到 Unity 的 UDP 通信

**English**  
UDP communication on the Raspberry Pi is implemented using Python's `socket` library. On the Unity side, UDP messages are received by `PiSystemBridge.cs` through `UdpClient`. The communication port used in this project is `5005`.

Example messages include:
- `JOY:LEFT`
- `JOY:RIGHT`
- `JOY:UP`
- `JOY:DOWN`
- `JOYBTN`
- `SLIDER1:<value>`
- `SLIDER2:<value>`
- `ENC1:1` or `ENC1:-1`
- `ENC2:1` or `ENC2:-1`
- `ENC3:1` or `ENC3:-1`

Communication flow:

```text
Physical device
-> Raspberry Pi Zero
-> pi_input_sender.py
-> UDP packet
-> Unity PiSystemBridge.cs
-> scene interaction
```

**中文**  
树莓派端的 UDP 通信由 Python 的 `socket` 库实现。Unity 端通过 `PiSystemBridge.cs` 中的 `UdpClient` 接收 UDP 消息。本项目使用的通信端口为 `5005`。

消息示例如下：
- `JOY:LEFT`
- `JOY:RIGHT`
- `JOY:UP`
- `JOY:DOWN`
- `JOYBTN`
- `SLIDER1:<value>`
- `SLIDER2:<value>`
- `ENC1:1` 或 `ENC1:-1`
- `ENC2:1` 或 `ENC2:-1`
- `ENC3:1` 或 `ENC3:-1`

通信流程如下：

```text
物理设备
-> Raspberry Pi Zero
-> pi_input_sender.py
-> UDP 数据包
-> Unity PiSystemBridge.cs
-> 场景交互
```

## 6. Detailed Wiring Procedure / 6. 详细插线步骤

### 6.1 Joystick Wiring Procedure / 6.1 摇杆插线步骤

**English**  
The joystick module used in this project has five main pins: `VCC`, `GND`, `VRx`, `VRy`, and `SW`. `VCC` provides power to the joystick module. `GND` provides the common ground reference. `VRx` outputs the analog signal for left-right movement. `VRy` outputs the analog signal for up-down movement. `SW` is the built-in push-button signal generated when the joystick is pressed down.

The joystick should be wired in the following order:
1. Connect the `VCC` pin of the joystick to the positive power rail on the breadboard.
2. Connect the positive power rail to the Raspberry Pi `3.3V` supply.
3. Connect the `GND` pin of the joystick to the ground rail on the breadboard.
4. Connect the ground rail to the Raspberry Pi `GND`.
5. Connect `VRx` to breadboard position `g37`.
6. From that breadboard line, connect the signal to `MCP3008 CH5`.
7. Connect `VRy` to breadboard position `g36`.
8. From that breadboard line, connect the signal to `MCP3008 CH6`.
9. Connect `SW` directly to Raspberry Pi `GPIO26`, which corresponds to physical pin `37`.

The joystick press signal is different from the joystick movement signals. `VRx` and `VRy` are analog values, so they must be routed through MCP3008 before the Raspberry Pi can read them. `SW` is a digital signal, so it is connected directly to a GPIO input pin.

To verify the joystick wiring, move the joystick left and right and check whether `CH5` changes in the ADC test output. Move the joystick up and down and check whether `CH6` changes. Press the joystick and confirm that the terminal shows `JOYBTN`. If all three behaviors are detected correctly, the joystick wiring is working.

Common mistakes include swapping `VRx` and `VRy`, connecting `SW` to an ADC channel instead of GPIO, connecting the module to the wrong power rail, or forgetting to connect the ground rail.

**中文**  
本项目使用的摇杆模块共有五个主要引脚：`VCC`、`GND`、`VRx`、`VRy` 和 `SW`。其中，`VCC` 用于给摇杆供电，`GND` 提供公共地线参考，`VRx` 输出左右方向的模拟信号，`VRy` 输出上下方向的模拟信号，`SW` 则是在按下摇杆时产生的内置按键信号。

摇杆的接线顺序如下：
1. 将摇杆的 `VCC` 引脚连接到面包板正电源轨。
2. 将正电源轨连接到树莓派的 `3.3V` 电源。
3. 将摇杆的 `GND` 引脚连接到面包板负电源轨。
4. 将负电源轨连接到树莓派的 `GND`。
5. 将 `VRx` 接到面包板位置 `g37`。
6. 再从这一行连接到 `MCP3008 CH5`。
7. 将 `VRy` 接到面包板位置 `g36`。
8. 再从这一行连接到 `MCP3008 CH6`。
9. 将 `SW` 直接连接到树莓派 `GPIO26`，即物理引脚 `37`。

摇杆的按下信号与方向移动信号不同。`VRx` 和 `VRy` 是模拟量，因此必须先经过 MCP3008 才能被树莓派读取。`SW` 是数字信号，因此可以直接连接到 GPIO 输入引脚。

要验证摇杆接线是否正确，可以左右推动摇杆并检查 ADC 测试输出中 `CH5` 是否变化；上下推动摇杆并检查 `CH6` 是否变化；按下摇杆并确认终端出现 `JOYBTN`。如果这三种行为都能正确检测到，则说明摇杆接线正常。

常见错误包括：将 `VRx` 和 `VRy` 接反、将 `SW` 错误接到 ADC 通道、接错电源轨、或者没有连接公共地线。

### 6.2 Horizontal Slider Wiring Procedure / 6.2 横向滑杆插线步骤

**English**  
The horizontal slider has three main connections: `VCC`, `GND`, and `OUT`. `OUT` is the analog output signal of the slider and is used to represent the slider position.

The wiring steps are:
1. Connect the slider `VCC` pin to the positive power rail on the breadboard.
2. Connect the positive rail to the Raspberry Pi `3.3V` supply.
3. Connect the slider `GND` pin to the ground rail on the breadboard.
4. Connect the ground rail to the Raspberry Pi `GND`.
5. Connect the slider `OUT` pin to breadboard position `g41`.
6. From that breadboard line, connect the signal to `MCP3008 CH1`.

The horizontal slider outputs a changing analog voltage when moved. Since Raspberry Pi cannot read analog voltage directly, the output must be passed through MCP3008.

To verify the wiring, run the ADC test script and move the horizontal slider. The `CH1` value should change continuously. In the main runtime script, movement of this slider should produce `SLIDER1:<value>` messages.

Common mistakes include connecting the output to the wrong ADC channel, reversing `VCC` and `GND`, or confusing the horizontal slider with the vertical slider.

**中文**  
横向滑杆共有三个主要连接点：`VCC`、`GND` 和 `OUT`。其中 `OUT` 是滑杆的模拟输出信号，用于表示滑杆当前位置。

接线步骤如下：
1. 将滑杆的 `VCC` 引脚连接到面包板正电源轨。
2. 将正电源轨连接到树莓派 `3.3V` 电源。
3. 将滑杆的 `GND` 引脚连接到面包板负电源轨。
4. 将负电源轨连接到树莓派 `GND`。
5. 将滑杆的 `OUT` 引脚连接到面包板位置 `g41`。
6. 再从该行连接到 `MCP3008 CH1`。

横向滑杆在移动时会输出连续变化的模拟电压。由于树莓派不能直接读取模拟量，因此必须先通过 MCP3008 转换。

验证方式为：运行 ADC 测试脚本后推动横向滑杆，观察 `CH1` 数值是否连续变化。在主脚本中，该滑杆的变化应表现为 `SLIDER1:<value>` 消息。

常见错误包括：将输出接到错误的 ADC 通道、将 `VCC` 和 `GND` 接反、或者把横向滑杆和纵向滑杆弄混。

### 6.3 Vertical Slider Wiring Procedure / 6.3 纵向滑杆插线步骤

**English**  
The vertical slider also has three main connections: `VCC`, `GND`, and `OUT`.

The wiring steps are:
1. Connect the slider `VCC` pin to the positive power rail on the breadboard.
2. Connect the positive rail to the Raspberry Pi `3.3V` supply.
3. Connect the slider `GND` pin to the ground rail on the breadboard.
4. Connect the ground rail to the Raspberry Pi `GND`.
5. Connect the slider `OUT` pin to breadboard position `g39`.
6. From that breadboard line, connect the signal to `MCP3008 CH3`.

The vertical slider is also an analog input device and must be read through MCP3008.

To verify the wiring, move the vertical slider while running the ADC test script. The `CH3` value should change. In the main runtime script, movement of this slider should produce `SLIDER2:<value>` messages.

Common mistakes include wiring the signal output to `CH1` instead of `CH3`, incorrect power rail connection, and mixing up the two slider modules.

**中文**  
纵向滑杆同样有三个主要连接点：`VCC`、`GND` 和 `OUT`。

接线步骤如下：
1. 将滑杆的 `VCC` 引脚连接到面包板正电源轨。
2. 将正电源轨连接到树莓派 `3.3V` 电源。
3. 将滑杆的 `GND` 引脚连接到面包板负电源轨。
4. 将负电源轨连接到树莓派 `GND`。
5. 将滑杆的 `OUT` 引脚连接到面包板位置 `g39`。
6. 再从该行连接到 `MCP3008 CH3`。

纵向滑杆也是模拟输入设备，因此必须通过 MCP3008 读取。

验证方式为：运行 ADC 测试脚本后推动纵向滑杆，观察 `CH3` 数值是否变化。在主脚本中，该滑杆的变化应表现为 `SLIDER2:<value>` 消息。

常见错误包括：误将信号线接到 `CH1` 而不是 `CH3`、接错电源轨、或者将两个滑杆模块混淆。

### 6.4 Encoder 1 Wiring Procedure / 6.4 旋钮 1 插线步骤

**English**  
Encoder 1 has five main connections: `VCC`, `GND`, `S1`, `S2`, and `KEY`. `S1` and `S2` are used for detecting rotation direction. `KEY` is the built-in push-button signal.

The wiring steps are:
1. Connect `VCC` to the `5V` positive rail at breadboard row `1`.
2. Connect `GND` to the `5V` negative rail at breadboard row `2`.
3. Connect `S1` to Raspberry Pi `GPIO17`.
4. Connect `S2` to Raspberry Pi `GPIO22`.
5. Connect `KEY` to Raspberry Pi `GPIO27`.

To verify the wiring, rotate the encoder and check whether the Raspberry Pi script produces `ENC1:1` or `ENC1:-1`. Press the encoder and check whether `BTN1` appears.

Common mistakes include swapping `S1` and `S2`, connecting `KEY` to the wrong GPIO, and connecting the encoder to the wrong power rail.

**中文**  
旋钮 1 有五个主要连接点：`VCC`、`GND`、`S1`、`S2` 和 `KEY`。其中 `S1` 和 `S2` 用于检测旋转方向，`KEY` 为旋钮按下时的按键信号。

接线步骤如下：
1. 将 `VCC` 连接到面包板第 `1` 行的 `5V` 正电源轨。
2. 将 `GND` 连接到面包板第 `2` 行的 `5V` 负电源轨。
3. 将 `S1` 连接到树莓派 `GPIO17`。
4. 将 `S2` 连接到树莓派 `GPIO22`。
5. 将 `KEY` 连接到树莓派 `GPIO27`。

验证方式为：转动旋钮并观察树莓派脚本是否输出 `ENC1:1` 或 `ENC1:-1`；按下旋钮并观察是否出现 `BTN1`。

常见错误包括：将 `S1` 与 `S2` 接反、将 `KEY` 接到错误的 GPIO、或者将旋钮接错电源轨。

### 6.5 Encoder 2 Wiring Procedure / 6.5 旋钮 2 插线步骤

**English**  
Encoder 2 is connected in the same way as Encoder 1, but uses different GPIO pins.

The wiring steps are:
1. Connect `VCC` to the `5V` positive rail at breadboard row `13`.
2. Connect `GND` to the `5V` negative rail at breadboard row `10`.
3. Connect `S1` to Raspberry Pi `GPIO18`.
4. Connect `S2` to Raspberry Pi `GPIO23`.
5. Connect `KEY` to Raspberry Pi `GPIO25`.

To verify the wiring, rotate the encoder and confirm `ENC2:1` or `ENC2:-1` output. Press the encoder and confirm `BTN2`.

**中文**  
旋钮 2 的连接方式与旋钮 1 相同，但使用不同的 GPIO 引脚。

接线步骤如下：
1. 将 `VCC` 连接到面包板第 `13` 行的 `5V` 正电源轨。
2. 将 `GND` 连接到面包板第 `10` 行的 `5V` 负电源轨。
3. 将 `S1` 连接到树莓派 `GPIO18`。
4. 将 `S2` 连接到树莓派 `GPIO23`。
5. 将 `KEY` 连接到树莓派 `GPIO25`。

验证方式为：转动旋钮后确认终端输出 `ENC2:1` 或 `ENC2:-1`；按下旋钮后确认出现 `BTN2`。

### 6.6 Encoder 3 Wiring Procedure / 6.6 旋钮 3 插线步骤

**English**  
Encoder 3 is also wired in the same structure.

The wiring steps are:
1. Connect `VCC` to the `5V` positive rail at breadboard row `22`.
2. Connect `GND` to the `5V` negative rail at breadboard row `19`.
3. Connect `S1` to Raspberry Pi `GPIO24`.
4. Connect `S2` to Raspberry Pi `GPIO16`.
5. Connect `KEY` to Raspberry Pi `GPIO20`.

To verify the wiring, rotate the encoder and confirm `ENC3:1` or `ENC3:-1` output. Press the encoder and confirm `BTN3`.

**中文**  
旋钮 3 也采用相同的连接结构。

接线步骤如下：
1. 将 `VCC` 连接到面包板第 `22` 行的 `5V` 正电源轨。
2. 将 `GND` 连接到面包板第 `19` 行的 `5V` 负电源轨。
3. 将 `S1` 连接到树莓派 `GPIO24`。
4. 将 `S2` 连接到树莓派 `GPIO16`。
5. 将 `KEY` 连接到树莓派 `GPIO20`。

验证方式为：转动旋钮后确认终端输出 `ENC3:1` 或 `ENC3:-1`；按下旋钮后确认出现 `BTN3`。

### 6.7 Wiring Verification Strategy / 6.7 接线验证策略

**English**  
Because the breadboard wiring is dense and difficult to follow visually, the wiring was not validated only by photographs. Instead, the final mapping was confirmed using three methods:
1. breadboard position notes,
2. direct GPIO or ADC testing scripts,
3. runtime terminal output from the main Raspberry Pi script.

This approach is more reliable for reproduction than relying on a single photograph of the full breadboard.

**中文**  
由于面包板接线密集且线路交叉较多，仅依赖照片很难准确追踪所有导线。因此，本项目并不是只通过照片来验证接线，而是综合以下三种方式确认最终映射：
1. 面包板位置记录，
2. GPIO 或 ADC 测试脚本，
3. 主树莓派脚本运行时的终端输出。

与单纯依赖整张面包板照片相比，这种方法更适合后续复现。
