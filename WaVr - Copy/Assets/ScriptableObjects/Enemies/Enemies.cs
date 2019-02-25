using UnityEngine;

[CreateAssetMenu()]
public class Enemies : ScriptableObject
{
    public new string name;
    public bool boss;
    public GameObject enemie;
    public float damage;
    public float health;
    public float speed;
    public Vector3 scale;
    public Mesh mesh;
}