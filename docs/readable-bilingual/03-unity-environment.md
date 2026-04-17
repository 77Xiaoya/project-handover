# 03 Unity Environment / 03 Unity 环境重建

## Unity Version / Unity 版本

**English**  
The project should be opened with Unity `6000.0.62f1`.

**中文**  
项目应使用 Unity `6000.0.62f1` 打开。

## Meta Quest and MR Environment / Meta Quest 与 MR 环境

**English**  
The current Unity environment is configured for Meta Quest Pro mixed reality. The project depends on Meta XR and Unity XR packages, so environment rebuilding should begin with package compatibility rather than script editing.

Important packages include:
- `com.meta.xr.sdk.all`
- `com.meta.xr.sdk.interaction`
- `com.unity.xr.openxr`
- `com.unity.xr.meta-openxr`
- `com.unity.xr.androidxr-openxr`
- `com.unity.xr.arfoundation`
- `com.unity.xr.hands`
- `com.unity.xr.interaction.toolkit`

**中文**  
当前 Unity 环境面向 Meta Quest Pro 混合现实配置。项目依赖 Meta XR 和 Unity XR 包，因此环境重建应优先保证包兼容性，而不是先修改脚本。

重要依赖包包括：
- `com.meta.xr.sdk.all`
- `com.meta.xr.sdk.interaction`
- `com.unity.xr.openxr`
- `com.unity.xr.meta-openxr`
- `com.unity.xr.androidxr-openxr`
- `com.unity.xr.arfoundation`
- `com.unity.xr.hands`
- `com.unity.xr.interaction.toolkit`

## Build Target / 构建目标

**English**  
The project settings indicate an Android-based XR deployment path. Unity environment reconstruction should therefore keep Android as the build target.

**中文**  
项目设置显示当前部署路径为基于 Android 的 XR 环境，因此在重建 Unity 环境时应保持 Android 作为构建目标。

## Main Scene / 主场景

**English**  
The main scene is `Assets/310.unity`.

**中文**  
主场景为 `Assets/310.unity`。

## Minimum Environment Rebuild Steps / 最小环境重建步骤

**English**
1. Open the project with Unity `6000.0.62f1`.
2. Wait for package resolution to complete.
3. Confirm Meta XR and Unity XR packages are installed.
4. Keep Android as the build target.
5. Open `Assets/310.unity`.

**中文**
1. 使用 Unity `6000.0.62f1` 打开项目。
2. 等待依赖包解析完成。
3. 确认 Meta XR 和 Unity XR 包已安装。
4. 保持 Android 为构建目标。
5. 打开 `Assets/310.unity`。
