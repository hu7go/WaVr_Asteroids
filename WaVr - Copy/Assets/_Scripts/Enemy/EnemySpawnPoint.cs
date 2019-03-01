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
    public GameObject animatedSphere;

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

    private Wave myWaveInfo;
    private MeshRenderer arrowRenderer;

    [Space(20)]
    public float damageDonePercent = 0;

    float myDamageDone = 0;

    public void HealthTracker (float damage)
    {
        myDamageDone += damage;
        damageDonePercent = (myDamageDone / Manager.Instance.masterMaxHealth) * 100f;
    }

    public void StartSpawner (float newTime, int n, List<AsteroidHealth> newList, float newThreshHold, int newWaveIndex, Wave newWave)
    {
        myWaveInfo = newWave;
        waveIndex = newWaveIndex;
        asteroidList = newList;
        timer = newTime;
        arrowRenderer = Manager.Instance.arrowRenderer;
        StartCoroutine(ArrowColor(timer, Color.red, 0));

        FindPath(transform.position);

        start = true;
        numberOfEnemies = n;

        threshHold = newThreshHold;

        StartCoroutine(ProbeSpawn());
    }

    private IEnumerator ArrowColor (float timeToMove, Color newColor, int index)
    {
        Color color;
        if (index == 0)
            color = Color.red;
        else
            color = Color.green;

        float time = 0f;
        while (time < 1)
        {
            time += Time.deltaTime / timeToMove;
            arrowRenderer.material.color = Color.Lerp(arrowRenderer.material.color, newColor, time / (timeToMove * (timeToMove * .33f)));
            yield return null;
        }
    }

    bool startedSpawing = false;

    private IEnumerator ProbeSpawn ()
    {
        yield return new WaitForSeconds(.5f);

        float x = Manager.Instance.masterMaxHealth / Manager.Instance.tAe.asteroidHealth * myWaveInfo.damageThreshHold / 100;
        x = Mathf.Floor(x);
        float numberOfAsteroidsToGoTo = Manager.Instance.asteroidList.Count() - x;

        int i = 0;
        while (i < numberOfAsteroidsToGoTo)
        {
            if (startedSpawing == false)
            {
                GameObject newProbe = Instantiate(probePrefab, transform.position, transform.rotation, Manager.Instance.enemyParent.transform);
                newProbe.GetComponent<Probe>().Instantiate(sortedList, transform.position, numberOfAsteroidsToGoTo);
                probes.Add(newProbe);
                i++;
                //Time before next one spawns!
                yield return new WaitForSeconds(2);
            }
            else
            {
                break;
            }
        }
    }

    bool pathForAll = false;

    public void FindPath (Vector3 pos)
    {
        pathForAll = true;
        spawnerPosition = pos;
        StartPathFinding();
    }

    public void RequestPath (Vector3 pos)
    {
        pathForAll = false;
        spawnerPosition = pos;
        StartPathFinding();
    }

    private void StartPathFinding ()
    {
        pathThread = new Thread(SortList);
        pathThread.Start();
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
            {
                currentPos = spawnerPosition;
            }
            else
            {
                currentPos = currentTarget.asteroid.postition;
            }

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
                for (int j = 0; j < asteroidList.Count; j++)
                {
                    if (!sortedList.Contains(asteroidList[j]) && asteroidList[j].asteroid.alive == true)
                    {
                        currentTarget = asteroidList[j];
                        break;
                    }
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

        //if (pathForAll)
        //{
        //    if (mySpawner != null)
        //    {
        //        mySpawner.UpdatePath(sortedList);
        //    }
        //}
        //if (!pathForAll)
        //{
        //    if (ai != null)
        //    {
        //        ai.GetPath(sortedList);
        //    }
        //}
    }
    //

    bool done = false;
    bool doneSpawning = false;

    private void Update()
    {
        if (start)
        {
            transform.LookAt(Manager.Instance.ReturnPlayer().transform, Manager.Instance.GetWorldAxis());

            currentColor = purple;

            timer -= Time.deltaTime;
            timerText.text = timer.ToString("00");
            if (spawned == false)
                Manager.Instance.uISettings.countDownText.text = timer.ToString("00");

            if (timer == 10)
                animatedSphere.GetComponent<Animator>().SetTrigger("CountDownStart");

            if (timer <= 0 && spawned == false)
            {
                foreach (GameObject tmpProbe in probes)
                    tmpProbe.GetComponent<Probe>().Return(transform.position);

                currentColor = purple;
                timerText.gameObject.SetActive(false);
                Manager.Instance.uISettings.countDownText.text = "00";
                GameObject tmp = Instantiate(spawer, transform);
                mySpawner = tmp.GetComponent<Spawner>();
                mySpawner.Initialize(this, numberOfEnemies, sortedList, threshHold, waveIndex, myWaveInfo);
                spawned = true;
                startedSpawing = true;
            }

            if (preSpawn != null)
                preSpawn.material.SetColor("_TintColor", currentColor); 

            if (spawned == true)
            {
                currentColor = purple;
                if (done == false)
                {
                    done = true;
                    StartCoroutine(ArrowColor(7, Color.green, 1));
                }
                if (preSpawn != null)
                    preSpawn.material.SetColor("_TintColor", currentColor);
            }
        }

        if (doneSpawning == true)
        {
            Over();
        }
    }

    public void Destroy ()
    {
        //StartCoroutine(Manager.Instance.SpawnThemNewEnemies());
        doneSpawning = true;
    }

    public void Over ()
    {
        //! This might need to get changed later because some enemies have no healththrehhold!
        if (mySpawner.enemies.Count <= 0)
        {
            Destroy(preSpawn.gameObject);
            mySpawner.StartEndAnim();
            Manager.Instance.SwitchPortalTarget();
            Destroy(gameObject, 3);
        }
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