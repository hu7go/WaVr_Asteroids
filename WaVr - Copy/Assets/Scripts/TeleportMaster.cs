using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using VRTK;

public class TeleportMaster : MonoBehaviour
{
    public Transform headsetFollower;

    public UnityEvent StartTeleport;
    public UnityEvent CanTeleport;

    public GameObject player;
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
                previousParentRotation = player.transform.parent.rotation;
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
            player.transform.position = currentHit.TeleportPosition().position;
            player.transform.rotation = newRot;
        }
        else
        {
            if(currentHit.GetComponent<AsteroidRot>() != null)
            {
                player.transform.position = currentHit.GetComponent<AsteroidRot>().ReturnCurrentTeleportPos(currentSide).position + new Vector3(0, -.75f, 0);
                player.transform.rotation = currentHit.GetComponent<AsteroidRot>().ReturnCurrentTeleportPos(currentSide).rotation;
            }
            else
            {
                player.transform.position = currentHit.TeleportPosition().position;
                player.transform.rotation = newRot;
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
            player.transform.parent = currentHit.rotator;

            player.transform.localPosition = new Vector3(0, 0.75f, 0);
            player.transform.parent.rotation = previousParentRotation;
            for (int i = 0; i < arrowScripts.Length; i++)
                arrowScripts[i].rotator = currentHit.rotator.gameObject;

            player.transform.localRotation = Quaternion.Euler(0, 0, 0);
        }

        if (Manager.Instance.teleportVersion == Manager.TeleVersion.onTopSide || Manager.Instance.teleportVersion == Manager.TeleVersion.arrowsSide)
            player.transform.localPosition += new Vector3(2, 0, 0);

        //
        if (Manager.Instance.teleportVersion == Manager.TeleVersion.arrowsSide)
        {
            player.transform.parent = currentHit.rotator;

            player.transform.parent.rotation = previousParentRotation;
            for (int i = 0; i < arrowScripts.Length; i++)
                arrowScripts[i].rotator = currentHit.rotator.gameObject;

            player.transform.localRotation = Quaternion.Euler(0, 0, 0);

            //Vector3 tmpVector = GetClosestSide().sideOffset;
            //player.transform.localPosition = new Vector3(tmpVector.x, .7f, tmpVector.z);

            Vector3 tmpVector = GetClosestSide().transform.localPosition * 3.636363f;

            arrowPositionCheck.localPosition = new Vector3(-tmpVector.x, -tmpVector.y + .7f, -tmpVector.z);

            ArrowIndexMain(tmpVector);

            arrowsPos.localPosition = new Vector3(-tmpVector.x, -.75f, -tmpVector.z);
            Vector3 playerPos = player.transform.localPosition + new Vector3(0, -1.45f, 0);
            Vector3 direction = (playerPos - arrowsPos.localPosition).normalized;
            Quaternion lookRot = Quaternion.LookRotation(direction, Vector3.up);

            arrowsPos.localRotation = lookRot;
            arrowsPos.localRotation *= Quaternion.Euler(0, -90, 0);
        }
        //

        //
        if (Manager.Instance.teleportVersion == Manager.TeleVersion.onTop)
        {
            player.transform.localPosition -= new Vector3(0, .75f, 0);
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
        player.transform.position += new Vector3(1, 0, 1);
    }

    //This is for when we rotate to see which side of the asteroid we are standing on!
    public void ChechWhichSideIsClosest ()
    {
        currentAsteroidStandingOn.GetComponent<TurretMenuMaster>().CheckSides(this, false);
    }

    public SideScript GetClosestSide ()
    {
        List<SideScript> closestSides = currentHit.GetComponent<TurretMenuMaster>().ReturnClosestSide(this);
        SideScript newSide = new SideScript();

        //foreach (SideScript side in closestSides)
            //Debug.Log(Vector3.Distance(side.transform.position, previousHit.transform.position));

        Debug.Log(closestSides[0].sides);
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

    private void ArrowIndexMain (Vector3 vector)
    {
        Utils.ClearLogConsole();

        //Calculate which side/arrow index the player currently has
        switch (currentSide)
        {
            case Sides.up:
                if (vector.x > 0 && vector.y == 0 && vector.z == 0)
                    arrowIndex = 1;
                if (vector.x == 0 && vector.y == 0 && vector.z < 0)
                    arrowIndex = 2;
                if (vector.x < 0 && vector.y == 0 && vector.z == 0)
                    arrowIndex = 3;
                if (vector.x == 0 && vector.y == 0 && vector.z > 0)
                    arrowIndex = 4;
                break;
            case Sides.down:
                if (vector.x < 0 && vector.y == 0 && vector.z == 0)
                    arrowIndex = 1;
                if (vector.x == 0 && vector.y == 0 && vector.z < 0)
                    arrowIndex = 2;
                if (vector.x < 0 && vector.y == 0 && vector.z == 0)
                    arrowIndex = 3;
                if (vector.x == 0 && vector.y == 0 && vector.z > 0)
                    arrowIndex = 4;
                break;
            case Sides.front:
                if (vector.x > 0 && vector.y == 0 && vector.z == 0)
                    arrowIndex = 1;
                if (vector.x == 0 && vector.y > 0 && vector.z == 0)
                    arrowIndex = 2;
                if (vector.x < 0 && vector.y == 0 && vector.z == 0)
                    arrowIndex = 3;
                if (vector.x == 0 && vector.y < 0 && vector.z == 0)
                    arrowIndex = 4;
                break;
            case Sides.back:
                if (vector.x > 0 && vector.y == 0 && vector.z == 0)
                    arrowIndex = 1;
                if (vector.x == 0 && vector.y < 0 && vector.z == 0)
                    arrowIndex = 2;
                if (vector.x < 0 && vector.y == 0 && vector.z == 0)
                    arrowIndex = 3;
                if (vector.x == 0 && vector.y > 0 && vector.z == 0)
                    arrowIndex = 4;
                break;
            case Sides.left:
                if (vector.x == 0 && vector.y > 0 && vector.z == 0)
                    arrowIndex = 1;
                if (vector.x == 0 && vector.y == 0 && vector.z < 0)
                    arrowIndex = 2;
                if (vector.x == 0 && vector.y < 0 && vector.z == 0)
                    arrowIndex = 3;
                if (vector.x == 0 && vector.y == 0 && vector.z > 0)
                    arrowIndex = 4;
                break;
            case Sides.right:
                if (vector.x == 0 && vector.y < 0 && vector.z == 0)
                    arrowIndex = 1;
                if (vector.x == 0 && vector.y == 0 && vector.z < 0)
                    arrowIndex = 2;
                if (vector.x == 0 && vector.y > 0 && vector.z == 0)
                    arrowIndex = 3;
                if (vector.x == 0 && vector.y == 0 && vector.z > 0)
                    arrowIndex = 4;
                break;
        }

        switch (arrowIndex)
        {
            case 1:
                switch (currentSide)
                {
                    case Sides.up:
                        //Done
                        switch (previousSide)
                        {
                            case Sides.front:
                                ChangePlayerPos(vector.x, vector.z);
                                break;
                            case Sides.back:
                                ChangePlayerPos(vector.x, vector.z);
                                break;
                            case Sides.left:
                                ChangePlayerPos(vector.x, vector.z);
                                break;
                            case Sides.right:
                                ChangePlayerPos(vector.x, vector.z);
                                break;
                        }
                        //
                        break;
                    case Sides.down:
                        //Done
                        switch (previousSide)
                        {
                            case Sides.front:
                                ChangePlayerPos(vector.x, vector.z);
                                break;
                            case Sides.back:
                                ChangePlayerPos(vector.x, vector.z);
                                break;
                            case Sides.left:
                                ChangePlayerPos(vector.x * -1, vector.z);
                                break;
                            case Sides.right:
                                ChangePlayerPos(vector.x * -1, vector.z);
                                break;
                        }
                        //
                        break;
                    case Sides.front:
                        //Done
                        switch (previousSide)
                        {
                            case Sides.up:
                                ChangePlayerPos(vector.x, vector.z);
                                break;
                            case Sides.down:
                                ChangePlayerPos(vector.x, vector.z);
                                break;
                            case Sides.left:
                                ChangePlayerPos(vector.z, vector.x);
                                break;
                            case Sides.right:
                                ChangePlayerPos(vector.z, vector.x * - 1);
                                break;
                        }
                        //
                        break;
                    case Sides.back:
                        //Done
                        switch (previousSide)
                        {
                            case Sides.up:
                                ChangePlayerPos(vector.x, vector.z);
                                break;
                            case Sides.down:
                                ChangePlayerPos(vector.x, vector.z);
                                break;
                            case Sides.left:
                                ChangePlayerPos(vector.z, vector.x * - 1);
                                break;
                            case Sides.right:
                                ChangePlayerPos(vector.z, vector.x);
                                break;
                        }
                        //
                        break;
                    case Sides.left:
                        //Done
                        switch (previousSide)
                        {
                            case Sides.up:
                                ChangePlayerPos(vector.y, vector.z);
                                break;
                            case Sides.down:
                                ChangePlayerPos(vector.y, vector.z);
                                break;
                            case Sides.front:
                                ChangePlayerPos(vector.z, vector.y * - 1);
                                break;
                            case Sides.back:
                                ChangePlayerPos(vector.z, vector.y);
                                break;
                        }
                        //
                        break;
                    case Sides.right:
                        //Done
                        switch (previousSide)
                        {
                            case Sides.up:
                                ChangePlayerPos(vector.y * -1, vector.z);
                                break;
                            case Sides.down:
                                ChangePlayerPos(vector.y * -1, vector.z);
                                break;
                            case Sides.front:
                                ChangePlayerPos(vector.z, vector.y * - 1);
                                break;
                            case Sides.back:
                                ChangePlayerPos(vector.z, vector.y);
                                break;
                        }
                        //
                        break;
                }
                break;
            case 2:
                switch (currentSide)
                {
                    case Sides.up:
                        //Done
                        switch (previousSide)
                        {
                            case Sides.front:
                                ChangePlayerPos(vector.x, vector.z);
                                break;
                            case Sides.back:
                                ChangePlayerPos(vector.x, vector.z);
                                break;
                            case Sides.left:
                                ChangePlayerPos(vector.x, vector.z);
                                break;
                            case Sides.right:
                                ChangePlayerPos(vector.x, vector.z);
                                break;
                        }
                        //
                        break;
                    case Sides.down:
                        //Done
                        switch (previousSide)
                        {
                            case Sides.front:
                                ChangePlayerPos(vector.x, vector.z * -1);
                                break;
                            case Sides.back:
                                ChangePlayerPos(vector.x, vector.z * -1);
                                break;
                            case Sides.left:
                                ChangePlayerPos(vector.x, vector.z);
                                break;
                            case Sides.right:
                                ChangePlayerPos(vector.x, vector.z);
                                break;
                        }
                        //
                        break;
                    case Sides.front:
                        //Done
                        switch (previousSide)
                        {
                            case Sides.up:
                                ChangePlayerPos(vector.x, vector.y * -1);
                                break;
                            case Sides.down:
                                ChangePlayerPos(vector.x, vector.y * -1);
                                break;
                            case Sides.left:
                                ChangePlayerPos(vector.y, vector.z);
                                break;
                            case Sides.right:
                                ChangePlayerPos(vector.y * -1, vector.z);
                                break;
                        }
                        //
                        break;
                    case Sides.back:
                        //Done
                        switch (previousSide)
                        {
                            case Sides.up:
                                ChangePlayerPos(vector.x, vector.y);
                                break;
                            case Sides.down:
                                ChangePlayerPos(vector.x, vector.y);
                                break;
                            case Sides.left:
                                ChangePlayerPos(vector.y, vector.z);
                                break;
                            case Sides.right:
                                ChangePlayerPos(vector.y * -1, vector.z);
                                break;
                        }
                        //
                        break;
                    case Sides.left:
                        //Done
                        switch (previousSide)
                        {
                            case Sides.up:
                                ChangePlayerPos(vector.x, vector.z);
                                break;
                            case Sides.down:
                                ChangePlayerPos(vector.x, vector.z);
                                break;
                            case Sides.front:
                                ChangePlayerPos(vector.z, vector.x);
                                break;
                            case Sides.back:
                                ChangePlayerPos(vector.z * -1, vector.x);
                                break;
                        }
                        //
                        break;
                    case Sides.right:
                        //Done
                        switch (previousSide)
                        {
                            case Sides.up:
                                ChangePlayerPos(vector.x, vector.z);
                                break;
                            case Sides.down:
                                ChangePlayerPos(vector.x, vector.z);
                                break;
                            case Sides.front:
                                ChangePlayerPos(vector.z * -1, vector.x);
                                break;
                            case Sides.back:
                                ChangePlayerPos(vector.z, vector.x);
                                break;
                        }
                        //
                        break;
                }
                break;
            case 3:
                switch (currentSide)
                {
                    case Sides.up:
                        //Done
                        switch (previousSide)
                        {
                            case Sides.front:
                                ChangePlayerPos(vector.x, vector.z);
                                break;
                            case Sides.back:
                                ChangePlayerPos(vector.x, vector.z);
                                break;
                            case Sides.left:
                                ChangePlayerPos(vector.x, vector.z);
                                break;
                            case Sides.right:
                                ChangePlayerPos(vector.x, vector.z);
                                break;
                        }
                        //
                        break;
                    case Sides.down:
                        //Done
                        switch (previousSide)
                        {
                            case Sides.front:
                                ChangePlayerPos(vector.x, vector.z);
                                break;
                            case Sides.back:
                                ChangePlayerPos(vector.x, vector.z);
                                break;
                            case Sides.left:
                                ChangePlayerPos(vector.x * -1, vector.z);
                                break;
                            case Sides.right:
                                ChangePlayerPos(vector.x * -1, vector.z);
                                break;
                        }
                        //
                        break;
                    case Sides.front:
                        //Done
                        switch (previousSide)
                        {
                            case Sides.up:
                                ChangePlayerPos(vector.x, vector.z);
                                break;
                            case Sides.down:
                                ChangePlayerPos(vector.x, vector.z);
                                break;
                            case Sides.left:
                                ChangePlayerPos(vector.z, vector.x);
                                break;
                            case Sides.right:
                                ChangePlayerPos(vector.z, vector.x * -1);
                                break;
                        }
                        //
                        break;
                    case Sides.back:
                        //Done
                        switch (previousSide)
                        {
                            case Sides.up:
                                ChangePlayerPos(vector.x, vector.z);
                                break;
                            case Sides.down:
                                ChangePlayerPos(vector.x, vector.z);
                                break;
                            case Sides.left:
                                ChangePlayerPos(vector.z, vector.x * - 1);
                                break;
                            case Sides.right:
                                ChangePlayerPos(vector.z, vector.x);
                                break;
                        }
                        //
                        break;
                    case Sides.left:
                        //Done
                        switch (previousSide)
                        {
                            case Sides.up:
                                ChangePlayerPos(vector.y, vector.z);
                                break;
                            case Sides.down:
                                ChangePlayerPos(vector.y, vector.z);
                                break;
                            case Sides.front:
                                ChangePlayerPos(vector.z, vector.y * -1);
                                break;
                            case Sides.back:
                                ChangePlayerPos(vector.z, vector.y);
                                break;
                        }
                        //
                        break;
                    case Sides.right:
                        //Done
                        switch (previousSide)
                        {
                            case Sides.up:
                                ChangePlayerPos(vector.y * -1, vector.z);
                                break;
                            case Sides.down:
                                ChangePlayerPos(vector.y * -1, vector.z);
                                break;
                            case Sides.front:
                                ChangePlayerPos(vector.z, vector.y * -1);
                                break;
                            case Sides.back:
                                ChangePlayerPos(vector.z, vector.y);
                                break;
                        }
                        //
                        break;
                }
                break;
            case 4:
                switch (currentSide)
                {
                    case Sides.up:
                        //Done
                        switch (previousSide)
                        {
                            case Sides.front:
                                ChangePlayerPos(vector.x, vector.z);
                                break;
                            case Sides.back:
                                ChangePlayerPos(vector.x, vector.z);
                                break;
                            case Sides.left:
                                ChangePlayerPos(vector.x, vector.z);
                                break;
                            case Sides.right:
                                ChangePlayerPos(vector.x, vector.z);
                                break;
                        }
                        //
                        break;
                    case Sides.down:
                        //Done
                        switch (previousSide)
                        {
                            case Sides.front:
                                ChangePlayerPos(vector.x, vector.z * -1);
                                break;
                            case Sides.back:
                                ChangePlayerPos(vector.x, vector.z * -1);
                                break;
                            case Sides.left:
                                ChangePlayerPos(vector.x, vector.z);
                                break;
                            case Sides.right:
                                ChangePlayerPos(vector.x, vector.z);
                                break;
                        }
                        //
                        break;
                    case Sides.front:
                        //Done
                        switch (previousSide)
                        {
                            case Sides.up:
                                ChangePlayerPos(vector.x, vector.y * -1);
                                break;
                            case Sides.down:
                                ChangePlayerPos(vector.x, vector.y * -1);
                                break;
                            case Sides.left:
                                ChangePlayerPos(vector.y, vector.z);
                                break;
                            case Sides.right:
                                ChangePlayerPos(vector.y * -1, vector.z);
                                break;
                        }
                        //
                        break;
                    case Sides.back:
                                Debug.Log("<color=cyan>Current place in the code!!</color>");
                        switch (previousSide)
                        {
                            case Sides.up:
                                break;
                            case Sides.down:
                                break;
                            case Sides.left:
                                break;
                            case Sides.right:
                                break;
                        }
                        player.transform.localPosition = new Vector3(vector.x, .7f, vector.y);
                        break;
                    case Sides.left:
                        switch (previousSide)
                        {
                            case Sides.up:
                                break;
                            case Sides.down:
                                break;
                            case Sides.front:
                                break;
                            case Sides.back:
                                break;
                        }
                        player.transform.localPosition = new Vector3(vector.x, .7f, vector.z);
                        break;
                    case Sides.right:
                        switch (previousSide)
                        {
                            case Sides.up:
                                break;
                            case Sides.down:
                                break;
                            case Sides.front:
                                break;
                            case Sides.back:
                                break;
                        }
                        player.transform.localPosition = new Vector3(vector.x, .7f, vector.z);
                        break;
                }
                break;
        }

        Debug.Log("Arrow index: " + arrowIndex);
        Debug.Log("<color=red>X: " + vector.x + "</color>,      " + "<color=green>Y: " + vector.y + "</color>,       " + "<color=blue>Z: " + vector.z + "</color>");
        Debug.DrawLine(currentHit.transform.position, previousHit.transform.position, Color.cyan, 1000f);
    }

    private void ChangePlayerPos (float x, float z)
    {
        player.transform.localPosition = new Vector3(x, .7f, z);
    }
}