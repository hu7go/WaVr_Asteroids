using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Collections;

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

    private GameObject probe;
    private float threshHold;
    private Vector3 spawnerPosition;

    [HideInInspector] public bool foundPath = false;

    public void StartSpawner (float newTime, int n, List<AsteroidHealth> newList, float newThreshHold)
    {
        spawnerPosition = transform.position;

        asteroidList = newList;

        FindPath();

        timer = newTime;
        start = true;
        numberOfEnemies = n;

        threshHold = newThreshHold;

        probe = Instantiate(probePrefab, transform.position, transform.rotation, Manager.Instance.enemyParent.transform);
    }

    public void FindPath ()
    {
        StartCoroutine(StartPathFinding());
    }

    private IEnumerator StartPathFinding ()
    {
        pathThread = new Thread(SortList);
        pathThread.Start();


        while (pathThread.IsAlive)
        {
            yield return null;
        }

        for (int i = 0; i < sortedList.Count; i++)
        {
            if (i == 0)
                Debug.DrawLine(transform.position, sortedList[i].asteroid.postition, new Color(255, 0, 100), 5);
            if (i + 1 < sortedList.Count)
                Debug.DrawLine(sortedList[i].asteroid.postition, sortedList[i + 1].asteroid.postition, new Color(255, 0, 100), 5);
        }
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
            //

            //Sorts the list from the current asteroid to the closest one!
            asteroidList.OrderBy(x => Vector3.Distance(currentPos, x.asteroid.postition)).ToList();
            asteroidList.Sort(delegate (AsteroidHealth a, AsteroidHealth b)
            {
                return Vector3.Distance(currentPos, a.asteroid.postition)
                .CompareTo(Vector3.Distance(currentPos, b.asteroid.postition));
            });
            //

            //Checks if the the next asteroid already exist in the list we have!
            //!? The "currentTarget.asteroid.alive" check might be nececary to take away, it might be causeing some performance issues when there is only a few asteroids left!
            int c = 0;
            if (sortedList.Contains(currentTarget) || currentTarget.asteroid.alive == false)
            {
                while (sortedList.Contains(currentTarget) || currentTarget.asteroid.alive == false)
                {
                    currentTarget = asteroidList[c];
                    c++;
                }
            }
            else
            {
                currentTarget = asteroidList[0];
            }
            //

            sortedList.Add(currentTarget);
        }
        foundPath = true;
    }
    //

    bool startedProbe = false;

    private void Update()
    {
        if (startedProbe == false)
        {
            if (pathThread.IsAlive == false)
            {
                probe.GetComponent<Probe>().Instantiate(sortedList[0].asteroid.postition, transform.position);
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
                probe.GetComponent<Probe>().Return(transform.position);

                currentColor = purple;
                timerText.gameObject.SetActive(false);
                Manager.Instance.uISettings.countDownText.text = "00";
                GameObject tmp = Instantiate(spawer, transform);
                tmp.GetComponent<Spawner>().Initialize(this, numberOfEnemies, sortedList, threshHold);
                spawned = true;
            }

            preSpawn.material.SetColor("_TintColor", currentColor); 
        }
    }

    public void Destroy ()
    {
        StartCoroutine(Manager.Instance.SpawnThemNewEnemies());
        //Destroy(gameObject);
    }
}