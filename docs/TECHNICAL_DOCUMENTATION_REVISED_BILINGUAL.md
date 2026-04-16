# TECHNICAL DOCUMENTATION
# 技术文档

## Mixed Reality River Water Quality Visualization System
## 混合现实河流水质可视化系统

### Revised Bilingual Technical Documentation and System Handover Draft
### 修订版双语技术文档与系统交接草稿

Prepared by: Zhang Xiaoya  
Supervisor: Dr. Lam Meng Chun  
Organization: FTSM / MYXLab, UKM  
Version: 2.0 Revised Draft  
Date: 17 April 2026

---

## Document Control
## 文档控制信息

| Item | Details |
| --- | --- |
| Document Title | Technical Documentation for Mixed Reality River Water Quality Visualization System |
| Document Type | Internship Final Technical Documentation |
| Project Type | Mixed Reality Visualization Prototype |
| Version | 2.0 Revised Draft |
| Prepared By | Zhang Xiaoya |
| Supervisor | Dr. Lam Meng Chun |
| Last Updated | 17 April 2026 |
| Status | Review Draft |

| 项目 | 内容 |
| --- | --- |
| 文档标题 | 混合现实河流水质可视化系统技术文档 |
| 文档类型 | 实习最终技术文档 |
| 项目类型 | 混合现实可视化原型 |
| 版本 | 2.0 修订草稿 |
| 编写人 | Zhang Xiaoya |
| 导师 | Dr. Lam Meng Chun |
| 最后更新 | 2026年4月17日 |
| 状态 | 评审草稿 |

## Purpose of This Revision
## 本次修订目的

**English**  
This revised document restructures the original draft according to meeting feedback. The main improvements focus on Raspberry Pi software explanation, MCP3008 details, UDP communication, detailed physical wiring, setup procedure, troubleshooting platforms, and a clearer document structure for technical handover.

**中文**  
本次修订根据会议反馈重新组织了原有草稿。重点补充了树莓派所用软件说明、MCP3008 细节、UDP 通信流程、详细实体接线、配置流程、故障排查平台，以及更清晰的交接型文档结构。

---

## Executive Summary
## 执行摘要

**English**  
This document describes the Mixed Reality River Water Quality Visualization System developed during the internship period. The system combines a Unity-based mixed reality application running on Meta Quest Pro with a physical control interface connected through Raspberry Pi Zero and MCP3008. Water quality records are loaded from CSV-based datasets and visualized through map style selection, river selection, class filtering, dataset switching, date filtering, and parameter-based chart rendering.

The current prototype follows a four-stage interaction path centered on `mapstyle -> river -> ri -> data`. Physical input is captured on the Raspberry Pi side, converted where necessary through MCP3008, and sent to Unity through UDP messages. Inside Unity, `PiSystemBridge.cs` acts as the communication bridge, while `WaterSystemManager.cs` manages filtering logic and chart updates.

This document is written as a technical handover reference rather than an academic report. Its main goal is to help supervisors, lab members, and future developers understand how the system is built, configured, operated, tested, and maintained.

**中文**  
本文件描述了在实习期间开发的混合现实河流水质可视化系统。该系统将运行在 Meta Quest Pro 上的 Unity 混合现实应用，与通过 Raspberry Pi Zero 和 MCP3008 连接的实体输入控制界面结合在一起。系统从 CSV 数据集中读取水质记录，并通过地图样式选择、河流选择、水质等级筛选、数据集切换、日期筛选和参数图表渲染实现可视化。

当前原型遵循 `mapstyle -> river -> ri -> data` 的四阶段交互路径。实体输入首先在树莓派端被读取，需要时再通过 MCP3008 进行模拟量转换，之后通过 UDP 消息发送到 Unity。在 Unity 内部，`PiSystemBridge.cs` 负责通信桥接，`WaterSystemManager.cs` 负责筛选逻辑和图表更新。

本文件定位为技术交接参考，而非学术论文。其主要目标是帮助导师、实验室成员以及后续开发者理解系统如何搭建、配置、运行、测试和维护。

## Quick Reference
## 快速理解

| Topic | Summary |
| --- | --- |
| Runtime headset | Meta Quest Pro |
| Main engine | Unity 6000.0.62f1 |
| Hardware bridge | Raspberry Pi Zero + MCP3008 |
| Communication method | UDP |
| Main Raspberry Pi script | `pi_input_sender.py` |
| Main Unity bridge script | `PiSystemBridge.cs` |
| Main data script | `WaterSystemManager.cs` |
| Core interaction path | `mapstyle -> river -> ri -> data` |

| 主题 | 说明 |
| --- | --- |
| 运行头显 | Meta Quest Pro |
| 主要引擎 | Unity 6000.0.62f1 |
| 硬件桥接 | Raspberry Pi Zero + MCP3008 |
| 通信方式 | UDP |
| 树莓派主脚本 | `pi_input_sender.py` |
| Unity 主桥接脚本 | `PiSystemBridge.cs` |
| 主要数据脚本 | `WaterSystemManager.cs` |
| 核心交互路径 | `mapstyle -> river -> ri -> data` |

---

## Table of Contents
## 目录

1. Introduction / 引言  
2. Project Overview / 项目概述  
3. System Overview / 系统总览  
4. System Architecture / 系统架构  
5. Raspberry Pi and Hardware Documentation / 树莓派与硬件文档  
6. Unity and Software Documentation / Unity 与软件文档  
7. Data and Communication Documentation / 数据与通信文档  
8. Installation and Deployment Guide / 安装与部署指南  
9. Operation Manual / 操作手册  
10. Testing and Validation / 测试与验证  
11. Troubleshooting / 故障排查  
12. Known Issues and Limitations / 已知问题与限制  
13. Maintenance and Future Recommendations / 维护与后续建议  
Appendices / 附录

