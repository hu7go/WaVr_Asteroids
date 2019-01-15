using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using VRTK;
using System.Linq;

public class TeleportMaster : MonoBehaviour
{
    public Transform headsetFollower;

    public UnityEvent StartTeleport;
    public UnityEvent CanTeleport;

    public GameObject playerParent;
    public GameObject player;
    public Transform[] indexPos;
    [Space(20)]
    [Tooltip("The position that s looked at when checkin which side the player is currently standing on")]
    public Transform arrowPositionCheck;
    private Quaternion previousParentRotation;
    public Transform arrowsPos;
    public GameObject[] arrows;
    public ChangeSide[] arrowScripts;
    public bool ghostLine;
    public bool arrowsTeleport;

    [SerializeField] private float teleportMaxLenght = 100f;

    /*
    [SerializeField] public bool dash = false;
    [SerializeField] protected float dashSpeed = 50;
    */
    //[SerializeField] protected bool reverseRotation;

    [Space(20)]
    public Sides currentSide = Sides.up;
    public Sides previousSide = Sides.right;
    [SerializeField] public bool reverseTeleport = false;
    public bool newWay;
    public bool onlyUp;

    [HideInInspector] public Vector3 newPos;
    [HideInInspector] public Quaternion newRot;
    private float tempX, tempY, tempZ;

    [HideInInspector] public SideScript currentHit;
    [HideInInspector] public SideScript previousHit;
    //The asteroid you are currently standing on!
    [HideInInspector] public SideScript currentAsteroidStandingOn;

    public VRTK_StraightPointerRenderer pointer;
    VRTK_InteractUse use;
    VRTK_InteractGrab grab;

    [Space(20)]
    public float fadeTime = .5f;
    public SideScript firstAsteroid;

    [Space(20), SerializeField] public int arrowIndex = 1;

    private void Start()
    {
        currentAsteroidStandingOn = firstAsteroid;
        currentHit = firstAsteroid;

        use = pointer.GetComponent<VRTK_InteractUse>();
        grab = pointer.GetComponent<VRTK_InteractGrab>();
        previousHit = firstAsteroid;

        //! WARNING, this clears ALL log messages on start, comment out the line below if you need to debug something on start.
        Invoke("ClearConsole", .01f);
    }

    public void ClearConsole ()
    {
        Utils.ClearLogConsole();
    }

    public void Update()
    {
        if (GvrControllerInput.ClickButtonUp)
        {
            StartTeleport.Invoke();
            pointer.Toggle(true, false);
        }
        if (GvrControllerInput.ClickButtonDown)
        {
            CanTeleport.Invoke();
            pointer.Toggle(false, true);
        }
        if (GvrControllerInput.AppButton)
        {
            use.AttemptUse();
            grab.AttemptGrab();
        }
    }

