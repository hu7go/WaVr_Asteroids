using UnityEngine;
public class ArrowIndicator : MonoBehaviour
{
    void Update()
    {
        gameObject.transform.LookAt(Manager.Instance.turretsAndEnemies.currentActiveSpawner);
    }
}