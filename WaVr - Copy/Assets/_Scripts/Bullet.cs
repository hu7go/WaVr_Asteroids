using UnityEngine;

public class Bullet : MonoBehaviour
{
    public string tagToHit;

    [SerializeField] private Rigidbody rb;
    [SerializeField] private float speed;

    private void Start() => Destroy(gameObject, 4f);

    private void FixedUpdate() => rb.velocity = transform.forward * speed;

    public void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == tagToHit)
            Destroy(gameObject);
    }
}