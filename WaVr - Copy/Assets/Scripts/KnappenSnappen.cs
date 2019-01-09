﻿using System.Linq;
using UnityEngine;

public class KnappenSnappen : MonoBehaviour {
    [SerializeField]
    private GameObject ghost;
    [SerializeField]
    private GameObject UIthing;

    [SerializeField]
    private GameObject object1, object2, object3, object4;

    //if ghost object is closer to one of the "objects" then snap the UIthing to the closest "object" to ghost
    void Update () {
        float distance1 = Vector3.Distance(ghost.transform.position, object1.transform.position);
        float distance2 = Vector3.Distance(ghost.transform.position, object2.transform.position);
        float distance3 = Vector3.Distance(ghost.transform.position, object3.transform.position);
        float distance4 = Vector3.Distance(ghost.transform.position, object4.transform.position);

        float[] nums = { distance1, distance2, distance3, distance4 };

        if (nums.Min() == distance1)
        {
            UIthing.transform.position = object1.transform.position;
        }

        else if (nums.Min() == distance2)
        {
            UIthing.transform.position = object2.transform.position;
        }

        else if (nums.Min() == distance3)
        {
            UIthing.transform.position = object3.transform.position;
        }

        else if (nums.Min() == distance4)
        {
            UIthing.transform.position = object4.transform.position;
        }
    }
}