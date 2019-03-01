using UnityEngine;

[CreateAssetMenu(fileName = "Enemy", menuName = "ScriptableObject/Enemies/Enemy")]
public class Enemies : ScriptableObject
{
    public new string name;
    public bool boss;
    public float damage;
    public float health;
    public float speed;
    public float fireRate;
    public float range;
    public GameObject enemy;
    public GameObject bullet;
    public SpaceGun.BulletType bulletType;
}