using UnityEngine;

public class IntersectionRiverDetector : MonoBehaviour
{
    public WaterSystemManager waterSystemManager;

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("RiverLabel"))
        {
            string riverName = other.name.ToUpper();

            Debug.Log("[River Selected] " + riverName);

            if (waterSystemManager != null)
            {
                waterSystemManager.SelectRiver(riverName);
            }
        }
    }
}