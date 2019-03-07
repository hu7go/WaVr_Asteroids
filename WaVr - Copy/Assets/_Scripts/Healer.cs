using System.Collections;
using UnityEngine;

public class Healer : MonoBehaviour, ITakeDamage<float, EnemySpawnPoint>, IPooledObject
{
    HealerInfo hI;

    private AsteroidHealth myAsteroid;

    void Start()
    {
        hI = Manager.Instance.healerInfoTemplate;
        transform.position += hI.postition;
    }

    public void OnObjectSpawn()
    {
        hI.health = Manager.Instance.healerInfoTemplate.health;
    }

    public void SpawnAHealer(GameObject currentCube)
    {
        myAsteroid = currentCube.GetComponentInChildren<AsteroidHealth>();
        myAsteroid.asteroid.beingHealed = true;
        myAsteroid.myHealer = this;
        if (myAsteroid.asteroid.health < Manager.Instance.tAe.asteroidHealth)
            StartCoroutine(IHeal());
    }

    public void TakeDamage(float damage, EnemySpawnPoint spawn = null)
    {
        hI.health -= damage;

        if (hI.health <= 0)
        {
            myAsteroid.asteroid.beingHealed = false;
            gameObject.SetActive(false);
        }
    }

    private IEnumerator IHeal ()
    {
        myAsteroid.Revive();

        while (myAsteroid.asteroid.health < Manager.Instance.tAe.asteroidHealth)
        {
            myAsteroid.asteroid.health += hI.regenValue;
            myAsteroid.Heal(hI.regenValue);
            yield return new WaitForSeconds(hI.regenSpeed);
        }

        //Ifall kuben blir max hp, förstör Healer
        if (myAsteroid.asteroid.health >= Manager.Instance.tAe.asteroidHealth)
        {
            myAsteroid.asteroid.beingHealed = false;
            gameObject.SetActive(false);
            myAsteroid.asteroid.health = Manager.Instance.tAe.asteroidHealth;
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