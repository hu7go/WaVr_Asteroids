using System.Collections;
using UnityEngine;

public class Firefly : MonoBehaviour
{
    private Vector3 newPosition;
    private Vector3 distance;
    private Vector3 oldPos;
    public GameObject fireflies;
    Animator anim;
    private AsteroidHealth currentAsteroid;
    private float speed;
    private float rand;
    private bool inAsteroid;
    private bool tester;
    private bool dead;
    private float time;

    public void Instantiation(AsteroidHealth asteroid)
    {
        fireflies.transform.position = asteroid.transform.position;
        currentAsteroid = asteroid;
        time = 0;
        StartCoroutine(Randomize());
        anim = GetComponent<Animator>();
        dead = false;
    }
    private IEnumerator Randomize()
    {
        yield return new WaitForSeconds(time);
        rand = Random.Range(-3f, 3f);
        oldPos = newPosition;
        newPosition = new Vector3(transform.position.x + rand, transform.position.y + rand, transform.position.z + rand);
        speed = Random.Range(0.05f, 0.5f);
        tester = true;
        yield return new WaitForSeconds(Random.Range(3, 10));
        time = 0;
        StartCoroutine(Randomize());
    }
    void Update()
    {
        if (currentAsteroid.asteroid.alive == false && dead == false)
            StartCoroutine("Dying");

        transform.position = Vector3.Lerp(transform.position, newPosition, Time.deltaTime * speed);
        transform.rotation = Quaternion.Euler(0f, 0f, 0f);
        distance = transform.position - fireflies.transform.position;

        if (distance.magnitude > 2.5)
        {
            if(time == 0)
                StopCoroutine("Randomize");

            oldPos = newPosition;
            newPosition = fireflies.transform.position;
            time = 2;
            StartCoroutine(Randomize());
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
        if (other.gameObject == currentAsteroid.gameObject)
        {
            StopCoroutine("Randomize");
            newPosition = oldPos;
            time = 2;
            StartCoroutine(Randomize());
        }
    }
}