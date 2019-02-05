using UnityEngine;

public class HeartRotation : MonoBehaviour
{
    //public GameObject looker;
    void Update()
    {
        if(Manager.Instance.turretsAndEnemies.heartrotator)
            gameObject.transform.LookAt(Manager.Instance.ReturnPlayer().transform);
            //gameObject.transform.LookAt(looker.transform);
        if (!Manager.Instance.turretsAndEnemies.heartrotator)
            gameObject.transform.Rotate(0, 1, 0);
    }
}