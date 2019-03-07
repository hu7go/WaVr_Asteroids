using UnityEngine;

public class Nuke : MonoBehaviour, IPooledObject
{
    public float explosionRadius = 20;
    public float damage = 100;
    public LayerMask layerMask;
    public GameObject explosion;
    public float speed = 1000;

    private float timer = 0;
    private float maxTimeAlive = 10f;
    private Rigidbody rb;

    public void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    public void OnObjectSpawn()
    {
        timer = 0;
        rb.angularVelocity = Vector3.zero;
        rb.velocity = Vector3.zero;
        rb.AddForce(transform.forward * speed);
    }

    private void Update()
    {
        timer += Time.deltaTime;
        if (timer > maxTimeAlive)
        {
            Instantiate(explosion, transform.position, transform.rotation);
            gameObject.SetActive(false);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, explosionRadius, layerMask);
        for (int i = 0; i < hitColliders.Length; i++)
        {
            hitColliders[i].GetComponent<EnemyAI>().TakeDamage(damage);
        }

        Instantiate(explosion, transform.position, transform.rotation);
        gameObject.SetActive(false);
    }
}