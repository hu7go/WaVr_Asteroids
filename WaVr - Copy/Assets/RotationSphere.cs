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
            //if (clicked == false)
            //{
            //    handRotation = hand.localRotation;
            //    clicked = true;
            //}

            //transform.localRotation = hand.localRotation;

            //Quaternion changeRotation = transform.localRotation * Quaternion.Inverse(handRotation);

            //Quaternion lerpRot = Quaternion.Lerp(transform.localRotation, transform.localRotation * handRotation, Time.deltaTime);

            manager.ReturnPlayer().transform.rotation = transform.localRotation;
        }
        else
        {
            //if (clicked)
            //    oldRot = transform.localRotation;
            //clicked = false;
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