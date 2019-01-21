using System.Collections.Generic;
using UnityEngine;
using VRTK;

public class TeleportRotation : MonoBehaviour
{
    public TeleportMaster master;

    [Space(20)]
    public List<GameObject> hands;
    public GameObject[] parents = new GameObject[2];
    [SerializeField] private GameObject teleportorBuildUI;
    private GameObject currentHand;

    private bool canTeleport = false;
    private bool switchedHands;
    private VRTK_CustomRaycast cr;
    RaycastHit hit;
    RaycastHit previousHit;
    RaycastHit asteroidHit;
    RaycastHit prevAsteroidHit;
    RaycastHit buildButtonTarget;
    RaycastHit tempPrevAsteroidHit;
    RaycastHit tempAsteroidHit;
    RaycastHit ghostLineHit;
    //Maybe does nothing!
    RaycastHit ghostLinePrevHit;
    Ray ray;
    private bool highlightCubes = false;

    LineRenderer line;
    Vector3[] linePos;
    public Transform lineEnd;
    public bool renderOwnLine = true;

    RaycastHit tmpPrev = new RaycastHit();

    public LayerMask startLayerMask;
    RaycastHit startHit;
    RaycastHit lineNotHit;

    public enum LineVersion
    {
        nothing,
        outOfRange,
        hit
    }
    public LineVersion lineVersion;
    
    private void Start()
    {
        if (cr == null)
            cr = GetComponent<VRTK_CustomRaycast>();

        line = GetComponent<LineRenderer>();
        linePos = new Vector3[2];
        UpdateLineRenderer();

        if (Physics.Raycast(master.firstAsteroid.transform.position + new Vector3(2, 0, 0), master.firstAsteroid.transform.position + new Vector3(-5, 0, 0), out hit))
            prevAsteroidHit = hit;
        if (Manager.Instance.enums.teleportVersion == Manager.Enums.TeleVersion.anywhere)
            highlightCubes = true;
        else
            highlightCubes = false;
    }

    public void Update()
    {
        if (!switchedHands)
            SwtichHands();

        if (renderOwnLine)
        {
            if (line != null)
            {
                linePos[1] = currentHand.transform.position;
                linePos[0] = lineEnd.position;
            }
        }

        //If we hit the start button!
        if (canTeleport)
        {
            if (Physics.Raycast(currentHand.transform.position, currentHand.transform.TransformDirection(Vector3.forward), out startHit, master.GetMaxLenght(), startLayerMask))
                Manager.Instance.StartGame();
        }
        //

        //This handles all the teleporting and clicking of buttons!
        if (canTeleport && Manager.Instance.StartedGame() && !Manager.Instance.turretsAndEnemies.holdingGun)
        {
            TeleportStuff ();
        }
    }

    //This function checks which VR headset you are currently using!
    private void SwtichHands()
    {
        if (parents[0] == enabled && parents[0].activeInHierarchy)
        {
            currentHand = hands[0];
            switchedHands = true;
            //renderOwnLine = false;
            return;
        }
        if (parents[1] == enabled && parents[1].activeInHierarchy)
        {
            currentHand = hands[1];
            switchedHands = true;
            //renderOwnLine = false;
            return;
        }
        //Using DayDream!
        if (parents[2] == enabled && parents[2].activeInHierarchy)
        {
            if (hands[2] != null)
            {
                Manager.Instance.UsingDayDream();
                renderOwnLine = true;

                currentHand = hands[2];
                switchedHands = true;
                return;
            }
        }
        else
            return;
    }

    private void TeleportStuff ()
    {
        if (renderOwnLine)
        {
            line.enabled = true;
            line.SetPositions(linePos);
        }

        ray = new Ray(currentHand.transform.position, currentHand.transform.TransformDirection(Vector3.forward));

        Physics.Raycast(currentHand.transform.position, currentHand.transform.TransformDirection(Vector3.forward), out hit, master.GetMaxLenght());
        Physics.Raycast(currentHand.transform.position, currentHand.transform.TransformDirection(Vector3.forward), out lineNotHit, master.GetMaxLenght() * 20);

        cr.CustomRaycast(ray, out hit, master.GetMaxLenght());

        if (renderOwnLine)
        {
            if (hit.collider != null)
                SetLineEnd(hit);

            if (hit.collider == null && lineNotHit.collider == null)
                lineVersion = LineVersion.nothing;
            if (hit.collider == null && lineNotHit.collider != null)
                lineVersion = LineVersion.outOfRange;
            if (hit.collider != null)
                lineVersion = LineVersion.hit;

            ChangeLineVersion();
        }

        if (Manager.Instance.turretsAndEnemies.turretHover)
        {
            int layerMask = 1 << 13;
            if (Physics.Raycast(currentHand.transform.position, currentHand.transform.TransformDirection(Vector3.forward), out buildButtonTarget, master.GetMaxLenght(), layerMask))
            {
                if (tmpPrev.collider != null)
                {
                    if (buildButtonTarget.collider != tmpPrev.collider)
                    {
                        tmpPrev.collider.GetComponent<TurretSpawn>().DisableRangeIndicator();
                    }
                }

                buildButtonTarget.collider.GetComponent<TurretSpawn>().ShowRangeIndicator();
                tmpPrev = buildButtonTarget;
            }
            else
            {
                if (tmpPrev.collider != null)
                    tmpPrev.collider.GetComponent<TurretSpawn>().DisableRangeIndicator();
            }
        }

        if (highlightCubes)
        {
            if (hit.collider != null)
                if (hit.collider.GetComponentInChildren<CubeHighlight>() != null)
                {
                    if (previousHit.collider != null)
                        if (previousHit.collider != hit.collider)
                            previousHit.collider.GetComponentInChildren<CubeHighlight>().StopRender();

                    previousHit = hit;
                    previousHit.collider.GetComponentInChildren<CubeHighlight>().Render();
                }
            if (hit.collider == null)
                if (previousHit.collider != null)
                    previousHit.collider.GetComponentInChildren<CubeHighlight>().StopRender();
        }
    }

