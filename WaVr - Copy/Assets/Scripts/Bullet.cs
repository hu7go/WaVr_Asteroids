using UnityEngine;

public class Bullet : MonoBehaviour
{
    [SerializeField] private Rigidbody rb;
    [SerializeField] private float speed;

    private void Start()
    {
        Destroy(gameObject, 5f);
    }

    private void FixedUpdate()
    {
        rb.velocity = transform.forward * speed;
    }

    public void OnTriggerEnter(Collider other)
    {
        Destroy(gameObject);
    }
}