using UnityEngine;
using UnityEngine.UI; // 必须引用 UI，因为要控制 Image

public class SimpleHighlighter : MonoBehaviour
{
    [Header("1. 拖入河流模型 (变色目标)")]
    public Renderer targetRenderer; 

    [Header("2. 拖入标签背景 (变色目标)")]
    public Image labelBackground; // 这里就是你要控制的 Image

    [Header("3. 选中后的颜色")]
    public Color riverHighlightColor = new Color(1f, 0.8f, 0f); // 河流变亮黄
    public Color labelHighlightColor = new Color(1f, 0f, 0f, 0.8f); // 标签背景变红

    // 内部记录原始颜色
    private Color orgRiverColor;
    private Color orgLabelColor;
    private bool isSelected = false;

    void Start()
    {
        // 自动记住河流原本的颜色
        if (targetRenderer != null) 
            orgRiverColor = targetRenderer.material.color;

        // 自动记住标签原本的颜色
        if (labelBackground != null) 
            orgLabelColor = labelBackground.color;
    }

    void OnMouseDown()
    {
        isSelected = !isSelected; // 切换开关
        UpdateColor();
    }

    void UpdateColor()
    {
        if (isSelected)
        {
            // === 变身！(选中状态) ===
            if (targetRenderer) targetRenderer.material.color = riverHighlightColor;
            if (labelBackground) labelBackground.color = labelHighlightColor;
        }
        else
        {
            // === 还原！(普通状态) ===
            if (targetRenderer) targetRenderer.material.color = orgRiverColor;
            if (labelBackground) labelBackground.color = orgLabelColor;
        }
    }
}