---

## 1. Introduction
## 1. 引言

### 1.1 Document Purpose / 1.1 文档目的

**English**  
The purpose of this document is to provide a complete technical reference for the current implementation of the project. It is written to support project review, future maintenance, prototype continuation, and onboarding of the next developer or intern.

**中文**  
本文件旨在为当前项目实现提供完整的技术参考，用于支持项目评审、后续维护、原型延续开发以及下一位开发者或实习生的接手工作。

### 1.2 Scope of Documentation / 1.2 文档范围

**English**  
This documentation covers the current prototype implemented in Unity with physical control integration through Raspberry Pi Zero. It focuses on architecture, hardware and software components, control flow, deployment requirements, operation steps, testing evidence, and current limitations.

**中文**  
本技术文档覆盖当前基于 Unity 实现、并通过 Raspberry Pi Zero 集成实体控制器的系统原型。内容重点包括系统架构、软硬件组成、控制流程、部署要求、操作步骤、测试证据以及当前限制。

### 1.3 Intended Audience / 1.3 目标读者

**English**  
The main readers of this document are:
- project supervisor
- lab members involved in prototype continuation
- future interns or developers
- reviewers who need technical understanding of the implementation

**中文**  
本文件的主要读者包括：
- 项目导师
- 参与后续原型开发的实验室成员
- 后续实习生或开发者
- 需要了解实现细节的评审人员

### 1.4 Document Organization / 1.4 文档结构

**English**  
The document is arranged from general to specific. It begins with project context and architecture, then explains the Raspberry Pi and Unity implementation, followed by setup, operation, testing, troubleshooting, and future recommendations.

**中文**  
本文件按照由整体到细节的方式组织，先介绍项目背景与系统架构，再说明树莓派与 Unity 实现，之后给出配置、操作、测试、故障排查和后续建议。

---

## 2. Project Overview
## 2. 项目概述

### 2.1 Project Background / 2.1 项目背景

**English**  
This project was developed as a mixed reality visualization prototype for river water quality exploration. It aims to provide an immersive and interactive way to explore river-related water quality information using a spatial map, chart-based data display, and physical control devices.

**中文**  
本项目作为一个河流水质探索的混合现实可视化原型进行开发，旨在结合空间地图、图表数据展示以及实体控制装置，为用户提供更沉浸和更具交互性的河流水质信息探索方式。

### 2.2 Project Objectives / 2.2 项目目标

**English**  
The main objectives are:
- visualize river water quality data in mixed reality
- support physical interaction through Raspberry Pi-based controls
- enable map style switching and river selection
- support class, dataset, date, and parameter filtering
- display charts for interpretation of water quality records

**中文**  
本项目的主要目标包括：
- 在混合现实环境中可视化河流水质数据
- 通过基于树莓派的控制器实现实体交互
- 支持地图样式切换和河流选择
- 支持水质等级、数据集、日期和参数筛选
- 通过图表展示水质记录，辅助理解与分析

### 2.3 Internship Scope / 2.3 实习工作范围

**English**  
The internship focused on a working prototype rather than a production deployment. Implemented scope includes Unity scene interaction, Raspberry Pi controller integration, UDP communication, river selection logic, filtering interface, and chart-based visualization.

**中文**  
本次实习工作聚焦于可运行原型，而非正式生产部署。已完成范围包括 Unity 场景交互、树莓派控制器集成、UDP 通信、河流选择逻辑、筛选界面和图表可视化。

### 2.4 Final Deliverables / 2.4 最终交付物

**English**  
Expected technical deliverables include:
- Unity mixed reality prototype
- Raspberry Pi Zero physical controller integration
- project scripts and scene logic
- architecture diagrams and screenshots
- technical documentation for handover

**中文**  
预期技术交付物包括：
- Unity 混合现实原型
- 树莓派 Zero 实体控制器集成
- 项目脚本与场景逻辑
- 架构图和截图资料
- 用于交接的技术文档

---

## 3. System Overview
## 3. 系统总览

### 3.1 System Summary / 3.1 系统简介

**English**  
The system is a Unity-based mixed reality application designed for Meta Quest Pro. It visualizes river water quality information using map-based selection, menu-driven filtering, and chart rendering. Physical inputs are captured on Raspberry Pi Zero and transmitted to Unity over UDP.

**中文**  
本系统是一个面向 Meta Quest Pro 的 Unity 混合现实应用。它通过地图选择、菜单筛选和图表渲染来展示河流水质信息。实体输入由 Raspberry Pi Zero 采集，并通过 UDP 传输到 Unity。

### 3.2 Main Functionalities / 3.2 主要功能

**English**  
The current prototype supports:
- map style selection
- river hover and confirmation
- class selection
- existing and prediction dataset switching
- date filtering and duration selection
- parameter category and parameter selection
- class bar chart rendering
- parameter line chart rendering
- physical control and hand interaction support

**中文**  
当前原型支持以下功能：
- 地图样式切换
- 河流悬停与确认
- 水质等级选择
- existing 与 prediction 数据集切换
- 日期筛选与持续时间选择
- 参数分类与参数选择
- 水质等级柱状图渲染
- 参数折线图渲染
- 实体控制与手部交互支持

### 3.3 Core Interaction Path / 3.3 核心交互路径

**English**  
The current interaction path is `mapstyle -> river -> ri -> data`. After a river is confirmed, the system keeps the current map and data panels visible so the user can continue filtering without leaving the selected context.

**中文**  
当前交互路径为 `mapstyle -> river -> ri -> data`。在用户确认河流后，系统会保持当前地图和数据面板可见，使用户无需离开当前上下文即可继续筛选数据。

