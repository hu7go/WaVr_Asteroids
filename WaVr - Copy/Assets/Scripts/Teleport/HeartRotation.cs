using UnityEngine;

public class HeartRotation : MonoBehaviour
{
    void Update()
    {
        if(Manager.Instance.turretsAndEnemies.heartrotator)
            gameObject.transform.LookAt(Manager.Instance.ReturnPlayer().transform);
        if (!Manager.Instance.turretsAndEnemies.heartrotator)
            gameObject.transform.Rotate(1, 0, 0);
    }
}