    //This happens when let go of the trigger on the controller to either teleport or click on a button!
    public void StartTeleport()
    {
        if (hit.collider)
        {
            //If we hit the teleport state button!
            if (hit.collider.CompareTag("TeleportState"))
                Manager.Instance.SetPointerState(Manager.Enums.PointerState.Teleport);
            //

            //If we hit the build state button!
            if (hit.collider.CompareTag("BuildState"))
                Manager.Instance.SetPointerState(Manager.Enums.PointerState.Build);
            //

            //If we hit the rotate state button!
            if (hit.collider.CompareTag("RotateState"))
                Manager.Instance.SetPointerState(Manager.Enums.PointerState.Rotate);
            //

            //If we hit a pizza!
            if (hit.collider.CompareTag("Pizza"))
            {
                //Do some pizza shit
                Manager.Instance.UpdatePizzaCounter();
                hit.collider.gameObject.SetActive(false);
                Manager.Instance.ReturnPlayer().GetComponent<AudioSource>().Play();
                Manager.Instance.RefreshGun();
                hit = new RaycastHit();
                return;
            }
            //

            //If we hit the ghost line to the previous asteroid
            if (hit.collider.CompareTag("GhostLine"))
            {
                master.newPos = prevAsteroidHit.point;
                master.newRot = prevAsteroidHit.collider.transform.rotation;
                master.ghostLine = true;

                master.SetHit(prevAsteroidHit);

                RaycastHit temporary;
                temporary = prevAsteroidHit;
                prevAsteroidHit = ghostLineHit;
                ghostLineHit = temporary;
                asteroidHit = ghostLineHit;

                master.previousHit = prevAsteroidHit.collider.GetComponent<SideScript>();

                TeleportTrue(true);
            }
            //

            //! If the manager is set to teleport you!
            if (Manager.Instance.enums.pointerState == Manager.Enums.PointerState.Teleport)
            {
                //If the target hit has a highlightscript!
                if (highlightCubes)
                    if (previousHit.collider.GetComponentInChildren<CubeHighlight>() != null)
                        previousHit.collider.GetComponentInChildren<CubeHighlight>().StopRender();
                //

                //If the target hit is has a sidescript attached to determin where we teleport!
                if (hit.collider.GetComponent<SideScript>() != null)
                {
                    if (hit.collider == asteroidHit.collider)
                        return;

                    if (master.arrowsTeleport)
                    {
                        hit.collider.GetComponent<SideScript>().sides = master.currentSide;
                        master.newRot = hit.collider.GetComponent<AsteroidRot>().ReturnCurrentTeleportPos(hit.collider.GetComponent<SideScript>().sides).rotation;
                    }
                    else
                        master.newRot = hit.collider.transform.rotation;

                    if (Manager.Instance.enums.teleportVersion != Manager.Enums.TeleVersion.arrowsSide)
                    {
                        master.newPos = hit.point;
                    }
                    else
                    {
                        master.newPos = hit.point;
                    }

                    if (!Manager.Instance.uISettings.useNewUI)
                        teleportorBuildUI.SetActive(true);

                    master.SetHit(hit);

                    if (asteroidHit.collider == null)
                        asteroidHit = prevAsteroidHit;

                    tempPrevAsteroidHit = prevAsteroidHit;
                    tempAsteroidHit = asteroidHit;

                    prevAsteroidHit = asteroidHit;
                    asteroidHit = hit;

                    ghostLineHit = asteroidHit;


                    master.currentAsteroidStandingOn = hit.collider.GetComponent<SideScript>();

                    //If we use the new State version of the UI we shoudl teleport instantly uppon clicking on a asteroid!
                    if (Manager.Instance.uISettings.useNewUI)
                        TeleportTrue();
                }
                //

                //If we hit the teleport button
                if (hit.collider.CompareTag("TeleButton"))
                {
                    DisableTeleMenu();

                    TeleportTrue();
                    master.RemoveTurretButtonsOnAsteroid();
                }
                //
            }
            //

            //! If the manager is set to build on the selected cube!
            if (Manager.Instance.enums.pointerState == Manager.Enums.PointerState.Build)
            {
                //If we hit a UI element that spawns a turret!
                if (hit.collider.GetComponent<TurretSpawn>() != null)
                {
                    master.ReseMaxLenght();
                    UpdateLineRenderer();

                    TurretSpawn newTurretSpawn = hit.collider.GetComponent<TurretSpawn>();
                    newTurretSpawn.SpawnEm();
                    Manager.Instance.CurrentBuildTarget(newTurretSpawn);
                }
                //

                //If we hit the build button
                if (Manager.Instance.uISettings.useNewUI)
                {
                    if (hit.collider.GetComponent<SideScript>() != null)
                    {
                        master.currentHit = hit.collider.GetComponent<SideScript>();

                        prevAsteroidHit = tempPrevAsteroidHit;
                        asteroidHit = tempAsteroidHit;
                        master.AsteroidToBuildOn();

                        master.IncreaseMaxLenght();
                        UpdateLineRenderer();
                    }
                }
                else
                {
                    if (hit.collider.CompareTag("BuildButton"))
                    {
                        DisableTeleMenu();

                        prevAsteroidHit = tempPrevAsteroidHit;
                        asteroidHit = tempAsteroidHit;
                        master.AsteroidToBuildOn();

                        master.IncreaseMaxLenght();
                        UpdateLineRenderer();
                    }
                }
                //

                //If we hit a cancelButton
                if (hit.collider.CompareTag("CancelButton"))
                {
                    DisableTeleMenu();

                    RaycastHit tmp;
                    tmp = prevAsteroidHit;
                    asteroidHit = tmp;
                    prevAsteroidHit = ghostLineHit;
                    ghostLineHit = tmp;
                }
                //
            }
            //

            //! If the manager is set to rotate you on the current cube you are standing on!
            if (Manager.Instance.enums.pointerState == Manager.Enums.PointerState.Rotate)
            {
                //If its a arrow that determins our rotation!
                if (hit.collider.GetComponent<ChangeSide>() != null)
                    hit.collider.GetComponent<ChangeSide>().DoFunction();
                //

                if (hit.collider.CompareTag("IndexNode"))
                {
                    int newIndex = hit.collider.GetComponent<IndexNode>().index;
                    if (newIndex != master.arrowIndex)
                        master.ChangeIndex(newIndex);
                }
            }

            //Not currently in use!
            //If we hit the confim or deny button!
            if (hit.collider.GetComponent<ConfirmDeny>() != null)
                hit.collider.GetComponent<ConfirmDeny>().DoEffect();
            //

            if (renderOwnLine)
                SetLineEnd(hit);
        }
        canTeleport = false;
        if (renderOwnLine)
            line.enabled = false;
    }

