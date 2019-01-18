using System;
using UnityEngine;
using UnityEngine.UI;

public class ObjectiveHP : MonoBehaviour
{
    [SerializeField] private float hp = 100;
    private float startingHp;
    [SerializeField] private LayerMask bulletLayerMask;
    AudioSource hb1;
    AudioSource hb2;
    AudioSource hb3;

    private void Start()
    {
        AudioSource[] audios = GetComponents<AudioSource>();
        hb1 = audios[0];
        hb2 = audios[1];
        hb3 = audios[2];
        startingHp = hp;
        if (!hb1.isPlaying)
            hb1.Play();
    }

    public void TakeDamage (int damage)
    {
        hp -= damage;
        Manager.Instance.uISettings.slider.value = hp;
        CheckHP();
    }

    private void CheckHP()
    {
        if (hp <= startingHp * 0.75f && hp > startingHp * 0.5f)
        {
            if (hb1.isPlaying)
                hb1.Stop();
            if(!hb2.isPlaying)
                hb2.Play();
            return;
        }
        if (hp <= startingHp * 0.5f && hp > startingHp * 0.25f)
        {
            if (hb2.isPlaying)
                hb2.Stop();
            if(!hb3.isPlaying)
                hb3.Play();
            return;
        }

        if(hp <= 0)
        {
            Manager.Instance.GameOver();
            Destroy(gameObject);
        }
    }
}