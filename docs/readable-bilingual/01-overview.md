# 01 Overview / 01 概述

## Project Summary / 项目简介

**English**  
This project is a mixed reality river water quality visualization prototype. It runs in Unity on Meta Quest Pro and uses Raspberry Pi Zero as a physical input bridge. The system reads joystick, slider, and encoder input, sends messages to Unity through UDP, and updates map selection, data filtering, and chart behavior.

**中文**  
本项目是一个混合现实河流水质可视化原型。系统运行在 Meta Quest Pro 的 Unity 环境中，并使用 Raspberry Pi Zero 作为实体输入桥接设备。系统读取摇杆、滑杆和旋钮输入，通过 UDP 将消息发送到 Unity，再更新地图选择、数据筛选和图表行为。

## Main Components / 主要组成

**English**
- Meta Quest Pro: runtime headset
- Unity: mixed reality application
- Raspberry Pi Zero: physical input controller
- MCP3008: analog-to-digital conversion for joystick and sliders
- CSV data: river water quality records

**中文**
- Meta Quest Pro：运行头显
- Unity：混合现实应用
- Raspberry Pi Zero：实体输入控制器
- MCP3008：用于摇杆和滑杆的模数转换
- CSV 数据：河流水质记录

## Core Interaction Flow / 核心交互流程

**English**  
The current interaction path is `mapstyle -> river -> ri -> data`.

**中文**  
当前交互路径为 `mapstyle -> river -> ri -> data`。

## Key Files / 关键文件

**English**
- `raspberry-pi-scripts/pi_input_sender.py`
- `unity/PiSystemBridge.cs`
- `Assets/MyScripts/WaterSystemManager.cs`

**中文**
- `raspberry-pi-scripts/pi_input_sender.py`
- `unity/PiSystemBridge.cs`
- `Assets/MyScripts/WaterSystemManager.cs`