### 3.4 System Boundary / 3.4 系统边界

**English**  
The prototype is not yet a production-ready system. It is limited by the currently available dataset structure, existing Unity scene implementation, current physical wiring setup, and the implemented menu and chart logic.

**中文**  
该原型尚不是一个可直接用于生产环境的系统。其边界受限于当前可用的数据集结构、现有 Unity 场景实现、当前实体接线方案以及已实现的菜单和图表逻辑。

---

## 4. System Architecture
## 4. 系统架构

### 4.1 Overall Architecture / 4.1 总体架构

**English**  
The project uses a hybrid architecture consisting of a mixed reality application layer, a physical control layer, and a data layer.

High-level structure:
- Meta Quest Pro runs the Unity application
- Raspberry Pi Zero reads physical control values
- MCP3008 converts analog input values for Raspberry Pi processing
- UDP sends control messages to Unity
- Unity updates UI state, selection state, and chart state
- CSV files provide water quality records for chart generation

**中文**  
本项目采用混合架构，由混合现实应用层、实体控制层和数据层组成。

高层结构如下：
- Meta Quest Pro 运行 Unity 应用
- Raspberry Pi Zero 读取实体控制输入
- MCP3008 将模拟输入转换为树莓派可处理的数字值
- UDP 将控制消息发送到 Unity
- Unity 更新界面状态、选择状态和图表状态
- CSV 文件为图表生成提供水质记录数据

### 4.2 Data Flow Overview / 4.2 数据流概览

**English**  
The general data flow is:
1. User interacts with physical controller or MR interface.
2. Raspberry Pi reads input values.
3. MCP3008 converts analog values where needed.
4. Python script formats messages.
5. UDP sends the messages to Unity.
6. `PiSystemBridge.cs` interprets the messages.
7. `WaterSystemManager.cs` applies filters and updates charts.

**中文**  
系统的一般数据流如下：
1. 用户操作实体控制器或 MR 界面。
2. 树莓派读取输入值。
3. 在需要时由 MCP3008 完成模拟量转换。
4. Python 脚本整理并生成控制消息。
5. 通过 UDP 将消息发送到 Unity。
6. `PiSystemBridge.cs` 解释收到的消息。
7. `WaterSystemManager.cs` 应用筛选逻辑并更新图表。

### 4.3 Communication Flow / 4.3 通信流程

**English**  
Unity listens for UDP commands on port `5005`. Messages are received in `PiSystemBridge.cs` and translated into scene navigation, river confirmation, menu interaction, and slider-based river selection behavior.

**中文**  
Unity 在端口 `5005` 上监听 UDP 指令。消息在 `PiSystemBridge.cs` 中被接收，并转换为场景导航、河流确认、菜单交互以及基于滑杆的河流选择行为。

---

## 5. Raspberry Pi and Hardware Documentation
## 5. 树莓派与硬件文档

### 5.1 Hardware Components / 5.1 硬件组成

| Hardware | Role |
| --- | --- |
| Meta Quest Pro | Runs the mixed reality application |
| Raspberry Pi Zero | Reads physical input and sends UDP control messages |
| MCP3008 | Converts analog input values into digital values |
| Joystick | Provides directional control and confirmation press |
| Horizontal slider | Provides horizontal analog control |
| Vertical slider | Provides vertical analog control |
| Rotary encoders | Provide stepping and button interaction |
| Breadboard and jumper wires | Physical wiring and signal routing |

| 硬件 | 作用 |
| --- | --- |
| Meta Quest Pro | 运行混合现实应用 |
| Raspberry Pi Zero | 读取实体输入并发送 UDP 控制消息 |
| MCP3008 | 将模拟量转换为数字量 |
| 摇杆 | 提供方向控制和确认按下 |
| 横向滑杆 | 提供横向模拟控制 |
| 纵向滑杆 | 提供纵向模拟控制 |
| 旋钮编码器 | 提供步进旋转和按钮交互 |
| 面包板与跳线 | 用于实体接线与信号转接 |

### 5.2 Raspberry Pi Zero Overview / 5.2 树莓派 Zero 概述

**English**  
The Raspberry Pi Zero acts as the hardware input controller of the system. It reads joystick, slider, and rotary encoder inputs, processes them in Python, and sends control messages to Unity through UDP.

**中文**  
树莓派 Zero 在本系统中充当硬件输入控制器。它负责读取摇杆、滑杆和旋钮编码器的输入，在 Python 中完成处理后，再通过 UDP 将控制消息发送到 Unity。

### 5.3 Raspberry Pi Access Method / 5.3 树莓派访问方式

**English**  
In this project, commands were entered from **Windows PowerShell** on the PC. PowerShell was used as the access platform to reach the Raspberry Pi remotely through SSH. After running `ssh xlab@192.168.50.38`, the command session was no longer acting on Windows directly. Instead, the user was working inside a remote Raspberry Pi shell through the SSH connection.

This means the workflow has two layers:
- **Access platform:** Windows PowerShell on the PC
- **Actual execution environment:** Raspberry Pi OS after SSH login

Therefore, commands such as `dir`, `nano pi_input_sender.py`, and `python3 pi_input_sender.py` were typed in a PowerShell window, but they were executed remotely on the Raspberry Pi through the SSH session.

**中文**  
在本项目中，所有命令最初都是在电脑端的 **Windows PowerShell** 中输入的。PowerShell 作为访问平台，用于通过 SSH 远程连接树莓派。在执行 `ssh xlab@192.168.50.38` 之后，命令会话就不再直接作用于 Windows，而是进入树莓派的远程 Shell 环境。

