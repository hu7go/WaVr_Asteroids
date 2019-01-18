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
        if(transform.parent == null)
        {
            audi.Play();
            Destroy(gameObject, 3f);
        }
    }
}