    public void CheckSides(RaycastHit hit, Sides otherSide = Sides.up)
    {
        if (hit.collider.GetComponent<SideScript>() != null)
        {
            if(!ghostLine)
                currentHit = hit.collider.GetComponent<SideScript>();

            Vector3 tempRot = newRot.eulerAngles;
            tempX = tempRot.x;
            tempY = tempRot.y;
            tempZ = tempRot.z;
            if (newWay)
                switch (currentSide)
                {
                    case Sides.up:
                        switch (otherSide)
                        {
                            case Sides.up:
                                //Default
                                break;
                            case Sides.down:
                                Down();
                                break;
                            case Sides.front:
                                Front();
                                break;
                            case Sides.back:
                                Back();
                                break;
                            case Sides.left:
                                tempX = 90;
                                if (reverseTeleport)
                                    tempX = tempY = tempZ = -90;
                                break;
                            case Sides.right:
                                tempX = 270;
                                if (reverseTeleport)
                                {
                                    tempX = tempY = 90;
                                    tempZ = -90;
                                }
                                break;
                            default:
                                break;
                        }
                        break;

                    case Sides.down:
                        switch (otherSide)
                        {
                            case Sides.up:
                                UP();
                                break;
                            case Sides.down:
                                break;
                            case Sides.front:
                                Front();
                                break;
                            case Sides.back:
                                Back();
                                break;
                            case Sides.left:
                                tempX = -90;
                                tempY = 180;
                                tempZ = 0;
                                if (reverseTeleport)
                                {
                                    tempX = 90;
                                    tempZ = 180;
                                }
                                break;
                            case Sides.right:
                                tempX = tempY = 90;
                                tempZ = -90;
                                if (reverseTeleport)
                                    tempX = -90;
                                break;

                            default:
                                break;
                        }
                        break;

                    case Sides.front:
                        switch (otherSide)
                        {
                            case Sides.up:
                                UP();
                                break;
                            case Sides.down:
                                Down();
                                break;
                            case Sides.front:
                                break;
                            case Sides.back:
                                Back();
                                break;
                            case Sides.left:
                                tempX = 0;
                                tempY = tempZ = 270;
                                if (reverseTeleport)
                                    tempY = tempZ = 90;
                                break;
                            case Sides.right:
                                tempX = 0;
                                tempY = 90;
                                tempZ = 270;
                                if (reverseTeleport)
                                {
                                    tempY = 270;
                                    tempZ = 90;
                                }
                                break;
                            default:
                                break;
                        }
                        break;

                    case Sides.back:
                        switch (otherSide)
                        {
                            case Sides.up:
                                UP();
                                break;
                            case Sides.down:
                                Down();
                                break;
                            case Sides.front:
                                Front();
                                break;
                            case Sides.back:
                                break;
                            case Sides.left:
                                tempX = 0;
                                tempY = tempZ = 90;
                                if (reverseTeleport)
                                    tempY = tempZ = 270;
                                break;
                            case Sides.right:
                                tempX = 0;
                                tempY = 270;
                                tempZ = 90;
                                if (reverseTeleport)
                                {
                                    tempY = 90;
                                    tempZ = 270;
                                }
                                break;
                            default:
                                break;
                        }
                        break;

                    case Sides.left:
                        switch (otherSide)
                        {
                            case Sides.up:
                                UP();
                                break;
                            case Sides.down:
                                Down();
                                break;
                            case Sides.front:
                                Front();
                                break;
                            case Sides.back:
                                Back();
                                break;
                            case Sides.left:
                                break;
                            case Sides.right:
                                tempX = 180;
                                tempY = -90;
                                tempZ = 90;
                                if (reverseTeleport)
                                {
                                    tempY = 90;
                                    tempZ = -90;
                                }
                                break;
                            default:
                                break;
                        }
                        break;

                    case Sides.right:
                        switch (otherSide)
                        {
                            case Sides.up:
                                UP();
                                break;
                            case Sides.down:
                                Down();
                                break;
                            case Sides.front:
                                Front();
                                break;
                            case Sides.back:
                                Back();
                                break;
                            case Sides.left:
                                tempX = 90;
                                tempY = tempZ = 0;
                                if (reverseTeleport)
                                {
                                    tempX = 270;
                                    tempZ = 180;
                                }
                                break;
                            case Sides.right:
                                break;
                            default:
                                break;
                        }
                        break;
                    default:
                        break;
                }
            tempRot = new Vector3(tempX, tempY, tempZ);
            if (!arrowsTeleport)
                newRot = Quaternion.Euler(tempRot);
            if (arrowsTeleport)
                previousParentRotation = playerParent.transform.parent.rotation;
            Teleport();
        }
        else
            return;
    }

    private void UP()
    {
        tempX = tempY = tempZ = 0;
        if (reverseTeleport)
            tempY = 180;
    }

    private void Front()
    {
        tempX = tempY = 180;
        tempZ = 90;
        if (reverseTeleport)
            tempX = 0;
    }

    private void Down()
    {
        tempX = tempY = 0;
        tempZ = 180;
        if (reverseTeleport)
            tempY = 180;
    }

    private void Back()
    {
        tempX = tempY = 0;
        tempZ = 90;
        if (reverseTeleport)
            tempX = 180;
    }

    protected void Teleport()
    {
        GetComponent<VRTK_HeadsetFade>().Fade(Color.black, fadeTime);
        Invoke("ActualTeleport", fadeTime);
    }

