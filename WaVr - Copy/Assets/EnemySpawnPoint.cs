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

    private bool spawned = false;

    public void StartSpawner (float newTime)
    {
        timer = newTime;
        start = true;
    }

    private void Update()
    {
        if (start)
        {
            timer -= Time.deltaTime;
            timerText.text = timer.ToString("00");
            Manager.Instance.uISettings.countDownText.text = timer.ToString("00");

            if (timer <= 0 && spawned == false)
            {
                timerText.gameObject.SetActive(false);
                Instantiate(spawer, transform);
                spawned = true;
            }
        }
    }
}
