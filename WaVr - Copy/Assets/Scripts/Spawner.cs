using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour
{
    public GameObject enemy;

    private int counter = 0;

    private void Start()
    {
        Invoke("Spawn", 2);
    }

    private void Update()
    {
        transform.LookAt(Manager.Instance.ReturnPlayer().transform);
    }

    private void Spawn ()
    {
        if (counter < Manager.Instance.turretsAndEnemies.maxNumberOfEnemies)
            SpawEnemy();
        else
            Destroy(gameObject, 2);
    }

    public void SpawEnemy()
    {
        counter++;
        GameObject newEnemy = Instantiate(enemy, transform.position, transform.rotation, Manager.Instance.enemyParent.transform);
        Manager.Instance.InstantiateEnemy(newEnemy);
        Invoke("Spawn", 2);
    }
}
