using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotationSphere : MonoBehaviour
{
    public TeleportRotation teleportRotation;
    public bool rotate = false;

    private bool clicked = false;

    private Transform hand;
    private Manager manager;
    private Quaternion newRot = new Quaternion();

    private Quaternion handRotation = new Quaternion();
    private Quaternion oldRot = new Quaternion();


    private Quaternion oldHandRot = new Quaternion();
    private bool afterRelease = false;

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
            if (afterRelease == false)
            {
                oldHandRot = hand.localRotation;
                afterRelease = true;
            }

            transform.localRotation = Quaternion.Lerp(transform.localRotation, transform.localRotation * (hand.localRotation * Quaternion.Inverse(oldHandRot)), Time.deltaTime * 1);

            manager.ReturnPlayer().transform.rotation = transform.localRotation;
        }
        else
        {
            afterRelease = false;
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