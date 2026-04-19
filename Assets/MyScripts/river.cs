using UnityEngine;

public class RiverHighlighter : MonoBehaviour
{
    public Color highlightColor = Color.cyan; 
    
    private Material myMaterial;
    private Color originalColor;
    
    // 【关键】静态变量：记录当前全场景中哪条河是亮的
    private static RiverHighlighter currentHighlightedRiver;

    void Start()
    {
        Renderer rend = GetComponent<Renderer>();
        if (rend != null)
        {
            myMaterial = rend.material; 
            if (myMaterial.HasProperty("_BaseColor"))
            {
                originalColor = myMaterial.GetColor("_BaseColor");
            }
        }
    }

    // --- 修改后的方法：供按钮调用 ---
    public void SelectThisRiver()
    {
        // 1. 如果当前已经有别的河亮着，让它先熄灭
        if (currentHighlightedRiver != null && currentHighlightedRiver != this)
        {
            currentHighlightedRiver.TurnOff();
        }

        // 2. 让自己变亮
        TurnOn();

        // 3. 更新静态引用，记录自己是当前的“主角”
        currentHighlightedRiver = this;
    }

    private void TurnOn()
    {
        if (myMaterial != null)
        {
            myMaterial.SetColor("_BaseColor", highlightColor);
            myMaterial.SetColor("_EmissionColor", highlightColor);
            // 如果你的材质有自发光，别忘了开启关键字
            myMaterial.EnableKeyword("_EMISSION");
        }
    }

    public void TurnOff()
    {
        if (myMaterial != null)
        {
            myMaterial.SetColor("_BaseColor", originalColor);
            myMaterial.SetColor("_EmissionColor", Color.black); // 熄灭发光
        }
    }
}
