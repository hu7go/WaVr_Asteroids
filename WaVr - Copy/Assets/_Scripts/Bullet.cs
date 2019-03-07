using UnityEngine;

public class Bullet : MonoBehaviour, IPooledObject
{
    public string tagToHit;

    [SerializeField] private Rigidbody rb;
    [SerializeField] private float speed;
    float timer = 0;

    public void OnObjectSpawn()
    {
        timer = 0;
    }

    private void FixedUpdate()
    {
        timer += Time.deltaTime;
        if (timer > 10)
            gameObject.SetActive(false);
        rb.velocity = transform.forward * speed;
    }

    public void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == tagToHit)
            gameObject.SetActive(false);
    }
}