using UnityEngine;

public class A_Nuke : Abilities
{
    [Space(20)]
    public GameObject nukeObj;

    public override void Effect(Transform hand)
    {
        GameObject newNuke = ObjectPooler.Instance.SpawnFromPool("Nuke", hand.position, hand.rotation);
    }
}