也就是说，整个流程分为两层：
- **访问平台：** 电脑上的 Windows PowerShell
- **实际执行环境：** SSH 登录后的 Raspberry Pi OS

因此，像 `dir`、`nano pi_input_sender.py` 和 `python3 pi_input_sender.py` 这些命令，虽然是在 PowerShell 窗口里输入的，但它们实际上是通过 SSH 会话在树莓派上执行的。

### 5.4 Software Used on Raspberry Pi / 5.4 树莓派所用软件

**English**  
The Raspberry Pi side uses Raspberry Pi OS and Python 3 as the actual runtime environment. The main Python libraries used are:
- `gpiozero` for digital input reading
- `spidev` for reading analog values through MCP3008 via SPI
- `socket` for UDP communication
- `threading` for parallel input loops

The final integrated runtime script is `pi_input_sender.py`. In other words, Windows PowerShell was the command-entry interface, while Raspberry Pi OS and Python 3 were the actual execution environment for the hardware-side logic.

**中文**  
树莓派端真正的运行环境是 Raspberry Pi OS 和 Python 3。项目中主要使用以下 Python 库：
- `gpiozero`：读取数字输入
- `spidev`：通过 SPI 与 MCP3008 通信并读取模拟量
- `socket`：实现 UDP 通信
- `threading`：并行运行多个输入读取循环

最终整合使用的主脚本为 `pi_input_sender.py`。也就是说，Windows PowerShell 是输入命令的界面，而 Raspberry Pi OS 与 Python 3 才是硬件逻辑真正执行的环境。

### 5.5 Reading Physical Input / 5.5 物理输入读取方式

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

### 5.6 MCP3008 Analog Conversion / 5.6 MCP3008 模拟量转换

**English**  
Raspberry Pi cannot directly read analog voltage. Therefore, the project uses MCP3008, an analog-to-digital converter (ADC), to convert slider and joystick analog signals into digital values. These values are then read through SPI using the `spidev` library.

Channel mapping:
- `CH1`: horizontal slider
- `CH3`: vertical slider
- `CH5`: joystick left-right axis
- `CH6`: joystick up-down axis

**中文**  
树莓派本身不能直接读取模拟电压，因此本项目使用 MCP3008 模拟数字转换器（ADC），将滑杆和摇杆输出的模拟信号转换为数字值。转换后的数据通过 SPI 接口，由 `spidev` 库读取。

通道映射如下：
- `CH1`：横向滑杆
- `CH3`：纵向滑杆
- `CH5`：摇杆左右方向
- `CH6`：摇杆上下方向

### 5.7 UDP Communication to Unity / 5.7 发送到 Unity 的 UDP 通信

**English**  
UDP communication on the Raspberry Pi is implemented using Python's `socket` library. On the Unity side, UDP messages are received by `PiSystemBridge.cs` through `UdpClient`. The communication port used in this project is `5005`.

Example messages include:
- `JOY:LEFT`
- `JOY:RIGHT`
- `JOY:UP`
- `JOY:DOWN`
- `JOYBTN`
- `BTN3`
- `SLIDER1:<value>`
- `SLIDER2:<value>`
- `ENC1:1` or `ENC1:-1`
- `ENC2:1` or `ENC2:-1`
- `ENC3:1` or `ENC3:-1`

**中文**  
树莓派端的 UDP 通信由 Python 的 `socket` 库实现。Unity 端通过 `PiSystemBridge.cs` 中的 `UdpClient` 接收 UDP 消息。本项目使用的通信端口为 `5005`。

消息示例如下：
- `JOY:LEFT`
- `JOY:RIGHT`
- `JOY:UP`
- `JOY:DOWN`
- `JOYBTN`
- `BTN3`
- `SLIDER1:<value>`
- `SLIDER2:<value>`
- `ENC1:1` 或 `ENC1:-1`
- `ENC2:1` 或 `ENC2:-1`
- `ENC3:1` 或 `ENC3:-1`

### 5.8 Detailed Wiring Procedure / 5.8 详细插线步骤

#### 5.8.1 Joystick / 5.8.1 摇杆

**English**  
The joystick module has five main pins: `VCC`, `GND`, `VRx`, `VRy`, and `SW`.

Wiring steps:
1. Connect joystick `VCC` to the positive rail.
2. Connect the positive rail to Raspberry Pi `3.3V`.
3. Connect joystick `GND` to the ground rail.
4. Connect the ground rail to Raspberry Pi `GND`.
5. Connect `VRx` to breadboard position `g37`, then to `MCP3008 CH5`.
6. Connect `VRy` to breadboard position `g36`, then to `MCP3008 CH6`.
7. Connect `SW` directly to Raspberry Pi `GPIO26` (physical pin `37`).

Verification:
- move left-right -> `CH5` changes
- move up-down -> `CH6` changes
- press joystick -> terminal prints `JOYBTN`

**中文**  
摇杆模块共有五个主要引脚：`VCC`、`GND`、`VRx`、`VRy` 和 `SW`。

接线步骤：
1. 将摇杆 `VCC` 连接到正电源轨。
2. 将正电源轨连接到树莓派 `3.3V`。
3. 将摇杆 `GND` 连接到负电源轨。
4. 将负电源轨连接到树莓派 `GND`。
5. 将 `VRx` 接到面包板位置 `g37`，再接到 `MCP3008 CH5`。
6. 将 `VRy` 接到面包板位置 `g36`，再接到 `MCP3008 CH6`。
7. 将 `SW` 直接连接到树莓派 `GPIO26`（物理引脚 `37`）。

验证方式：
- 左右移动 -> `CH5` 变化
- 上下移动 -> `CH6` 变化
- 按下摇杆 -> 终端输出 `JOYBTN`

