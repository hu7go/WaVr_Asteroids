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

    void Update()
    {
        if (hI.alive == false)
            Destroy(gameObject);
    }
    public void SpawnAHealer(GameObject currentCube)
    {
        myAsteroid = currentCube.GetComponentInChildren<AsteroidHealth>();
        if (myAsteroid.asteroid.alive == false)
            if (myAsteroid.asteroid.health < Manager.Instance.turretsAndEnemies.asteroidHealth)
                InvokeRepeating("Heal", 0, hI.regenSpeed);
    }
    public void TakeDamage(float damage)
    {
        hI.health -= damage;

        if (hI.health <= 0)
        {
            hI.alive = false;
            CancelInvoke();
        }
    }
    private void Heal()
    {
        if (myAsteroid.asteroid.health < Manager.Instance.turretsAndEnemies.asteroidHealth)
            myAsteroid.asteroid.health += hI.regenValue;
        if (myAsteroid.asteroid.health >= Manager.Instance.turretsAndEnemies.asteroidHealth)
            CancelInvoke();
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