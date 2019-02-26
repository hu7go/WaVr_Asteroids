using System.Collections;
using UnityEngine;

public class Firefly : MonoBehaviour
{
    [HideInInspector]
    public Vector3 newPosition;
    private Vector3 positionAdded;
    private Vector3 distance;
    public GameObject fireflies;
    //will be private later
    public GameObject currentAsteroid;
    private Vector3 distanceToAsteroid;
    private float speed;
    private float rand;
    private void Start()
    {
        StartCoroutine("Randomize",false);
    }
    private IEnumerator Randomize(bool call)
    {
        if (call)
            yield return new WaitForSeconds(5f);
        rand = Random.Range(-3f, 3f);
        newPosition = new Vector3(transform.position.x + rand, transform.position.y + rand, transform.position.z + rand);
        speed = Random.Range(0.05f, 0.5f);
        yield return new WaitForSeconds(Random.Range(3, 10));
        StartCoroutine("Randomize",false);
    }
    void Update()
    {
        transform.position = Vector3.Lerp(transform.position, newPosition, Time.deltaTime * speed);
        transform.rotation = Quaternion.Euler(0f, 0f, 0f);
        positionAdded = transform.position - newPosition;
        distance = transform.position - fireflies.transform.position;
        distanceToAsteroid = transform.position - currentAsteroid.transform.position;
        if (distance.magnitude > 5 || distance.magnitude < -5)
            newPosition = fireflies.transform.position;
        if(gameObject.name == "Sphere(6)")
            print(distance.magnitude);
    }

    private void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.CompareTag("Asteroid"))
        {
            print(other.gameObject.name);
            Vector3 direction = ((transform.position - currentAsteroid.transform.position)/distanceToAsteroid.magnitude) *-1;
            newPosition = direction;
        }
    }
}