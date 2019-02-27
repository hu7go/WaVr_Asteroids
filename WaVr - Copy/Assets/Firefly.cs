using System.Collections;
using UnityEngine;

public class Firefly : MonoBehaviour
{
    private Vector3 newPosition;
    private Vector3 positionAdded;
    private Vector3 distance;
    private Vector3 oldPos;
    public GameObject fireflies;
    Animator anim;
    private AsteroidHealth currentAsteroid;
    private Vector3 distanceToAsteroid;
    private float speed;
    private float rand;
    private bool inAsteroid;
    private bool tester;
    private bool dead;

    public void Instantiation(AsteroidHealth asteroid)
    {
        fireflies.transform.position = asteroid.transform.position;
        currentAsteroid = asteroid;
        StartCoroutine("Randomize",0f);
        anim = GetComponent<Animator>();
        dead = false;
    }
    private IEnumerator Randomize(float timeIfTrue)
    {
        yield return new WaitForSeconds(timeIfTrue);
        rand = Random.Range(-3f, 3f);
        oldPos = newPosition;
        newPosition = new Vector3(transform.position.x + rand, transform.position.y + rand, transform.position.z + rand);
        speed = Random.Range(0.05f, 0.5f);
        tester = true;
        yield return new WaitForSeconds(Random.Range(3, 10));
        StartCoroutine("Randomize",0f);
    }
    void Update()
    {
        if (currentAsteroid.asteroid.alive == false && dead == false)
            StartCoroutine("Dying");
        transform.position = Vector3.Lerp(transform.position, newPosition, Time.deltaTime * speed);
        transform.rotation = Quaternion.Euler(0f, 0f, 0f);
        positionAdded = transform.position - newPosition;
        distance = transform.position - fireflies.transform.position;
        distanceToAsteroid = transform.position - currentAsteroid.transform.position;
        if (distance.magnitude > 2.5)
        {
            StopAllCoroutines();
            oldPos = newPosition;
            newPosition = fireflies.transform.position;
            StartCoroutine("Randomize", 1f);
        }
        if (dead == true && currentAsteroid.asteroid.alive == true)
            StartCoroutine("Reviving");

    }
    private IEnumerator Reviving()
    {
        dead = false;
        yield return new WaitForSeconds(Random.Range(1, 3));
        anim.SetTrigger("Revive");
    }
    private IEnumerator Dying()
    {
        dead = true;
        yield return new WaitForSeconds(Random.Range(1, 3));
        anim.SetTrigger("Die");
    }
    private void OnCollisionEnter(Collision other)
    {
        if (other.gameObject == currentAsteroid)
        {
            StopAllCoroutines();
            newPosition = oldPos;
            StartCoroutine("Randomize",2f);
            //Vector3 direction = (distanceToAsteroid / distanceToAsteroid.magnitude) *-1;
            //newPosition = direction;
        }
    }
}