using UnityEngine;

public class FireFlyParent : MonoBehaviour
{
    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Fireflies"))
        {
            other.GetComponent<Firefly>().newPosition = transform.position;
            other.GetComponent<Firefly>().StartCoroutine("Randomize",true);
        }
    }
}