#### 5.8.2 Horizontal Slider / 5.8.2 横向滑杆

**English**  
The horizontal slider has three main connections: `VCC`, `GND`, and `OUT`.

Wiring steps:
1. Connect slider `VCC` to the positive rail.
2. Connect the positive rail to Raspberry Pi `3.3V`.
3. Connect slider `GND` to the ground rail.
4. Connect the ground rail to Raspberry Pi `GND`.
5. Connect slider `OUT` to breadboard position `g41`.
6. Connect that line to `MCP3008 CH1`.

Verification:
- moving the slider changes `CH1`
- main script outputs `SLIDER1:<value>`

**中文**  
横向滑杆共有三个主要连接点：`VCC`、`GND` 和 `OUT`。

接线步骤：
1. 将滑杆 `VCC` 连接到正电源轨。
2. 将正电源轨连接到树莓派 `3.3V`。
3. 将滑杆 `GND` 连接到负电源轨。
4. 将负电源轨连接到树莓派 `GND`。
5. 将滑杆 `OUT` 接到面包板位置 `g41`。
6. 将该行连接到 `MCP3008 CH1`。

验证方式：
- 滑杆移动时 `CH1` 变化
- 主脚本输出 `SLIDER1:<value>`

#### 5.8.3 Vertical Slider / 5.8.3 纵向滑杆

**English**  
The vertical slider also has `VCC`, `GND`, and `OUT`.

Wiring steps:
1. Connect slider `VCC` to the positive rail.
2. Connect the positive rail to Raspberry Pi `3.3V`.
3. Connect slider `GND` to the ground rail.
4. Connect the ground rail to Raspberry Pi `GND`.
5. Connect slider `OUT` to breadboard position `g39`.
6. Connect that line to `MCP3008 CH3`.

Verification:
- moving the slider changes `CH3`
- main script outputs `SLIDER2:<value>`

**中文**  
纵向滑杆同样有 `VCC`、`GND` 和 `OUT`。

接线步骤：
1. 将滑杆 `VCC` 连接到正电源轨。
2. 将正电源轨连接到树莓派 `3.3V`。
3. 将滑杆 `GND` 连接到负电源轨。
4. 将负电源轨连接到树莓派 `GND`。
5. 将滑杆 `OUT` 接到面包板位置 `g39`。
6. 将该行连接到 `MCP3008 CH3`。

验证方式：
- 滑杆移动时 `CH3` 变化
- 主脚本输出 `SLIDER2:<value>`

#### 5.8.4 Rotary Encoders / 5.8.4 旋钮编码器

**English**  
Encoder 1 wiring:
- `VCC -> 5V + rail row 1`
- `GND -> 5V - rail row 2`
- `S1 -> GPIO17`
- `S2 -> GPIO22`
- `KEY -> GPIO27`

Encoder 2 wiring:
- `VCC -> 5V + rail row 13`
- `GND -> 5V - rail row 10`
- `S1 -> GPIO18`
- `S2 -> GPIO23`
- `KEY -> GPIO25`

Encoder 3 wiring:
- `VCC -> 5V + rail row 22`
- `GND -> 5V - rail row 19`
- `S1 -> GPIO24`
- `S2 -> GPIO16`
- `KEY -> GPIO20`

Verification:
- rotation outputs `ENCx:1` or `ENCx:-1`
- button press outputs `BTNx`

For encoder 3 specifically:
- rotating the encoder changes the map scale when encoder 3 is in zoom mode
- pressing the encoder sends `BTN3` and toggles encoder 3 between **Zoom mode** and **Rotate mode**
- after switching to rotate mode, rotating encoder 3 rotates the map horizontally instead of zooming

**中文**  
旋钮 1 接线：
- `VCC -> 5V 正电源轨 row 1`
- `GND -> 5V 负电源轨 row 2`
- `S1 -> GPIO17`
- `S2 -> GPIO22`
- `KEY -> GPIO27`

旋钮 2 接线：
- `VCC -> 5V 正电源轨 row 13`
- `GND -> 5V 负电源轨 row 10`
- `S1 -> GPIO18`
- `S2 -> GPIO23`
- `KEY -> GPIO25`

旋钮 3 接线：
- `VCC -> 5V 正电源轨 row 22`
- `GND -> 5V 负电源轨 row 19`
- `S1 -> GPIO24`
- `S2 -> GPIO16`
- `KEY -> GPIO20`

验证方式：
- 旋转输出 `ENCx:1` 或 `ENCx:-1`
- 按下输出 `BTNx`

对于旋钮 3，还需要特别说明：
- 当旋钮 3 处于缩放模式时，旋转会改变地图缩放比例
- 按下旋钮 3 会发送 `BTN3`，并在**缩放模式**与**旋转模式**之间切换
- 切换到旋转模式后，再旋转旋钮 3 时，地图会进行左右方向旋转，而不是继续缩放

### 5.9 Wiring Verification Strategy / 5.9 接线验证策略

**English**  
Because the breadboard wiring is dense and difficult to follow visually, the final mapping was not validated by photographs alone. It was confirmed using:
1. breadboard position notes,
2. GPIO and ADC diagnostic scripts,
3. runtime terminal output from the main Raspberry Pi script.

**中文**  
由于面包板接线密集且不易通过肉眼完整追踪，最终映射并不是只靠照片确认的，而是通过以下方式综合验证：
1. 面包板位置记录，
2. GPIO 与 ADC 诊断脚本，
3. 主树莓派脚本运行时的终端输出。

---

## 6. Unity and Software Documentation
## 6. Unity 与软件文档

### 6.1 Development Environment / 6.1 开发环境

