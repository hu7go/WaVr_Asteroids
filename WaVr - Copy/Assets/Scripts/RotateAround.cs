using UnityEngine;

public class RotateAround : MonoBehaviour {
    float random;

	void Start () {
        random = Random.Range(0.1f, 2f);
	}
	
	void Update () {
        transform.RotateAround(Vector3.zero,Vector3.right,random * Time.deltaTime);
	}
}