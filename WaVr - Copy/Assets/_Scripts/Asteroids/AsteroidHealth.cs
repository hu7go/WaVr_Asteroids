using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AsteroidHealth : MonoBehaviour, ITakeDamage<float>
{
    public AsteroidInfo asteroid;
    private MeshRenderer rend;
    [HideInInspector] public Healer myHealer;
    private TurretMenuMaster turretMaster;
    Color colorStart;
    Color colorEnd;
    Color switcher;
    Color red;
    float h;
    float s;
    float v;
    float rate;
    float i;
    bool healing;
    bool takingDamage;
    private Renderer renderers;
    public void Start()
    {
        colorStart = new Color(h, s, v);
        colorEnd = new Color(h, s, v);
        red = new Color(360, 100, 50);
        renderers = GetComponent<Renderer>();


        turretMaster = GetComponentInParent<TurretMenuMaster>();

        asteroid = new AsteroidInfo(transform.position, Manager.Instance.tAe.asteroidHealth, true, false);

        rend = GetComponent<MeshRenderer>();
        Color.RGBToHSV(rend.material.GetColor("_Color"), out h, out s, out v);
    }

    bool tmp = false;
    void Update()
    {
        if (healing)
        {
            i += Time.deltaTime * rate;
            colorEnd = new Color(h, s * (Manager.Instance.tAe.asteroidHealth / 100) * 100, v);
            renderers.material.color = Color.Lerp(colorStart, colorEnd, Mathf.PingPong(i * 2, 1));
            if (i >= 2)
                i = 0;
            renderers.material.color = Color.Lerp(colorEnd, colorStart, Mathf.PingPong(i * 2, 1));
            if (i >= 2)
                i = 0;
        }
        if (takingDamage)
        {
            i += Time.deltaTime * rate;
            colorEnd = new Color(h, s * (Manager.Instance.tAe.asteroidHealth / 100) * 100, v);
            renderers.material.color = Color.Lerp(colorEnd, red, Mathf.PingPong(i * 2, 1));
            if (i >= 2)
                i = 0;
            colorEnd = new Color(h, s * (Manager.Instance.tAe.asteroidHealth / 100) * 100, v);
            renderers.material.color = Color.Lerp(red, colorEnd, Mathf.PingPong(i * 2, 1));
            if (i >= 2)
                i = 0;
        }
    }
    public void TakeDamage(float damage)
    {
        if (asteroid.beingHealed == true)
        {
            myHealer.TakeDamage(damage);
            return;
        }

        asteroid.health -= damage;
        if (asteroid.health <= 0)
        {
            turretMaster.AsteroidDied();
            asteroid.alive = false;
            if (tmp == false)
            {
                Manager.Instance.UpdatePath(transform.position);
                tmp = true;
            }
        }

        Manager.Instance.UpdateHealth(-damage);

        UpdateColor();
    }
    public IEnumerator HealingVisual()
    {
        healing = true;
        yield return new WaitForSeconds(8);
        healing = false;
    }
    public IEnumerator DamageVisual()
    {
        takingDamage = true;
        yield return new WaitForSeconds(8);
        takingDamage = false;
    }
    public void Heal(float newHealth)
    {
        Manager.Instance.UpdateHealth(newHealth);
        StartCoroutine(HealingVisual());
        UpdateColor();
    }

    void UpdateColor()
    {
        s = (asteroid.health / 100) / (Manager.Instance.tAe.asteroidHealth / 100);
        rend.material.SetColor("_Color", Color.HSVToRGB(h, s, v));
    }

    public AsteroidInfo GetInfo()
    {
        return asteroid;
    }
}

[System.Serializable]
public struct AsteroidInfo
{
    public Vector3 postition;
    public float health;
    public bool alive;
    public bool beingHealed;

    public AsteroidInfo(Vector3 postition, float health, bool alive, bool beingHealed)
    {
        this.postition = postition;
        this.health = health;
        this.alive = alive;
        this.beingHealed = beingHealed;
    }
}