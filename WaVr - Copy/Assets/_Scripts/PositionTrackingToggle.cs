using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VR;

public class PositionTrackingToggle : MonoBehaviour
{
    public bool positionTracking = true;
    public void LateUpdate ()
    {
        if (!positionTracking)
        {
            transform.position = new Vector3(0, 1.5f, 0);
            transform.rotation = Quaternion.Euler(0, 0, 0);
        }
    }
}