    void ActualTeleport ()
    {
        if (!arrowsTeleport)
        {
            previousSide = currentSide;
            currentSide = currentHit.sides;
        }
        if (onlyUp)
        {
            playerParent.transform.position = currentHit.TeleportPosition().position;
            playerParent.transform.rotation = newRot;
        }
        else
        {
            if(currentHit.GetComponent<AsteroidRot>() != null)
            {
                playerParent.transform.position = currentHit.GetComponent<AsteroidRot>().ReturnCurrentTeleportPos(currentSide).position + new Vector3(0, -.75f, 0);
                playerParent.transform.rotation = currentHit.GetComponent<AsteroidRot>().ReturnCurrentTeleportPos(currentSide).rotation;
            }
            else
            {
                playerParent.transform.position = currentHit.TeleportPosition().position;
                playerParent.transform.rotation = newRot;
            }
        }
        //if (arrowsTeleport)
        //{
        //    player.transform.rotation = newRot;
        //}

        //TODO: fix this!!! it should only happen in the version where it is needed
        //if (Manager.Instance.teleportVersion != Manager.TeleVersion.anywhere)
            //player.transform.parent = currentHit.rotator; ///VI FÅR INTE EN NY ROTATOR UTAN VI ÄR KVAR UNDER DEN FÖRSTA.

        if (Manager.Instance.teleportVersion == Manager.TeleVersion.arrows)
        {
            playerParent.transform.parent = currentHit.rotator;

            playerParent.transform.localPosition = new Vector3(0, 0.75f, 0);
            playerParent.transform.parent.rotation = previousParentRotation;
            for (int i = 0; i < arrowScripts.Length; i++)
                arrowScripts[i].rotator = currentHit.rotator.gameObject;

            playerParent.transform.localRotation = Quaternion.Euler(0, 0, 0);
        }

        if (Manager.Instance.teleportVersion == Manager.TeleVersion.onTopSide || Manager.Instance.teleportVersion == Manager.TeleVersion.arrowsSide)
            playerParent.transform.localPosition += new Vector3(2, 0, 0);

        //
        if (Manager.Instance.teleportVersion == Manager.TeleVersion.arrowsSide)
        {
            playerParent.transform.parent = currentHit.rotator;

            playerParent.transform.parent.rotation = previousParentRotation;
            for (int i = 0; i < arrowScripts.Length; i++)
                arrowScripts[i].rotator = currentHit.rotator.gameObject;

            playerParent.transform.localRotation = Quaternion.Euler(0, 0, 0);

            //Vector3 tmpVector = GetClosestSide().sideOffset;
            //player.transform.localPosition = new Vector3(tmpVector.x, .7f, tmpVector.z);

            Vector3 tmpVector = GetClosestSide(previousHit.transform.position).transform.localPosition * 3.636363f;

            arrowPositionCheck.localPosition = new Vector3(-tmpVector.x, -tmpVector.y + .7f, -tmpVector.z);

            playerParent.transform.position = currentHit.transform.position;

            //
            indexPos = indexPos.OrderBy(x => Vector3.Distance(previousHit.transform.position, x.position)).ToArray();
            player.transform.position = indexPos[0].position;
            //

            arrowIndex = indexPos[0].GetComponent<IndexNode>().index;
            Debug.Log(arrowIndex);

            arrowsPos.transform.LookAt(indexPos[0].transform.position - new Vector3(0, .75f, 0), Vector3.up);
            arrowsPos.transform.localRotation *= Quaternion.Euler(0, -90, 0);
        }
        //

        //
        if (Manager.Instance.teleportVersion == Manager.TeleVersion.onTop)
        {
            playerParent.transform.localPosition -= new Vector3(0, .75f, 0);
        }

        GetComponent<VRTK_HeadsetFade>().Unfade(fadeTime);

        ghostLine = false;

        Manager.Instance.teleportOffset = currentHit.transform.position;
        Manager.Instance.UpdateAsteroidLine();
    }

    public void AsteroidToBuildOn ()
    {
        if (currentHit.GetComponent<TurretMenuMaster>() == null)
            currentHit.GetComponentInParent<TurretMenuMaster>().CheckSides(this);
        else
            currentHit.GetComponent<TurretMenuMaster>().CheckSides(this);
    }

    public void RemoveTurretButtonsOnAsteroid ()
    {
        if (currentHit.GetComponent<TurretMenuMaster>() != null)
            currentHit.GetComponent<TurretMenuMaster>().RemoveButtons();
    }

    public void SetHit (RaycastHit newHit)
    {
        if (newHit.collider.GetComponent<SideScript>() != null)
            currentHit = newHit.collider.GetComponent<SideScript>();
    }

    public void IncreaseMaxLenght ()
    {
        teleportMaxLenght += 10;
    }

    public void ReseMaxLenght ()
    {
        teleportMaxLenght = 35;
    }

    public float GetMaxLenght()
    {
        return teleportMaxLenght;
    }

    public void DDTest()
    {
        playerParent.transform.position += new Vector3(1, 0, 1);
    }

    //This is for when we rotate to see which side of the asteroid we are standing on!
    public void ChechWhichSideIsClosest ()
    {
        currentAsteroidStandingOn.GetComponent<TurretMenuMaster>().CheckSides(this, false);
    }

    public SideScript GetClosestSide (Vector3 posToCompare)
    {
        List<SideScript> closestSides = currentHit.GetComponent<TurretMenuMaster>().ReturnClosestSide(posToCompare);
        SideScript newSide = new SideScript();

        //foreach (SideScript side in closestSides)
            //Debug.Log(Vector3.Distance(side.transform.position, previousHit.transform.position));

        //Debug.Log(closestSides[0].sides);
        if (closestSides[0].sides == currentSide || TMP(closestSides[0].sides) == false)
            newSide = closestSides[1];
        else
            newSide = closestSides[0];

        return newSide;
    }

    public bool TMP (Sides newSide)
    {
        switch (newSide)
        {
            case Sides.up:
                if (currentSide == Sides.down)
                    return false;
                else
                    return true;
            case Sides.down:
                if (currentSide == Sides.up)
                    return false;
                else
                    return true;
            case Sides.front:
                if (currentSide == Sides.back)
                    return false;
                else
                    return true;
            case Sides.back:
                if (currentSide == Sides.front)
                    return false;
                else
                    return true;
            case Sides.left:
                if (currentSide == Sides.right)
                    return false;
                else
                    return true;
            case Sides.right:
                if (currentSide == Sides.left)
                    return false;
                else
                    return true;
            default:
                break;
        }
        return true;
    }

    private void ChangePlayerPos (float x, float z)
    {
        playerParent.transform.localPosition = new Vector3(x, .7f, z);
    }
}