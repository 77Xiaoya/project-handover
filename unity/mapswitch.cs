using UnityEngine;

public class MapSwitcher : MonoBehaviour
{
    [Header("地图图层对象")]
    public GameObject defaultMap;
    public GameObject satelliteMap;
    public GameObject terrainMap;

    // 初始化：游戏开始时默认显示其中一个（可选）
    void Start()
    {
        ShowDefault(); 
    }

    // --- 供按钮调用的公共方法 ---

    public void ShowDefault()
    {
        UpdateVisibility(defaultMap);
    }

    public void ShowSatellite()
    {
        UpdateVisibility(satelliteMap);
    }

    public void ShowTerrain()
    {
        UpdateVisibility(terrainMap);
    }

    // --- 核心逻辑 ---
    
    // 私有辅助方法：只激活传入的目标，隐藏其他所有
    private void UpdateVisibility(GameObject target)
    {
        // 如果传入的是 defaultMap，则 SetActive(true)，否则 false
        if (defaultMap != null) defaultMap.SetActive(target == defaultMap);
        if (satelliteMap != null) satelliteMap.SetActive(target == satelliteMap);
        if (terrainMap != null) terrainMap.SetActive(target == terrainMap);
    }
}