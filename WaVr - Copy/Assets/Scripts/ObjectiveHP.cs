using UnityEngine;

public class ObjectiveHP : MonoBehaviour
{
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
        startingHp = Manager.Instance.objectiveHealth;
        if (!hb1.isPlaying)
            hb1.Play();
    }

    public void TakeDamage (int damage)
    {
        Manager.Instance.objectiveHealth -= damage;
        Manager.Instance.uISettings.slider.value = Manager.Instance.objectiveHealth;
        CheckHP();
    }

    private void CheckHP()
    {
        if (Manager.Instance.objectiveHealth <= startingHp * 0.75f && Manager.Instance.objectiveHealth > startingHp * 0.5f)
        {
            if (hb1.isPlaying)
                hb1.Stop();
            if(!hb2.isPlaying)
                hb2.Play();
            return;
        }
        if (Manager.Instance.objectiveHealth <= startingHp * 0.5f && Manager.Instance.objectiveHealth > startingHp * 0.25f)
        {
            if (hb2.isPlaying)
                hb2.Stop();
            if(!hb3.isPlaying)
                hb3.Play();
            return;
        }

        if(Manager.Instance.objectiveHealth <= 0)
        {
            Manager.Instance.GameOver();
            Destroy(gameObject);
        }
    }
}