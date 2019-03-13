using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotationSphere : MonoBehaviour
{
    public delegate void Function();

    [SerializeField] private int frameInterval = 10;
    //Arrows in order front, left, back, right! 
    [SerializeField] private List<ChangeSide> arrowsSide;
    [SerializeField] private List<ChangeSide> arrowsTop;
    private List<ChangeSide> currentArrows;

    private TeleportMaster tpMaster;

    public TeleportRotation teleportRotation;
    public bool rotate = false;

    private Transform hand;
    private Manager manager;
    private Quaternion oldHandRot = new Quaternion();
    private bool firstPressed = false;
    private bool afterRelease = false;

    private int numberOfRotations = 0;
    private List<Sides> rotationOrder = new List<Sides>();
    private List<Sides> prevRotation = new List<Sides>();

    private Quaternion offset = new Quaternion();
    private List<Function> rotPath = new List<Function>();

    private Transform player;

    private bool wentRight;
    private bool wentLeft;
    private bool wentForward;
    private bool wentBackward;

    private void Start()
    {
        manager = Manager.Instance;
        tpMaster = teleportRotation.master;
        player = manager.ReturnPlayer().transform;
        switch (manager.enums.teleportVersion)
        {
            case Manager.Enums.TeleVersion.arrows:
                currentArrows = arrowsTop;
                break;
            case Manager.Enums.TeleVersion.arrowsSide:
                currentArrows = arrowsSide;
                break;
        }
    }

    private void FixedUpdate()
    {
        Debug.Log(transform.localRotation);

        if (hand == null && teleportRotation.currentHand != null)
            hand = teleportRotation.currentHand.transform;

        if (rotate == true)
        {
            if (firstPressed == false)
            {
                oldHandRot = hand.localRotation;
                firstPressed = true;
                rotationOrder.Clear();
                prevRotation.Clear();
            }

            transform.localRotation = Quaternion.Lerp(transform.localRotation, transform.localRotation * (hand.localRotation * Quaternion.Inverse(oldHandRot)), Time.deltaTime * 1);

            manager.ReturnPlayer().transform.rotation = transform.localRotation;

            Vector3 rotEuler = player.localRotation.eulerAngles;

            //TODO: add a change for arrow index too!
            Vector3 lookAtTarget = transform.up;
            Vector3 testVector = Vector3.zero - lookAtTarget;
            testVector = testVector * 1;

            #region LeftAndRight
            if (Mathf.Cos(testVector.x) < .7f && right == false)
            {
                right = true;
                left = false;

                if (testVector.y < 0)
                {
                    if (testVector.x > 0)
                    {
                        rotPath.Add(currentArrows[1].Rotate);
                        leftRotation += 1;
                    }
                    else
                    {
                        rotPath.Add(currentArrows[3].Rotate);
                        rightRotation += 1;
                    }
                }
                else
                {
                    if (testVector.x < 0)
                    {
                        rotPath.Add(currentArrows[1].Rotate);
                        leftRotation += 1;
                    }
                    else
                    {
                        rotPath.Add(currentArrows[3].Rotate);
                        rightRotation += 1;
                    }
                }

            }
            if (Mathf.Cos(testVector.x) > .7f && left == false)
            {
                left = true;
                right = false;
                if (testVector.y < 0)
                {
                    if (testVector.x > 0)
                    {
                        rotPath.Add(currentArrows[3].Rotate);
                        rightRotation += 1;
                    }
                    else
                    {
                        rotPath.Add(currentArrows[1].Rotate);
                        leftRotation += 1;
                    }
                }
                else
                {
                    if (testVector.x < 0)
                    {
                        rotPath.Add(currentArrows[3].Rotate);
                        rightRotation += 1;
                    }
                    else
                    {
                        rotPath.Add(currentArrows[1].Rotate);
                        leftRotation += 1;
                    }
                }
            }
            #endregion

            #region ForwardAndBackward
            if (Mathf.Cos(testVector.z) < .7f && forward == false)
            {
                forward = true;
                backward = false;

                if (testVector.y < 0)
                {
                    if (testVector.z > 0)
                    {
                        rotPath.Add(currentArrows[2].Rotate);
                        backwardRotation += 1;
                    }
                    else
                    {
                        rotPath.Add(currentArrows[0].Rotate);
                        forwardRotation += 1;
                    }
                }
                else
                {
                    if (testVector.z < 0)
                    {
                        rotPath.Add(currentArrows[2].Rotate);
                        backwardRotation += 1;
                    }
                    else
                    {
                        rotPath.Add(currentArrows[0].Rotate);
                        forwardRotation += 1;
                    }
                }

            }
            if (Mathf.Cos(testVector.z) > .7f && backward == false)
            {
                backward = true;
                forward = false;
                if (testVector.y < 0)
                {
                    if (testVector.z > 0)
                    {
                        rotPath.Add(currentArrows[0].Rotate);
                        forwardRotation += 1;
                    }
                    else
                    {
                        rotPath.Add(currentArrows[2].Rotate);
                        backwardRotation += 1;
                    }
                }
                else
                {
                    if (testVector.z < 0)
                    {
                        rotPath.Add(currentArrows[0].Rotate);
                        forwardRotation += 1;
                    }
                    else
                    {
                        rotPath.Add(currentArrows[2].Rotate);
                        backwardRotation += 1;
                    }
                }
            }
            #endregion

            afterRelease = false;
        }
        else
        {
            //Reset the rotation stuffs!
            if (afterRelease == false)
            {
                player.localRotation = Quaternion.Euler(0, 0, 0);

                for (int i = 0; i < rotPath.Count; i++)
                {
                    rotPath[i]();
                }

                transform.localRotation = player.localRotation;

                afterRelease = true;
                numberOfRotations = 0;

                leftRotation = 0;
                rightRotation = 0;
                backwardRotation = 0;
                forwardRotation = 0;
            }
            firstPressed = false;
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

    int rightRotation = 0;
    int leftRotation = 0;
    int forwardRotation = 0;
    int backwardRotation = 0;

    [Header("Gizmos bools")]
    [Space(20)]
    public bool resetRotNumber = false;
    public bool drawGizmos = true;

    private bool left = false;
    private bool right = false;
    private bool forward = false;
    private bool backward = false;


    private void OnDrawGizmos()
    {
        if (drawGizmos == false)
            return;

        Vector3 lookAtTarget = transform.up;
        Vector3 testVector = Vector3.zero - lookAtTarget;
        testVector = testVector * 1;
        Gizmos.color = Color.yellow;
        Gizmos.DrawLine(Vector3.zero, lookAtTarget);

        Utils.ClearLogConsole();
        Debug.Log(testVector);
        Debug.Log(lookAtTarget);

        #region LeftAndRight
        if (Mathf.Cos(testVector.x) < .7f && right == false)
        {
            right = true;
            left = false;

            if (testVector.y < 0)
            {
            }
            else
            {
            }

        }
        if (Mathf.Cos(testVector.x) > .7f && left == false)
        {
            left = true;
            right = false;
            if (testVector.y < 0)
            {
            }
            else
            {
            }
        }
        #endregion

        #region ForwardAndBackward
        if (Mathf.Cos(testVector.z) < .7f && forward == false)
        {
            forward = true;
            backward = false;

            if (testVector.y < 0)
            {
            }
            else
            {
            }

        }
        if (Mathf.Cos(testVector.z) > .7f && backward == false)
        {
            backward = true;
            forward = false;
            if (testVector.y < 0)
            {
            }
            else
            {
            }
        }
        #endregion

        if (resetRotNumber)
        {
            rightRotation = 0;
            leftRotation = 0;
            forwardRotation = 0;
            backwardRotation = 0;
        }

        Gizmos.color = Color.white;
        Gizmos.DrawWireSphere(Vector3.zero, 1);
        Gizmos.DrawCube(Vector3.zero, Vector3.one / 10);
        Gizmos.color = Color.green;
        Gizmos.DrawLine(Vector3.zero, Vector3.zero + Vector3.up);
        Gizmos.color = Color.red;
        Gizmos.DrawLine(Vector3.zero, Vector3.zero + Vector3.right);
        Gizmos.color = Color.blue;
        Gizmos.DrawLine(Vector3.zero, Vector3.zero + Vector3.forward);

        Debug.Log("Right rot " + rightRotation);
        Debug.Log("Left rot " + leftRotation);
        Debug.Log("Forward rot " + forwardRotation);
        Debug.Log("Backward rot " + backwardRotation);
    }
}