# 04 Unity Scripts and Inspector / 04 Unity 脚本与 Inspector

## Script Summary / 脚本总览

| Script | Main Purpose | Attachment or Scene Role | Must Check |
| --- | --- | --- | --- |
| `PiSystemBridge.cs` | Receives UDP messages from Raspberry Pi and maps them to Unity actions | Attached to the dedicated `PiSystemBridge` GameObject in `Assets/310.unity` | port, object references, slider ranges |
| `WaterSystemManager.cs` | Controls selected river state, filtering logic, and chart refresh | Attached to the scene object that manages river data UI and charts | toggles, dropdowns, chart references |
| `mapswitch.cs` | Switches between default, satellite, and terrain map layers | Attached to the map-switch controller object referenced by `PiSystemBridge` | three map layer references |

| 脚本 | 主要用途 | 挂载位置或场景角色 | 必须检查 |
| --- | --- | --- | --- |
| `PiSystemBridge.cs` | 接收树莓派 UDP 消息并映射为 Unity 动作 | 挂载在 `Assets/310.unity` 中名为 `PiSystemBridge` 的独立 GameObject 上 | 端口、对象引用、滑杆范围 |
| `WaterSystemManager.cs` | 控制已选河流状态、筛选逻辑和图表刷新 | 挂载在管理河流数据界面和图表的场景对象上 | toggles、dropdowns、图表引用 |
| `mapswitch.cs` | 切换默认、卫星和地形地图图层 | 挂载在被 `PiSystemBridge` 引用的地图切换控制对象上 | 三种地图图层引用 |

## Key Scripts / 关键脚本

### PiSystemBridge.cs

**English**  
`PiSystemBridge.cs` receives UDP messages from Raspberry Pi and maps them to Unity-side actions.

Important message behavior:
- `JOYBTN`: confirm action
- `JOY:LEFT/RIGHT/UP/DOWN`: move selection
- `SLIDER1/SLIDER2`: update slider state
- `BTN3`: switch encoder 3 mode
- `ENC3:*`: zoom or rotate map depending on mode

**中文**  
`PiSystemBridge.cs` 负责接收树莓派发来的 UDP 消息，并将其映射为 Unity 侧动作。

重要消息行为：
- `JOYBTN`：确认动作
- `JOY:LEFT/RIGHT/UP/DOWN`：移动选择
- `SLIDER1/SLIDER2`：更新滑杆状态
- `BTN3`：切换旋钮 3 模式
- `ENC3:*`：根据模式执行地图缩放或旋转

**Attachment / 挂载位置**

**English**  
In the current validated scene, `PiSystemBridge.cs` is attached to a dedicated GameObject named `PiSystemBridge`.

**中文**  
在当前已验证场景中，`PiSystemBridge.cs` 挂载在一个独立的 GameObject 上，其名称为 `PiSystemBridge`。

### WaterSystemManager.cs

**English**  
`WaterSystemManager.cs` manages selected river state, filtering logic, and chart refresh behavior.

**中文**  
`WaterSystemManager.cs` 负责管理已选河流状态、筛选逻辑和图表刷新行为。

**What to check / 检查重点**

**English**
- class toggle references
- data toggle references
- date dropdown references
- category and parameter toggle references
- chart references

**中文**
- class toggle 引用
- data toggle 引用
- 日期 dropdown 引用
- 参数分类和参数 toggle 引用
- 图表引用

### mapswitch.cs

**English**  
`mapswitch.cs` switches between default, satellite, and terrain map layers.

**中文**  
`mapswitch.cs` 负责在默认、卫星和地形地图图层之间切换。

**What to check / 检查重点**

**English**
- `defaultMap`
- `satelliteMap`
- `terrainMap`

**中文**
- `defaultMap`
- `satelliteMap`
- `terrainMap`

## PiSystemBridge Inspector / PiSystemBridge Inspector 检查

**English**  
In the current validated scene, the `PiSystemBridge` GameObject should have these key fields assigned:
- `listenPort = 5005`
- `mapSwitcher`
- `waterSystemManager`
- `mapRoot`
- `sliderXContainer`
- `sliderYContainer`
- `intersectionMarker`
- river highlighter references
- `classPanel`
- `dataPanel`

Key numeric values in the validated scene include:
- `minScale: 0.3`
- `maxScale: 3`
- `sliderXMin: -2`
- `sliderXMax: 200`
- `sliderYMin: -45`
- `sliderYMax: 200`
- `riverSelectionCooldown: 0.04`
- `snapRadius: 0.5`

**中文**  
在当前已验证场景中，`PiSystemBridge` GameObject 应至少正确赋值以下关键字段：
- `listenPort = 5005`
- `mapSwitcher`
- `waterSystemManager`
- `mapRoot`
- `sliderXContainer`
- `sliderYContainer`
- `intersectionMarker`
- 河流高亮引用
- `classPanel`
- `dataPanel`

当前已验证场景中的关键数值包括：
- `minScale: 0.3`
- `maxScale: 3`
- `sliderXMin: -2`
- `sliderXMax: 200`
- `sliderYMin: -45`
- `sliderYMax: 200`
- `riverSelectionCooldown: 0.04`
- `snapRadius: 0.5`

## Why Inspector Assignment Matters / 为什么 Inspector 挂载很重要

**English**  
If references are missing, Unity may still enter Play mode, but the Raspberry Pi integration will not behave correctly.

**中文**  
如果这些引用缺失，Unity 虽然可能仍然可以进入 Play 模式，但树莓派集成部分不会按预期工作。

## Minimal Teaching Order / 最小教学顺序

**English**
1. First explain what the Unity environment is for.
2. Then explain which scene should be opened.
3. Then explain which scripts are important.
4. Finally explain which Inspector fields must be checked before testing.

**中文**
1. 先解释 Unity 环境是做什么的。
2. 然后说明应该打开哪个场景。
3. 再说明哪些脚本最重要。
4. 最后说明在测试前必须检查哪些 Inspector 字段。
