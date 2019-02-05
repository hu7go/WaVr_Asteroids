using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EnemySpawnPoint : MonoBehaviour
{
    public GameObject spawer;
    public Text timerText;

    private float timer;
    private bool start = false;

    public void StartSpawner (float newTime)
    {
        timer = newTime;
    }

    private void Update()
    {
        if (start)
        {
            timer -= Time.deltaTime;
            timerText.text = timer.ToString("00");

            if (timer <= 0)
            {
                timerText.gameObject.SetActive(false);
                Instantiate(spawer, transform);
            }
        }
    }
}
