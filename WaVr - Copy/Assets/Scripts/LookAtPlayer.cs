using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LookAtPlayer : MonoBehaviour
{
    public enum SignType
    {
        upRight,
        upsideDown,
        lyingLeft,
        lyingRight,
        lyingForward
    }

    public SignType signType;
    public Transform target;

	void Update ()
    {
        switch (signType)
        {
            case SignType.upRight:
                transform.LookAt(target, Vector3.up);
                break;
            case SignType.upsideDown:
                transform.LookAt(target, Vector3.down);
                break;
            case SignType.lyingLeft:
                transform.LookAt(target, Vector3.left);
                break;
            case SignType.lyingRight:
                transform.LookAt(target, Vector3.right);
                break;
            case SignType.lyingForward:
                transform.LookAt(target, Vector3.forward);
                break;
            default:
                break;
        }
    }
}