using UnityEngine;

public class ScaleMonitor : MonoBehaviour {
    void Update() {
        // 每秒打印一次，确保一定能看到输出
        if (Time.frameCount % 60 == 0) {
            Debug.Log($"[CHECK_SCALE] {gameObject.name} 缩放: {transform.localScale.x}");
        }
    }
}
