using UnityEngine;
using System.Collections.Generic;
using System.Collections;

public class Spawner : MonoBehaviour
{
    public GameObject enemy;
    [Tooltip("The time it takes before the next enemy spawns!")]public float spawnTime = 1f;

    private EnemySpawnPoint master;
    private int numberOfEnemies;
    private int counter = 0;

    private float threshHold;

    private List<AsteroidHealth> objecctiveOrder;

    private List<EnemyAI> enemies = new List<EnemyAI>();

    private void Start()
    {
        Invoke("Spawn", spawnTime);
    }

    private void Update()
    {
        transform.LookAt(Manager.Instance.ReturnPlayer().transform);
    }

    public void Initialize (EnemySpawnPoint m, int n, List<AsteroidHealth> newList, float newThreshHold)
    {
        objecctiveOrder = newList;
        master = m;
        numberOfEnemies = n;
        threshHold = newThreshHold;
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
        newEnemy.GetComponent<EnemyAI>().Initialize(objecctiveOrder, threshHold, master, this);
        enemies.Add(newEnemy.GetComponent<EnemyAI>());
        Manager.Instance.InstantiateEnemy(newEnemy);
        Invoke("Spawn", spawnTime);
    }

    bool startedPathFinding = false;

    public void CheckForNewPath ()
    {
        if (startedPathFinding == false)
        {
            startedPathFinding = true;

            master.FindPath();
            StartCoroutine(WaitForPath());
        }
    }

    private IEnumerator WaitForPath ()
    {
        while (master.foundPath == false)
        {
            yield return null;
        }

        for (int i = 0; i < enemies.Count; i++)
        {
            enemies[i].SetPath(master.sortedList);
        }
        startedPathFinding = false;
    }
}