**English**  
The implementation environment includes Unity `6000.0.62f1`, C# scripting, Android/XR build workflow, and supporting packages such as TextMeshPro and Unity UI.

**中文**  
实现环境包括 Unity `6000.0.62f1`、C# 脚本、Android/XR 构建流程，以及 TextMeshPro、Unity UI 等支持包。

### 6.2 Key Project Files / 6.2 关键项目文件

| File or Folder | Purpose |
| --- | --- |
| `Assets/310.unity` | Main scene used in build settings |
| `Assets/MyScripts/PiSystemBridge.cs` | Receives UDP commands and maps them to Unity actions |
| `Assets/MyScripts/WaterSystemManager.cs` | Manages filtering logic and chart refresh |
| `Assets/MyScripts/mapswitch.cs` | Handles map style switching |
| `Assets/Resources/WaterData/` | Stores CSV-based water quality data |
| `Packages/manifest.json` | Unity package configuration |
| `ProjectSettings/ProjectVersion.txt` | Unity version record |

| 文件或目录 | 作用 |
| --- | --- |
| `Assets/310.unity` | 构建设置中的主场景 |
| `Assets/MyScripts/PiSystemBridge.cs` | 接收 UDP 指令并映射为 Unity 动作 |
| `Assets/MyScripts/WaterSystemManager.cs` | 管理筛选逻辑与图表刷新 |
| `Assets/MyScripts/mapswitch.cs` | 处理地图样式切换 |
| `Assets/Resources/WaterData/` | 存放 CSV 水质数据 |
| `Packages/manifest.json` | Unity 包配置 |
| `ProjectSettings/ProjectVersion.txt` | Unity 版本记录 |

### 6.3 PiSystemBridge.cs / 6.3 PiSystemBridge.cs 说明

**English**  
`PiSystemBridge.cs` is the communication bridge between Raspberry Pi and Unity. It listens on UDP port `5005`, receives messages such as `JOYBTN`, `JOY:UP`, `SLIDER1:<value>`, `BTN3`, and encoder commands, then maps them to menu actions, map behavior, zoom behavior, or rotation behavior. In the current implementation, `BTN3` toggles encoder 3 between zoom mode and rotate mode.

**中文**  
`PiSystemBridge.cs` 是树莓派与 Unity 之间的通信桥接模块。它在 UDP 端口 `5005` 上监听，接收 `JOYBTN`、`JOY:UP`、`SLIDER1:<value>`、`BTN3` 以及旋钮指令等消息，并将其映射为菜单动作、地图行为、缩放行为或旋转行为。在当前实现中，`BTN3` 用于在旋钮 3 的缩放模式与旋转模式之间切换。

### 6.4 WaterSystemManager.cs / 6.4 WaterSystemManager.cs 说明

**English**  
`WaterSystemManager.cs` manages selected river state, class toggles, dataset toggles, date filtering, parameter filtering, and chart refresh behavior.

**中文**  
`WaterSystemManager.cs` 负责管理已选河流状态、水质等级切换、数据集切换、日期筛选、参数筛选以及图表刷新行为。

### 6.5 Unity Hand Interaction / 6.5 Unity 手部交互

**English**  
The project also includes hand interaction support inside Unity for mixed reality runtime, but the current documentation prioritizes the physical Raspberry Pi control path because that is the main handover requirement.

**中文**  
本项目在 Unity 中也支持混合现实运行时的手部交互，但当前文档优先说明基于树莓派的实体控制路径，因为这是当前交接的重点要求。

---

## 7. Data and Communication Documentation
## 7. 数据与通信文档

### 7.1 Data Source Description / 7.1 数据源说明

**English**  
Water quality data is stored in CSV files located under the Unity resources folder. The application loads and filters these datasets for class charts and parameter line charts.

**中文**  
水质数据以 CSV 文件形式存储在 Unity 的资源目录中。应用会加载并筛选这些数据集，以生成水质等级柱状图和参数折线图。

### 7.2 Existing and Prediction Dataset Handling / 7.2 Existing 与 Prediction 数据处理

**English**  
The current UI supports switching between existing and prediction datasets. This switching is controlled in Unity by the state management logic inside `WaterSystemManager.cs`.

**中文**  
当前界面支持在 existing 与 prediction 数据集之间切换，该切换由 `WaterSystemManager.cs` 中的状态管理逻辑控制。

### 7.3 Message Mapping / 7.3 消息映射

| Raspberry Pi Message | Unity Meaning |
| --- | --- |
| `JOY:LEFT/RIGHT/UP/DOWN` | directional navigation |
| `JOYBTN` | confirmation action |
| `BTN3` | toggle encoder 3 mode between zoom and rotate |
| `SLIDER1:<value>` | horizontal slider update |
| `SLIDER2:<value>` | vertical slider update |
| `ENC1:*` | encoder 1 rotation input |
| `ENC2:*` | menu stepping |
| `ENC3:*` | zoom stepping or horizontal map rotation depending on encoder 3 mode |

| 树莓派消息 | Unity 含义 |
| --- | --- |
| `JOY:LEFT/RIGHT/UP/DOWN` | 方向导航 |
| `JOYBTN` | 确认动作 |
| `BTN3` | 切换旋钮 3 的缩放模式与旋转模式 |
| `SLIDER1:<value>` | 横向滑杆更新 |
| `SLIDER2:<value>` | 纵向滑杆更新 |
| `ENC1:*` | 旋钮 1 旋转输入 |
| `ENC2:*` | 菜单步进 |
| `ENC3:*` | 根据旋钮 3 当前模式执行缩放步进或地图左右旋转 |

---

## 8. Installation and Deployment Guide
## 8. 安装与部署指南

### 8.1 System Requirements / 8.1 系统要求

