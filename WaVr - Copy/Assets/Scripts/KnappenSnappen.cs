﻿using System.Linq;
using UnityEngine;

public class KnappenSnappen : MonoBehaviour {
    [SerializeField]
    private GameObject ghost;

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
            transform.position = object1.transform.position;
            transform.localRotation = Quaternion.Euler(52.295f, 0, 0);
        }

        else if (nums.Min() == distance2)
        {
            transform.position = object2.transform.position;
            transform.localRotation = Quaternion.Euler(52.295f, 90, 0);
        }

        else if (nums.Min() == distance3)
        {
            transform.position = object3.transform.position;
            transform.localRotation = Quaternion.Euler(52.295f, 180, 0);
        }

        else if (nums.Min() == distance4)
        {
            transform.position = object4.transform.position;
            transform.localRotation = Quaternion.Euler(52.295f, 270, 0);
        }
    }
}