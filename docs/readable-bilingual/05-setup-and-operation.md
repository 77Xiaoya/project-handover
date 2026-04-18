# 05 Setup and Operation / 05 配置与操作

## Raspberry Pi Setup / 树莓派配置

**English**
1. Open Windows PowerShell.
2. Run `arp -a`.
3. Connect using `ssh xlab@192.168.50.38`.
4. Confirm `pi_input_sender.py` is present.
5. Edit it if needed using `nano pi_input_sender.py`.
6. Confirm `UNITY_IP` and `UNITY_PORT`.
7. Enable SPI using `sudo raspi-config`.
8. Reboot if needed.
9. Check `ls /dev/spidev*`.
10. If needed, check `lsmod | grep spi`.
11. Run `python3 pi_input_sender.py`.

**中文**
1. 打开 Windows PowerShell。
2. 输入 `arp -a`。
3. 使用 `ssh xlab@192.168.50.38` 连接树莓派。
4. 确认 `pi_input_sender.py` 存在。
5. 如有需要，使用 `nano pi_input_sender.py` 修改。
6. 确认 `UNITY_IP` 和 `UNITY_PORT`。
7. 使用 `sudo raspi-config` 启用 SPI。
8. 如有需要，重启树莓派。
9. 输入 `ls /dev/spidev*` 检查。
10. 如有需要，输入 `lsmod | grep spi`。
11. 输入 `python3 pi_input_sender.py` 运行脚本。

## Unity Setup / Unity 配置

**English**
1. Open `Assets/310.unity`.
2. Select the `PiSystemBridge` GameObject.
3. Check Inspector references.
4. Confirm `listenPort = 5005`.
5. Press Play.

**中文**
1. 打开 `Assets/310.unity`。
2. 选中 `PiSystemBridge` GameObject。
3. 检查 Inspector 引用。
4. 确认 `listenPort = 5005`。
5. 点击 Play。

## Runtime Flow / 运行流程

**English**
1. `ENC1` switches map style.
2. Sliders position the river-selection intersection.
3. `JOYBTN` confirms the hovered river.
4. `ENC2` changes menu focus.
5. Joystick directions move inside the active menu.
6. `JOYBTN` confirms the current menu selection.
7. `BTN3` switches encoder 3 mode if needed.
8. `ENC3` zooms or rotates the map.

**中文**
1. `ENC1` 切换地图样式。
2. 滑杆控制河流选择交点位置。
3. `JOYBTN` 确认当前悬停河流。
4. `ENC2` 切换菜单焦点。
5. 摇杆方向输入在当前菜单内移动。
6. `JOYBTN` 确认当前菜单选择。
7. 如有需要，`BTN3` 切换旋钮 3 模式。
8. `ENC3` 执行地图缩放或旋转。