**English**  
Minimum setup requirements include a Windows PC with Unity installed, Meta Quest Pro, Raspberry Pi Zero, MCP3008, the physical control hardware, and access to the local network used for UDP communication.

**中文**  
最低配置要求包括：安装了 Unity 的 Windows 电脑、Meta Quest Pro、Raspberry Pi Zero、MCP3008、实体控制硬件，以及用于 UDP 通信的局域网环境。

### 8.2 Raspberry Pi Setup Procedure / 8.2 树莓派配置流程

**English**  
Recommended setup procedure:
1. Power on the Raspberry Pi and confirm it is reachable locally.
2. Open Windows PowerShell.
3. Run `arp -a` to identify the Raspberry Pi IP address.
4. Connect using `ssh xlab@192.168.50.38`.
5. After login, run `dir` to confirm that `pi_input_sender.py` is present.
6. If needed, edit the script with `nano pi_input_sender.py`.
7. Update `UNITY_IP` and `UNITY_PORT` if necessary.
8. Ensure SPI is enabled on the Raspberry Pi by running `sudo raspi-config`, then go to `Interface Options -> SPI -> Enable`.
9. Reboot the Raspberry Pi if SPI was just enabled.
10. Reconnect through SSH and run `python3 pi_input_sender.py`.
11. Watch for outgoing message activity in the Raspberry Pi terminal.

The commands are entered in a Windows PowerShell window, but after SSH login they are executed remotely in the Raspberry Pi environment. Google Colab is not used in this project.

**中文**  
推荐配置流程如下：
1. 打开树莓派电源并确认其可在局域网中访问。
2. 打开 Windows PowerShell。
3. 输入 `arp -a` 查找树莓派 IP。
4. 使用 `ssh xlab@192.168.50.38` 连接树莓派。
5. 登录后输入 `dir`，确认 `pi_input_sender.py` 存在。
6. 如有需要，使用 `nano pi_input_sender.py` 修改脚本。
7. 根据需要更新 `UNITY_IP` 和 `UNITY_PORT`。
8. 通过 `sudo raspi-config` 确认树莓派已启用 SPI，然后进入 `Interface Options -> SPI -> Enable`。
9. 如果刚启用 SPI，则重启树莓派。
10. 重新通过 SSH 登录后，输入 `python3 pi_input_sender.py` 运行脚本。
11. 在树莓派终端观察是否有消息持续输出。

命令是在 Windows PowerShell 窗口中输入的，但在 SSH 登录后，它们实际上是在树莓派的远程环境中执行的。本项目不使用 Google Colab。

### 8.3 Unity Setup Procedure / 8.3 Unity 配置流程

**English**  
On the Unity side:
1. Open the Unity project.
2. Open the main scene `Assets/310.unity`.
3. Confirm the scene contains the object with `PiSystemBridge.cs`.
4. Confirm the listen port is `5005`.
5. Press Play and watch the Console for incoming messages.

**中文**  
Unity 端流程如下：
1. 打开 Unity 项目。
2. 打开主场景 `Assets/310.unity`。
3. 确认场景中存在挂载 `PiSystemBridge.cs` 的对象。
4. 确认监听端口为 `5005`。
5. 点击 Play，并在 Console 中观察是否收到消息。

---

## 9. Operation Manual
## 9. 操作手册

### 9.1 Startup Procedure / 9.1 启动流程

**English**  
Normal runtime startup order:
1. Power on Raspberry Pi.
2. Confirm network connection.
3. Use Windows PowerShell to connect through SSH.
4. Run `pi_input_sender.py` in the Raspberry Pi remote shell.
5. Open Unity project and scene.
6. Press Play in Unity.
7. Verify message activity and scene behavior.

**中文**  
正常运行时的启动顺序如下：
1. 打开树莓派电源。
2. 确认网络连接正常。
3. 通过 Windows PowerShell 使用 SSH 连接树莓派。
4. 在树莓派远程 Shell 中运行 `pi_input_sender.py`。
5. 打开 Unity 项目和场景。
6. 在 Unity 中点击 Play。
7. 确认有消息输出且场景行为正常。

### 9.2 User Operation Flow / 9.2 用户操作流程

**English**  
The user first selects map style, then selects a river, then performs RI and data filtering. Sliders, joystick, and encoders are used to navigate and confirm actions.

**中文**  
用户首先选择地图样式，然后选择河流，接着进行 RI 和数据筛选。滑杆、摇杆和旋钮用于导航与确认操作。

---

## 10. Testing and Validation
## 10. 测试与验证

### 10.1 Testing Objectives / 10.1 测试目标

**English**  
Testing focused on verifying physical input mapping, UDP communication, Unity message handling, and chart update behavior.

**中文**  
测试重点包括验证实体输入映射、UDP 通信、Unity 消息处理以及图表更新行为。

### 10.2 Lab Validation Evidence / 10.2 实验室验证证据

**English**  
The Raspberry Pi to Unity integration was validated in the lab environment. Evidence includes:
- verified GPIO and ADC mappings
- runtime terminal output such as `JOY:UP`, `JOY:DOWN`, `JOY:LEFT`, `JOY:RIGHT`, and `JOYBTN`
- matching sender-side Python logic and receiver-side Unity logic
- archived scripts and handover notes stored in GitHub

**中文**  
树莓派到 Unity 的集成已经在实验室环境中完成验证。证据包括：
- 已确认的 GPIO 与 ADC 映射
- 运行终端输出，例如 `JOY:UP`、`JOY:DOWN`、`JOY:LEFT`、`JOY:RIGHT` 和 `JOYBTN`
- 树莓派发送端 Python 逻辑与 Unity 接收端逻辑的对应关系
- 已保存到 GitHub 的脚本与交接资料

