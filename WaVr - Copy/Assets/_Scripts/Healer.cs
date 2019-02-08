using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Healer : MonoBehaviour, ITakeDamage<float>
{
    HealerInfo hI;

    private AsteroidHealth myAsteroid;

    void Start()
    {
        hI = Manager.Instance.healerInfoTemplate;

        hI.health = 100;
        hI.alive = true;
    }

    public void SpawnAHealer(GameObject currentCube)
    {
        myAsteroid = currentCube.GetComponentInChildren<AsteroidHealth>();
        if (myAsteroid.asteroid.health < Manager.Instance.turretsAndEnemies.asteroidHealth)
            StartCoroutine(IHeal());
    }

    public void TakeDamage(float damage)
    {
        hI.health -= damage;

        if (hI.health <= 0)
        {
            hI.alive = false;
            CancelInvoke();
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

    private void Heal()
    {
        if (myAsteroid.asteroid.health >= Manager.Instance.turretsAndEnemies.asteroidHealth)
        {
            CancelInvoke();
        }

        myAsteroid.asteroid.health += hI.regenValue;

        if (myAsteroid.asteroid.health > 0)
        {
            myAsteroid.asteroid.alive = true;
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
    public bool alive;
    public bool startHealing;
}