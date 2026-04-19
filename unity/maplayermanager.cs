using UnityEngine;
using UnityEngine.UI; // 如果你后面需要控制UI显示，需要引用这个，目前只需下面逻辑

public class MapManager : MonoBehaviour
{
    public GameObject[] mapLayers;

    // --- 新增部分：初始化 ---
    void Start()
    {
        // 游戏一开始，直接强制调用一次切换函数
        // 参数 0 对应你数组里的第一个元素 (SgLangatDefault)
        SwitchToMap(0); 
    }
    // ----------------------

    public void SwitchToMap(int index)
    {
        for (int i = 0; i < mapLayers.Length; i++)
        {
            if (i == index)
            {
                mapLayers[i].SetActive(true);
            }
            else
            {
                mapLayers[i].SetActive(false);
            }
        }
    }
}