using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Collections;
using System;

public class EnemySpawnPoint : MonoBehaviour
{
    Thread pathThread;

    public GameObject spawer;
    public MeshRenderer preSpawn;
    public Text timerText;

    private float timer;
    private bool start = false;

    private bool spawned = false;

    public Color red = Color.red;
    public Color purple = Color.magenta;
    private Color currentColor;
    private int numberOfEnemies;

    private List<AsteroidHealth> asteroidList;
    [HideInInspector] public List<AsteroidHealth> sortedList;

    [Space(20)]
    public GameObject probePrefab;

    private List<GameObject> probes = new List<GameObject>();
    private float threshHold;
    private Vector3 spawnerPosition;

    int waveIndex;

    [HideInInspector] public bool foundPath = false;

    private Spawner mySpawner;

    public void StartSpawner (float newTime, int n, List<AsteroidHealth> newList, float newThreshHold, int newWaveIndex)
    {
        waveIndex = newWaveIndex;

        //asteroidList.Clear();
        asteroidList = newList;

        FindPath(transform.position);

        timer = newTime;
        start = true;
        numberOfEnemies = n;

        threshHold = newThreshHold;

        StartCoroutine(ProbeSpawn());
    }

    private IEnumerator ProbeSpawn ()
    {
        yield return new WaitForSeconds(.5f);
        int i = 0;
        while (i < 3)
        {
            GameObject newProbe = Instantiate(probePrefab, transform.position, transform.rotation, Manager.Instance.enemyParent.transform);
            newProbe.GetComponent<Probe>().Instantiate(sortedList, transform.position);
            probes.Add(newProbe);
            i++;
            yield return new WaitForSeconds(3);
        }
    }

    public void FindPath (Vector3 pos)
    {
        spawnerPosition = pos;

        StartCoroutine(StartPathFinding());
    }

    private IEnumerator StartPathFinding ()
    {
        //sortedList.Clear();

        pathThread = new Thread(SortList);
        pathThread.Start();

        yield return null;
    }

    //! Happens on a separate thread!!
    void SortList ()
    {
        foundPath = false;
        //Sort list based on distance! from the previouse target!!!!
        sortedList = new List<AsteroidHealth>();
        AsteroidHealth currentTarget = new AsteroidHealth();
        Vector3 currentPos = new Vector3();

        for (int i = 0; i < asteroidList.Count; i++)
        {
            //Special case if we are in the first position!
            if (i == 0)
                currentPos = spawnerPosition;
            else
                currentPos = currentTarget.asteroid.postition;

            //Sorts the list from the current asteroid to the closest one!
            asteroidList.OrderBy(x => Vector3.Distance(currentPos, x.asteroid.postition)).ToList();
            asteroidList.Sort(delegate (AsteroidHealth a, AsteroidHealth b)
            {
                return Vector3.Distance(currentPos, a.asteroid.postition)
                .CompareTo(Vector3.Distance(currentPos, b.asteroid.postition));
            });

            //The first element of the asteroid list is always the closest to the current asteroid!
            if (sortedList.Contains(asteroidList[0]) || asteroidList[0].asteroid.alive == false)
            {
                try
                {
                    for (int j = 0; j < asteroidList.Count; j++)
                    {
                        if (!sortedList.Contains(asteroidList[j]) && asteroidList[j].asteroid.alive == true)
                        {
                            Debug.Log("How many times does this thing do?: " + j);

                            currentTarget = asteroidList[j];
                            break;
                        }
                    }
                }
                catch (Exception e)
                {
                    Debug.Log("Caught a error: ");
                    Debug.Log(e, this);
                    throw;
                }
            }
            else
            {
                currentTarget = asteroidList[0];
            }

            sortedList.Add(currentTarget);
        }

        foundPath = true;

        if (mySpawner != null)
        {
            mySpawner.UpdatePath(sortedList);
        }
    }
    //

    bool startedProbe = false;

    private void Update()
    {
        if (startedProbe == false)
        {
            if (pathThread.IsAlive == false)
            {
                for (int i = 0; i < probes.Count; i++)
                {
                    Debug.Log("Test " + i);
                    probes[i].GetComponent<Probe>().Instantiate(sortedList, transform.position);
                }
                startedProbe = true;
            }
        }

        if (start)
        {
            transform.LookAt(Manager.Instance.ReturnPlayer().transform);

            currentColor = purple;

            timer -= Time.deltaTime;
            timerText.text = timer.ToString("00");
            if (spawned == false)
                Manager.Instance.uISettings.countDownText.text = timer.ToString("00");

            if (timer <= 10)
                currentColor = red;
            if (timer <= 8)
                currentColor = purple;
            if (timer <= 6)
                currentColor = red;
            if (timer <= 4)
                currentColor = purple;
            if (timer <= 3)
                currentColor = red;
            if (timer <= 2.5f)
                currentColor = purple;
            if (timer <= 2f)
                currentColor = red;
            if (timer <= 1.5f)
                currentColor = purple;
            if (timer <= 1.25f)
                currentColor = red;
            if (timer <= 1f)
                currentColor = purple;
            if (timer <= .75f)
                currentColor = red;
            if (timer <= .5f)
                currentColor = purple;
            if (timer <= .35f)
                currentColor = red;
            if (timer <= .2f)
                currentColor = purple;
            if (timer <= .1f)
                currentColor = red;

            if (timer <= 0 && spawned == false)
            {
                foreach (GameObject tmpProbe in probes)
                    tmpProbe.GetComponent<Probe>().Return(transform.position);

                currentColor = purple;
                timerText.gameObject.SetActive(false);
                Manager.Instance.uISettings.countDownText.text = "00";
                GameObject tmp = Instantiate(spawer, transform);
                mySpawner = tmp.GetComponent<Spawner>();
                mySpawner.Initialize(this, numberOfEnemies, sortedList, threshHold, waveIndex);
                spawned = true;
            }

            preSpawn.material.SetColor("_TintColor", currentColor); 

            if (spawned == true)
            {
                currentColor = purple;
                preSpawn.material.SetColor("_TintColor", currentColor);
            }
        }
    }

    public void Destroy ()
    {
        StartCoroutine(Manager.Instance.SpawnThemNewEnemies());
    }

    //Debuging stuffs

    [HideInInspector] public bool drawPath = false;

    private void OnDrawGizmos()
    {
        if (drawPath)
        {
            for (int i = 0; i < sortedList.Count; i++)
            {
                if (i == 0)
                {
                    Gizmos.color = new Color(100, 50 + (i * 2), 0);
                    Gizmos.DrawLine(transform.position, sortedList[i].asteroid.postition);
                }
                if (i - 1 >= 0)
                {
                    Gizmos.color = new Color(100, 50 + (i * 2), 0);
                    Gizmos.DrawLine(sortedList[i - 1].asteroid.postition, sortedList[i].asteroid.postition);
                }
            }
        }
    }
}