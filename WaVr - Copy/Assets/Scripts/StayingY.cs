using UnityEngine;

public class StayingY : MonoBehaviour
{
    public Transform target;

	void LateUpdate ()
    {
        transform.rotation = Quaternion.Euler(0, target.rotation.eulerAngles.y, 0);
	}
}