    bool tmp = false;
    public void TeleportTrue (bool ghost = false)
    {
        if (ghost)
        {
            if (tmp)
                master.previousHit = prevAsteroidHit.collider.GetComponent<SideScript>();

            master.CheckSides(ghostLineHit, ghostLineHit.collider.GetComponent<SideScript>().sides);
            ghostLineHit.collider.GetComponent<SideScript>().Reached();
        }
        else
        {
            if (tmp && !master.ghostLine)
                master.previousHit = prevAsteroidHit.collider.GetComponent<SideScript>();

            master.CheckSides(asteroidHit, asteroidHit.collider.GetComponent<SideScript>().sides);
            asteroidHit.collider.GetComponent<SideScript>().Reached();
        }
        tmp = true;
    }

    public void DisableTeleMenu ()
    {
        teleportorBuildUI.SetActive(false);
    }

    public void SetLineEnd (RaycastHit hit)
    {
        linePos[0] = hit.point;
        line.SetPositions(linePos);
    }

    public void ResetLineEnd ()
    {
        linePos[0] = lineEnd.position;
        line.SetPositions(linePos);
    }

    void UpdateLineRenderer ()
    {
        GetComponent<VRTK_StraightPointerRenderer>().maximumLength = master.GetMaxLenght();
    }

    //Changes the color of the ray/laser based on what you are pointing at!
    void ChangeLineVersion ()
    {
        switch (lineVersion)
        {
            case LineVersion.nothing:
                line.material.SetColor("_BaseColor", Color.cyan);
                break;
            case LineVersion.outOfRange:
                line.material.SetColor("_BaseColor", Color.red);
                break;
            case LineVersion.hit:
                line.material.SetColor("_BaseColor", Color.green);
                break;
        }
    }

    public void CanTeleport ()
    {
        canTeleport = true;
    }
}