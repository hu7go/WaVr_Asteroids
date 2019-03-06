using System.Collections;
using UnityEngine;

public class Firefly : MonoBehaviour
{
    public GameObject fireflies;
    Animator anim;
    private AsteroidHealth currentAsteroid;
    private float speed = 10;
    private bool dead;

    Vector3 axis;
    float timer = 0;
    float changeTime = 5;

    public void Instantiation(AsteroidHealth asteroid)
    {
        currentAsteroid = asteroid;
        fireflies.transform.position = currentAsteroid.transform.position;
        anim = GetComponent<Animator>();
        dead = false;
        ChangeAxis();
        speed = Random.Range(7.5f, 12.5f);
    }

    private void ChangeAxis()
    {
        axis = new Vector3(Random.Range(.5f, 2f), Random.Range(.5f, 2f), Random.Range(.5f, 2f));
    }

    void Update()
    {
        if (currentAsteroid.asteroid.alive == false && dead == false)
            StartCoroutine("Dying");

        timer += Time.deltaTime;

        if (timer >= changeTime)
        {
            ChangeAxis();
            changeTime = Random.Range(2.5f, 6f);
            timer = 0;
        }

        transform.RotateAround(currentAsteroid.transform.position, axis, speed * Time.deltaTime);

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
}