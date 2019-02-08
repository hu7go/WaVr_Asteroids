using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Linq;

public class EnemySpawnPoint : MonoBehaviour
{
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

    public List<AsteroidHealth> asteroidList;
    public List<AsteroidHealth> sortedList;

    public void StartSpawner (float newTime, int n, List<AsteroidHealth> newList)
    {
        asteroidList = newList;
        SortList();

        timer = newTime;
        start = true;
        numberOfEnemies = n;
    }

    void SortList ()
    {
        sortedList = new List<AsteroidHealth>();
        //Sort list based on distance! from the previouse target!!!!

        AsteroidHealth currentTarget = new AsteroidHealth();

        Vector3 currentPos = new Vector3();

        for (int i = 0; i < asteroidList.Count; i++)
        {
            //Special case if we are in the first position!
            if (i == 0)
                currentPos = transform.position;
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

            Debug.DrawLine(currentPos, currentTarget.asteroid.postition, new Color(Random.Range(0, 255), Random.Range(0, 255), Random.Range(0, 255)), 120);
        }
    }

    private void Update()
    {
        if (start)
        {
            transform.LookAt(sortedList[0].asteroid.postition);

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
                currentColor = purple;
                timerText.gameObject.SetActive(false);
                Manager.Instance.uISettings.countDownText.text = "00";
                GameObject tmp = Instantiate(spawer, transform);
                tmp.GetComponent<Spawner>().Initialize(this, numberOfEnemies, sortedList);
                spawned = true;
            }

            preSpawn.material.SetColor("_TintColor", currentColor); 
        }
    }

    public void Destroy ()
    {
        StartCoroutine(Manager.Instance.SpawnThemNewEnemies());
        Destroy(gameObject);
    }
}