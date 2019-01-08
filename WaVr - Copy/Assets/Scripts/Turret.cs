using UnityEngine;

[CreateAssetMenu()]
public class Turret : ScriptableObject
{
    public float damage;
    public float rangeRadius;
    public float attackSpeed;
    public float rotationSpeed;

    [Space(20)]
    public GameObject model;
}