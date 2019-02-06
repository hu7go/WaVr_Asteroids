using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

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

    public void StartSpawner (float newTime)
    {
        timer = newTime;
        start = true;
    }

    private void Update()
    {
        if (start)
        {
            currentColor = purple;

            timer -= Time.deltaTime;
            timerText.text = timer.ToString("00");
            if (spawned == true)
                Manager.Instance.uISettings.countDownText.text = timer.ToString("00");

            if (timer <= 10)
                currentColor = red;
            if (timer <= 6)
                currentColor = purple;
            if (timer <= 4)
                currentColor = red;
            if (timer <= 3)
                currentColor = purple;
            if (timer <= 2.5f)
                currentColor = red;
            if (timer <= 2f)
                currentColor = purple;
            if (timer <= 1.5f)
                currentColor = red;
            if (timer <= 1.25f)
                currentColor = purple;
            if (timer <= 1f)
                currentColor = red;
            if (timer <= .75f)
                currentColor = purple;
            if (timer <= .5f)
                currentColor = red;
            if (timer <= .25f)
                currentColor = purple;
            if (timer <= .1f)
                currentColor = red;

            if (timer <= 0 && spawned == false)
            {
                //preSpawn.gameObject

                timerText.gameObject.SetActive(false);
                Manager.Instance.uISettings.countDownText.text = "00";
                Instantiate(spawer, transform);
                spawned = true;
            }

            preSpawn.material.SetColor("_TintColor", currentColor); 
        }
    }
}
