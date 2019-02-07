using UnityEngine;
using System.Collections.Generic;

public class Spawner : MonoBehaviour
{
    public GameObject enemy;
    [Tooltip("The time it takes before the next enemy spawns!")]public float spawnTime = 1f;

    private EnemySpawnPoint master;
    private int numberOfEnemies;
    private int counter = 0;

    private List<AsteroidHealth> objecctiveOrder;
    private void Start()
    {
        Invoke("Spawn", spawnTime);
    }

    private void Update()
    {
        transform.LookAt(Manager.Instance.ReturnPlayer().transform);
    }

    public void Initialize (EnemySpawnPoint m, int n, List<AsteroidHealth> newList)
    {
        objecctiveOrder = newList;
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
        newEnemy.GetComponent<EnemyAI>().Initialize(objecctiveOrder);
        Manager.Instance.InstantiateEnemy(newEnemy);
        Invoke("Spawn", spawnTime);
    }
}