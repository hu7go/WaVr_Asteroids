using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotationSphere : MonoBehaviour
{
    public TeleportRotation teleportRotation;
    public bool rotate = false;

    private Transform hand;
    private Manager manager;
    private Quaternion tmpQuat = new Quaternion();

    private void Start()
    {
        manager = Manager.Instance;
    }

    private void FixedUpdate()
    {
        if (hand == null && teleportRotation.currentHand != null)
            hand = teleportRotation.currentHand.transform;

        if (rotate == true)
        {
            transform.localRotation = hand.localRotation;
            tmpQuat = transform.localRotation;
            manager.ReturnPlayer().transform.rotation = tmpQuat;
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