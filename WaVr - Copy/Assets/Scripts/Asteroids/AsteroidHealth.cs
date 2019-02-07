using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AsteroidHealth : MonoBehaviour
{
    public AsteroidInfo asteroid;
    private MeshRenderer rend;

    float h;
    float s;
    float v;

    public void Start()
    {
        asteroid.postition = transform.position;
        asteroid.health = 100;
        asteroid.alive = true;

        rend = GetComponent<MeshRenderer>();
        Color.RGBToHSV(rend.material.GetColor("_Color"), out h, out s, out v);
    }

    public void TakeDamage (int damage)
    {
        asteroid.health -= damage;
        if (asteroid.health <= 0)
            asteroid.alive = false;
        Manager.Instance.objectiveHealth -= damage;
        Manager.Instance.uISettings.slider.value = Manager.Instance.Masterhealth;
        UpdateColor();
        CheckHP();
    }

    void UpdateColor ()
    {
        s = (float)asteroid.health / 100;
        rend.material.SetColor("_Color", Color.HSVToRGB(h, s, v)); 
    }

    public AsteroidInfo GetInfo ()
    {
        return asteroid;
    }

    private void CheckHP()
    {
        if (Manager.Instance.Masterhealth <= 0)
            Manager.Instance.GameOver();
    }
}

[System.Serializable]
public struct AsteroidInfo
{
    public Vector3 postition;
    [Range(0 , 100)]
    public int health;
    public bool alive;
}
