using UnityEngine;
using System.Collections.Generic;
using System.Collections;

public class Spawner : MonoBehaviour
{
    public RFX4_ScaleCurves portalOpening;

    [Tooltip("The time it takes before the next enemy spawns!")]public float spawnTime = 1f;

    private EnemySpawnPoint master;
    private int numberOfEnemies;
    private int counter = 0;

    private float threshHold;

    private List<AsteroidHealth> objectiveOrder;

    private List<EnemyAI> enemies = new List<EnemyAI>();

    private int waveIndex;

    private Wave myWaveInfo;

    private void Start()
    {
        Invoke("Spawn", spawnTime);

        portalOpening.GraphTimeMultiplier = (Manager.Instance.waves[waveIndex].maxNumberOfEnemies * spawnTime) + 5;
    }

    private void Update()
    {
        if (objectiveOrder[0] != null)
            transform.LookAt(objectiveOrder[0].asteroid.postition);
    }

    public void Initialize (EnemySpawnPoint m, int n, List<AsteroidHealth> newList, float newThreshHold, int newWaveIndex, Wave newWave)
    {
        myWaveInfo = newWave;
        waveIndex = newWaveIndex;
        objectiveOrder = newList;
        master = m;
        numberOfEnemies = (int)Manager.Instance.waves[waveIndex].maxNumberOfEnemies;
        threshHold = Manager.Instance.waves[waveIndex].damageThreshHold;
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
        //                      'This takes the first item from the enemyTypes list and spawns it'
        GameObject newEnemy = Instantiate(/*>>>>>*/myWaveInfo.enemyTypes[0].enemie/*<<<<<*/, transform.position, transform.rotation, Manager.Instance.enemyParent.transform);
        EnemyAI tmp = newEnemy.GetComponent<EnemyAI>();
        tmp.Initialize(objectiveOrder, threshHold, master, this, waveIndex);
        enemies.Add(tmp);
        Manager.Instance.waves[waveIndex].enemies.Add(tmp);
        Manager.Instance.InstantiateEnemy(newEnemy, waveIndex);
        Invoke("Spawn", spawnTime);
    }

    bool startedPathFinding = false;

    public void CheckForNewPath ()
    {
        Debug.Log("Started checking path!");

        if (startedPathFinding == false)
        {
            startedPathFinding = true;

            //master.FindPath();
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

        Debug.Log("Stoped checking path!");
    }

    public void UpdatePath (List<AsteroidHealth> newList)
    {
        Debug.Log("testing path updateing");
        objectiveOrder = newList;
        for (int i = 0; i < enemies.Count; i++)
        {
            Debug.Log("Testing stuffs paths n that");
            enemies[i].SetPath(objectiveOrder);
        }
    }
}