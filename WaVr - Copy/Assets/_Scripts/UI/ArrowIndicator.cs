using UnityEngine;

public class ArrowIndicator : MonoBehaviour
{
    void Update()
    {
        gameObject.transform.LookAt(Manager.Instance.tAe.currentActiveSpawner.transform.position);
    }
}