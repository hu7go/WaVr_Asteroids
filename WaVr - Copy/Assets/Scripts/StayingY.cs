using UnityEngine;

public class StayingY : MonoBehaviour
{
    public Transform target;

    public float x = 0;
    public float z = 0;

	void LateUpdate ()
    {
        transform.localRotation = Quaternion.Euler(x, target.parent.transform.localRotation.eulerAngles.y, z);
	}
}
