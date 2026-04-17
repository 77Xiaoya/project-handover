# 04 Unity Scripts and Inspector / 04 Unity 脚本与 Inspector

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

### WaterSystemManager.cs

**English**  
`WaterSystemManager.cs` manages selected river state, filtering logic, and chart refresh behavior.

**中文**  
`WaterSystemManager.cs` 负责管理已选河流状态、筛选逻辑和图表刷新行为。

### mapswitch.cs

**English**  
`mapswitch.cs` switches between default, satellite, and terrain map layers.

**中文**  
`mapswitch.cs` 负责在默认、卫星和地形地图图层之间切换。

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
