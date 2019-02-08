using UnityEngine;

public class StayingY : MonoBehaviour
{
    public Transform target;

	void LateUpdate ()
    {
        transform.localRotation = Quaternion.Euler(0, target.parent.transform.localRotation.eulerAngles.y, 0);
	}
}