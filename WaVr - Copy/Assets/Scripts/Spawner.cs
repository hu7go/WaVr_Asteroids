using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour
{
    public GameObject enemy;

    private int counter;

    private void Start()
    {
        StartCoroutine(Spawn());
    }

    private IEnumerator Spawn ()
    {
        if (counter <= Manager.Instance.turretsAndEnemies.maxNumberOfEnemies)
        {
            yield return new WaitForSeconds(2);
            SpawEnemy();
        }
        else
        {
            Destroy(gameObject, 2);
        }
    }

    public void SpawEnemy()
    {
        counter++;
        GameObject newEnemy = Instantiate(enemy, transform.position, transform.rotation, Manager.Instance.enemyParent.transform);
        Manager.Instance.InstantiateEnemy(newEnemy);
    }
}
