using UnityEngine;

public class RotateAround : MonoBehaviour {
    float random;

	void Start () {
        random = Random.Range(0.4f, 4f);
	}
	
	void Update () {
        transform.RotateAround(Vector3.zero,Vector3.up,random * Time.deltaTime);
	}
}