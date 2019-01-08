using System.Collections;
using UnityEngine;

public class UnparentSound : MonoBehaviour {
    AudioSource audi;
	void Start ()
    {
        audi = GetComponent<AudioSource>();
	}
	
    public void UnParent()
    {
        transform.parent = null;
        audi.Play();
        StartCoroutine(Destroyable());
    }

    private IEnumerator Destroyable()
    {
        yield return new WaitForSeconds(2f);
        Destroy(gameObject);
    }
}