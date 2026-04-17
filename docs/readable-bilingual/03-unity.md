# 03 Unity / 03 Unity 部分

## Environment Rebuild / 环境重建

**English**  
The Unity project should be opened with Unity `6000.0.62f1`. The current environment depends on Meta XR and Unity XR packages for Meta Quest Pro mixed reality support.

Important packages include:
- `com.meta.xr.sdk.all`
- `com.unity.xr.openxr`
- `com.unity.xr.meta-openxr`
- `com.unity.xr.androidxr-openxr`
- `com.unity.xr.arfoundation`
- `com.unity.xr.hands`
- `com.unity.xr.interaction.toolkit`

**中文**  
Unity 项目应使用 Unity `6000.0.62f1` 打开。当前环境依赖 Meta XR 和 Unity XR 包，以支持 Meta Quest Pro 混合现实运行。

重要包包括：
- `com.meta.xr.sdk.all`
- `com.unity.xr.openxr`
- `com.unity.xr.meta-openxr`
- `com.unity.xr.androidxr-openxr`
- `com.unity.xr.arfoundation`
- `com.unity.xr.hands`
- `com.unity.xr.interaction.toolkit`

## Main Scene / 主场景

**English**  
The main scene is `Assets/310.unity`.

**中文**  
主场景为 `Assets/310.unity`。

## Key Scripts / 关键脚本

**PiSystemBridge.cs**

**English**  
Receives UDP messages from Raspberry Pi and maps them to Unity actions.

**中文**  
接收来自树莓派的 UDP 消息，并将其映射为 Unity 动作。

Important message behavior:
- `JOYBTN`: confirm action
- `BTN3`: switch encoder 3 mode
- `ENC3:*`: zoom or rotate map depending on mode

**WaterSystemManager.cs**

**English**  
Controls selected river state, filtering logic, and chart refresh.

**中文**  
控制已选河流状态、筛选逻辑和图表刷新。

**mapswitch.cs**

**English**  
Controls map layer switching between default, satellite, and terrain.

**中文**  
控制默认、卫星和地形地图图层切换。

## Inspector Checks / Inspector 检查项

For `PiSystemBridge`, check:
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

对于 `PiSystemBridge`，请检查：
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
