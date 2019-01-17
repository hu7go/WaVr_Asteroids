using UnityEngine;

public class DestroyGameObjectDelay : MonoBehaviour {

	void Start () {
        Destroy(gameObject, 4f);
	}
}