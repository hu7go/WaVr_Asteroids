using UnityEngine;
using System.Collections.Generic;

public class Spawner : MonoBehaviour
{
    public RFX4_ScaleCurves portalOpening;

    [Tooltip("The time it takes before the next enemy spawns!")]public float spawnTime = 1f;

    private EnemySpawnPoint master;
    private int numberOfEnemies;
    private int counter = 0;
    private float threshHold;
    private List<AsteroidHealth> objectiveOrder;
    /*[HideInInspector]*/ public List<EnemyAI> enemies = new List<EnemyAI>();
    private int waveIndex;
    private Wave myWaveInfo;

    private void Start()
    {
        Invoke("Spawn", spawnTime);

        portalOpening.GraphTimeMultiplier = (Manager.Instance.waves[waveIndex].enemyController.totalNumberOfEnemies * spawnTime) + 5;
    }

    private void Update()
    {
        if (objectiveOrder[0] != null)
            transform.LookAt(objectiveOrder[0].asteroid.postition);

        //if (Manager.Instance.healthPercent <= Manager.Instance.tAe.loseThreshHold)
        //{
        //    Debug.Log(Manager.Instance.healthPercent);
        //    Debug.Log(myWaveInfo.damageThreshHold);

        //    for (int i = 0; i < enemies.Count; i++)
        //    {
        //        enemies[i].GoHome();
        //    }
        //}
    }

    public void Initialize (EnemySpawnPoint m, int n, List<AsteroidHealth> newList, float newThreshHold, int newWaveIndex, Wave newWave)
    {
        myWaveInfo = newWave;
        waveIndex = newWaveIndex;
        objectiveOrder = newList;
        master = m;
        numberOfEnemies = (int)Manager.Instance.waves[waveIndex].enemyController.totalNumberOfEnemies;
        threshHold = Manager.Instance.waves[waveIndex].damageThreshHold;
    }

    private void Spawn ()
    {
        if (counter < numberOfEnemies)
            SpawEnemy();
        else
            master.Destroy();
    }

    int index = 0;
    int currentCounter = 0;

    public void SpawEnemy()
    {
        Utils.ClearLogConsole();
        if (currentCounter < myWaveInfo.enemyController.enemyTypePercent[index])
        {
            Debug.Log("index: " + index);
            Debug.Log("Number of enemies suppose to spawn: " + myWaveInfo.enemyController.enemyTypePercent[index]);
            Debug.Log("current spawn index: " + currentCounter);
        }
        else
        {
            Debug.Log("Next in enemy order");
            currentCounter = 0;
            index++;
        }

        currentCounter++;

        Debug.Log("total enemy counter: " + (counter + 1));

        //                      'This takes the first item from the enemyTypes list and spawns it'
        GameObject newEnemy = Instantiate(/*>>>>>*/myWaveInfo.enemyController.enemyTypes[index].enemie/*<<<<<*/, transform.position, transform.rotation, Manager.Instance.enemyParent.transform);
        EnemyAI tmp = newEnemy.GetComponent<EnemyAI>();
        tmp.Initialize(objectiveOrder, threshHold, master, this, waveIndex);
        enemies.Add(tmp);
        Manager.Instance.waves[waveIndex].enemies.Add(tmp);
        Manager.Instance.InstantiateEnemy(newEnemy, waveIndex);
        counter++;
        Invoke("Spawn", spawnTime);
    }

    public void UpdatePath (List<AsteroidHealth> newList)
    {
        objectiveOrder = newList;
        for (int i = 0; i < enemies.Count; i++)
        {
            enemies[i].SetPath(objectiveOrder);
        }
    }

    public void RemoveEnemie (EnemyAI enemy) => enemies.Remove(enemy);

    public void StartEndAnim ()
    {
        Animator anim = GetComponent<Animator>();
        anim.SetTrigger("PortalBeGone");
    }
}