using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour
{
    public GameObject enemy;

    private EnemySpawnPoint master;
    private int numberOfEnemies;
    private int counter = 0;

    private void Start()
    {
        Invoke("Spawn", 2);
    }

    private void Update()
    {
        transform.LookAt(Manager.Instance.ReturnPlayer().transform);
    }

    public void Initialize (EnemySpawnPoint m, int n)
    {
        master = m;
        numberOfEnemies = n;
    }

    private void Spawn ()
    {
        if (counter < numberOfEnemies)
            SpawEnemy();
        else
            master.Destroy();
    }

    public void SpawEnemy()
    {
        counter++;
        GameObject newEnemy = Instantiate(enemy, transform.position, transform.rotation, Manager.Instance.enemyParent.transform);
        Manager.Instance.InstantiateEnemy(newEnemy);
        Invoke("Spawn", 2);
    }
}
