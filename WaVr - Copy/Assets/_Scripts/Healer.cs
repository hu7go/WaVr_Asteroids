using System.Collections;
using UnityEngine;

public class Healer : MonoBehaviour, ITakeDamage<float>
{
    HealerInfo hI;

    private AsteroidHealth myAsteroid;

    void Start()
    {
        hI = Manager.Instance.healerInfoTemplate;
        transform.position += hI.postition;
        hI.health = 100;
    }

    public void SpawnAHealer(GameObject currentCube)
    {
        myAsteroid = currentCube.GetComponentInChildren<AsteroidHealth>();
        myAsteroid.asteroid.beingHealed = true;
        if (myAsteroid.asteroid.health < Manager.Instance.turretsAndEnemies.asteroidHealth)
            StartCoroutine(IHeal());
    }

    public void TakeDamage(float damage)
    {
        hI.health -= damage;

        if (hI.health <= 0)
        {
            myAsteroid.asteroid.beingHealed = false;
            Destroy(gameObject);
        }
    }

    private IEnumerator IHeal ()
    {
        myAsteroid.asteroid.alive = true;

        while (myAsteroid.asteroid.health < Manager.Instance.turretsAndEnemies.asteroidHealth)
        {
            myAsteroid.asteroid.health += hI.regenValue;
            myAsteroid.Heal(hI.regenValue);
            yield return new WaitForSeconds(hI.regenSpeed);
        }
    }
}

[System.Serializable]
public struct HealerInfo
{
    public Vector3 postition;
    [Range(0, 100)]
    public float health;
    public float regenSpeed;
    public float regenValue;
}