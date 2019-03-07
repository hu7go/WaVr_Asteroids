using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotationSphere : MonoBehaviour
{
    public TeleportRotation teleportRotation;
    private bool rotate = false;

    private Transform hand;

    private void Update()
    {
        if (hand == null && teleportRotation.currentHand != null)
            hand = teleportRotation.currentHand.transform;

        if (rotate == true)
        {
            Quaternion tmp = transform.localRotation;
            transform.localRotation = Quaternion.Lerp(tmp, hand.localRotation, Time.deltaTime * 1);
            Manager.Instance.ReturnPlayer().transform.rotation = transform.localRotation;
        }
        else
        {
        }
    }

    public void StartRotate ()
    {
        rotate = true;
    }

    public void StopRotate ()
    {
        rotate = false;
    }
}