### 10.3 Test Scripts Used / 10.3 使用的测试脚本

| Script | Purpose |
| --- | --- |
| `watch_gpio.py` | identify GPIO pin changes |
| `mcp_read_all.py` | identify ADC channel changes |
| `joy_click_test.py` | verify joystick movement and press |
| `udp_test_send.py` | verify UDP path |

| 脚本 | 用途 |
| --- | --- |
| `watch_gpio.py` | 识别 GPIO 变化 |
| `mcp_read_all.py` | 识别 ADC 通道变化 |
| `joy_click_test.py` | 验证摇杆移动和按下 |
| `udp_test_send.py` | 验证 UDP 通路 |

---

## 11. Troubleshooting
## 11. 故障排查

### 11.1 Troubleshooting Platforms / 11.1 排查平台

**English**  
Troubleshooting is mainly performed on three platforms:
- Raspberry Pi terminal
- Windows PowerShell
- Unity Console

**中文**  
故障排查主要在三个平台进行：
- 树莓派终端
- Windows PowerShell
- Unity Console

### 11.2 Troubleshooting Table / 11.2 排查表

| Problem | Platform | Tool or Command | Suggested Check |
| --- | --- | --- | --- |
| Cannot find Raspberry Pi IP | Windows PowerShell | `arp -a` | confirm local network entry |
| Cannot connect by SSH | Windows PowerShell | `ssh xlab@<ip>` | confirm power, IP, and network |
| No joystick or encoder response | Raspberry Pi terminal | `watch_gpio.py` | confirm digital signal changes |
| No analog response | Raspberry Pi terminal | `mcp_read_all.py` | confirm ADC channel changes |
| No joystick press detection | Raspberry Pi terminal | `joy_click_test.py` | confirm `JOYBTN` or click value |
| Unity receives nothing | Unity Console | Console logs | confirm port `5005` and message output |

| 问题 | 平台 | 工具或命令 | 建议检查项 |
| --- | --- | --- | --- |
| 找不到树莓派 IP | Windows PowerShell | `arp -a` | 确认局域网设备条目 |
| 无法 SSH 连接 | Windows PowerShell | `ssh xlab@<ip>` | 确认电源、IP 和网络 |
| 摇杆或旋钮无反应 | 树莓派终端 | `watch_gpio.py` | 确认数字信号变化 |
| 模拟输入无反应 | 树莓派终端 | `mcp_read_all.py` | 确认 ADC 通道变化 |
| 摇杆按下无检测 | 树莓派终端 | `joy_click_test.py` | 确认 `JOYBTN` 或点击计数 |
| Unity 没有收到消息 | Unity Console | Console 日志 | 确认端口 `5005` 和消息输出 |

---

## 12. Known Issues and Limitations
## 12. 已知问题与限制

**English**  
Current limitations include:
- dense breadboard wiring makes visual tracing difficult
- system depends on stable local network communication
- analog input may require threshold tuning
- some final physical pin references should still be rechecked if a full hardware redraw is required

**中文**  
当前限制包括：
- 面包板接线密集，难以通过肉眼完整追踪
- 系统依赖稳定的局域网通信
- 模拟输入可能需要阈值调节
- 若后续需要严格的硬件重绘图，部分最终物理引脚仍建议再次核对

---

## 13. Maintenance and Future Recommendations
## 13. 维护与后续建议

### 13.1 Maintenance Notes / 13.1 维护说明

**English**  
For future maintenance, the most important files to preserve are the Unity scene, `PiSystemBridge.cs`, `WaterSystemManager.cs`, `pi_input_sender.py`, and the verified wiring notes archived in the GitHub handover repository.

**中文**  
对于后续维护，最需要保留的文件包括 Unity 场景、`PiSystemBridge.cs`、`WaterSystemManager.cs`、`pi_input_sender.py`，以及已归档到 GitHub 交接仓库中的接线记录。

### 13.2 Suggested Future Enhancements / 13.2 后续改进建议

**English**  
Suggested future improvements include:
- cleaner and labeled hardware enclosure
- clearer wiring diagrams
- more stable network configuration
- expanded test evidence including screenshots and videos
- more complete hand interaction documentation

**中文**  
建议的后续改进方向包括：
- 更整洁并带标签的硬件外壳
- 更清晰的接线图
- 更稳定的网络配置
- 补充截图和视频形式的测试证据
- 更完整的手部交互说明

---

## Appendices
## 附录

### Appendix A. Related GitHub Handover Repository / 附录A 相关 GitHub 交接仓库

Repository: `https://github.com/77Xiaoya/project-handover`

This repository contains:
- Raspberry Pi scripts
- joystick, slider, and encoder wiring notes
- bilingual Raspberry Pi documentation draft
- Unity bridge script copy and handover notes

该仓库包含：
- 树莓派脚本
- 摇杆、滑杆和旋钮接线记录
- 双语树莓派文档草稿
- Unity 桥接脚本副本和交接说明

### Appendix B. Key Command Reference / 附录B 关键命令参考

| Action | Command |
| --- | --- |
| Find Raspberry Pi IP | `arp -a` |
| Connect to Raspberry Pi | `ssh xlab@192.168.50.38` |
| Check files on Raspberry Pi | `dir` |
| Edit main script | `nano pi_input_sender.py` |
| Run main script | `python3 pi_input_sender.py` |

| 操作 | 命令 |
| --- | --- |
| 查找树莓派 IP | `arp -a` |
| 连接树莓派 | `ssh xlab@192.168.50.38` |
| 查看树莓派文件 | `dir` |
| 编辑主脚本 | `nano pi_input_sender.py` |
| 运行主脚本 | `python3 pi_input_sender.py` |
