using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeadsetRotation : MonoBehaviour
{
    public Transform headset;
    public float minAngle = 40;

    private TeleportMaster teleportMaster;
    private bool rotated;

    public ChangeSide[] arrows;

    private void Start()
    {
        teleportMaster = GetComponent<TeleportMaster>();
    }

    private void Update()
    {
        if ((headset.localRotation.eulerAngles.x >= minAngle && headset.localRotation.eulerAngles.x <= 180) && !rotated)
        {
            rotated = true;
            StartCoroutine(Test());
            arrows[0].DoFunction();
            return;
        }
        if ((headset.localRotation.eulerAngles.x <= 360 - minAngle && headset.localRotation.eulerAngles.x >= 180) && !rotated)
        {
            rotated = true;
            StartCoroutine(Test());
            arrows[1].DoFunction();
            return;
        }
        if ((headset.localRotation.eulerAngles.z >= minAngle && headset.localRotation.eulerAngles.z <= 180) && !rotated)
        {
            rotated = true;
            StartCoroutine(Test());
            arrows[2].DoFunction();
            return;
        }
        if ((headset.localRotation.eulerAngles.z <= 360 - minAngle && headset.localRotation.eulerAngles.z >= 180) && !rotated)
        {
            rotated = true;
            arrows[3].DoFunction();
            StartCoroutine(Test());
            return;
        }
    }

    IEnumerator Test ()
    {
        yield return new WaitForSeconds(1f);
        rotated = false;
    }
}
