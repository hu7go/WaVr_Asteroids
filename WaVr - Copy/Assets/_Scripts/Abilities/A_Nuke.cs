using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class A_Nuke : Abilities
{
    [Space(20)]
    public GameObject nukeObj;
    public float force = 1000;

    public override void Effect(Transform hand)
    {
        GameObject newNuke = Instantiate(nukeObj, hand.position, hand.rotation);
        newNuke.GetComponent<Rigidbody>().AddForce(hand.forward * force);
        Debug.Log("Testing active ablities!");
    }
}
