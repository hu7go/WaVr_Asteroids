using System.Collections.Generic;
using UnityEngine;
using VRTK;

public class TeleportRotation : MonoBehaviour
{
    public TeleportMaster master;
    public LayerMask turretLayerMask;
    public LayerMask ignoredLayer;

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

    [HideInInspector] public bool renderLine = false;

    RaycastHit tmpPrev = new RaycastHit();

    public LayerMask startLayerMask;
    RaycastHit startHit;
    RaycastHit lineNotHit;

    private LineBender lineRender;

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

        lineRender = GetComponent<LineBender>();

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

        //If we hit the start button!
        if (canTeleport)
        {
            if (Physics.Raycast(currentHand.transform.position, currentHand.transform.TransformDirection(Vector3.forward), out startHit, master.GetMaxLenght(), startLayerMask))
                Manager.Instance.StartGame();
        }
        //

        //This handles all the teleporting and clicking of buttons!
        if (canTeleport && Manager.Instance.StartedGame())
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
            return;
        }
        if (parents[1] == enabled && parents[1].activeInHierarchy)
        {
            currentHand = hands[1];
            switchedHands = true;
            return;
        }
        //Using DayDream!
        if (parents[2] == enabled && parents[2].activeInHierarchy)
        {
            if (hands[2] != null)
            {
                Manager.Instance.UsingDayDream();

                currentHand = hands[2];
                switchedHands = true;
                return;
            }
        }
        else
            return;
    }

    RaycastHit tmpRaycastHit;
    private void TeleportStuff ()
    {
        Manager.Instance.freeze = true;

        renderLine = true;
        lineRender.render.enabled = true;

        ray = new Ray(currentHand.transform.position, currentHand.transform.TransformDirection(Vector3.forward));

        Physics.Raycast(currentHand.transform.position, currentHand.transform.TransformDirection(Vector3.forward), out hit, master.GetMaxLenght());

        Physics.Raycast(currentHand.transform.position, currentHand.transform.TransformDirection(Vector3.forward), out lineNotHit, master.GetMaxLenght() * 20);


        cr.CustomRaycast(ray, out hit, master.GetMaxLenght());

        if (hit.collider != null)
        {
            Ray tmpRay = new Ray(hit.point + (hit.collider.transform.position - hit.point) * .1f, currentHand.transform.TransformDirection(Vector3.forward) * (master.GetMaxLenght() - (hit.point - currentHand.transform.position).magnitude));

            Debug.DrawRay(hit.point + (hit.collider.transform.position - hit.point) * .1f, currentHand.transform.TransformDirection(Vector3.forward) * (master.GetMaxLenght() - (hit.point - currentHand.transform.position).magnitude), Color.cyan);

            if (Physics.Raycast(tmpRay, out tmpRaycastHit, master.GetMaxLenght(), ignoredLayer))
            {
                hit = tmpRaycastHit;
            }
        }

        //! Line stuffs
        if (hit.collider != null)
        {
            //Debug.Log("Bending!");

            lineRender.SetEnd(hit.point);

            if (hit.collider.GetComponent<SideScript>() != null)
            {
                lineRender.SetLineEnd(hit.collider.transform.position, hit.point);
            }
        }
        else
        {
            //Debug.Log("Straight!");
            lineRender.StraightRenderer();
        }

        if (hit.collider == null && lineNotHit.collider == null)
            lineVersion = LineVersion.nothing;
        if (hit.collider == null && lineNotHit.collider != null)
            lineVersion = LineVersion.outOfRange;
        if (hit.collider != null)
            lineVersion = LineVersion.hit;

        lineRender.ChangeLineVersion();
        //

        if (Manager.Instance.tAe.turretHover)
        {
            if (Physics.Raycast(currentHand.transform.position, currentHand.transform.TransformDirection(Vector3.forward), out buildButtonTarget, master.GetMaxLenght(), turretLayerMask))
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
        Manager.Instance.freeze = false;

        if (hit.collider)
        {
            //If we press the restart button after we have died!
            if (hit.collider.CompareTag("RestartButton"))
            {
                master.RemoveTurretButtonsOnAsteroid();
                Manager.Instance.Restarter();
            }
            //

            //If we hit the teleport state button!
            if (hit.collider.CompareTag("TeleportState"))
            {
                canTeleport = false;
                lineRender.render.enabled = false;
                master.RemoveTurretButtonsOnAsteroid();
                Manager.Instance.SetPointerState(Manager.Enums.PointerState.Teleport);
            }
            //

            //If we hit the build state button!
            if (hit.collider.CompareTag("BuildState"))
            {
                canTeleport = false;
                lineRender.render.enabled = false;
                Manager.Instance.SetPointerState(Manager.Enums.PointerState.Build);
            }
            //

            //If we hit the rotate state button!
            if (hit.collider.CompareTag("RotateState"))
            {
                canTeleport = false;
                lineRender.render.enabled = false;
                master.RemoveTurretButtonsOnAsteroid();
                Manager.Instance.SetPointerState(Manager.Enums.PointerState.Rotate);
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
                if (hit.collider.GetComponent<SideScript>() != null && hit.collider != asteroidHit.collider)
                {
                    master.RemoveTurretButtonsOnAsteroid();

                    if (hit.collider == asteroidHit.collider)
                    {
                        //!? This might not always work, so far there has been a few instances of it not working strangly enough!
                        Debug.Log("Maybe it happend again... if you didnt mean to do it!");
                        UpdateLineRenderer();
                        return;
                    }

                    if (master.arrowsTeleport)
                    {
                        hit.collider.GetComponent<SideScript>().sides = master.currentSide;
                        master.newRot = hit.collider.GetComponent<AsteroidRot>().ReturnCurrentTeleportPos(hit.collider.GetComponent<SideScript>().sides).rotation;
                    }
                    else
                        master.newRot = hit.collider.transform.rotation;

                    if (Manager.Instance.enums.teleportVersion != Manager.Enums.TeleVersion.arrowsSide)
                        master.newPos = hit.point;
                    else
                        master.newPos = hit.point;

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

                //If we hit the teleport button. Not currently in use!
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
                master.RemoveTurretButtonsOnAsteroid();

                //If we hit a UI element that spawns a turret, which will spawn a turret at the selected location!
                if (hit.collider.GetComponent<TurretSpawn>() != null)
                {
                    master.ReseMaxLenght();
                    UpdateLineRenderer();
                   
                    if (Manager.Instance.spawnedFirstTurret == false)
                        Manager.Instance.StartEnemyWaves();

                    TurretSpawn newTurretSpawn = hit.collider.GetComponent<TurretSpawn>();
                    newTurretSpawn.SpawnEm();
                    Manager.Instance.CurrentBuildTarget(newTurretSpawn);
                    Manager.Instance.turretReload.Reload();

                    canTeleport = false;
                    lineRender.render.enabled = false;
                }
                //

                if (Manager.Instance.uISettings.useNewUI)
                {
                    //To build a healer!
                    Debug.Log(master.currentAsteroidStandingOn.gameObject);
                    if (hit.collider.GetComponent<SideScript>().gameObject == master.currentAsteroidStandingOn.gameObject)
                    {
                        if (master.currentAsteroidStandingOn.GetComponentInChildren<AsteroidHealth>().asteroid.alive == false && master.currentAsteroidStandingOn.GetComponentInChildren<AsteroidHealth>().asteroid.beingHealed == false)
                        {
                            GameObject healer = Instantiate(Manager.Instance.tAe.healer, master.currentAsteroidStandingOn.transform);
                            healer.GetComponent<Healer>().SpawnAHealer(master.currentAsteroidStandingOn.gameObject);
                            UIMaster uImaster = Manager.Instance.gameObject.GetComponent<UIMaster>();
                            StartCoroutine(uImaster.TextOnDelayOff(uImaster.NowHealingTextStart, uImaster.NowHealingTextStop));
                        }
                        canTeleport = false;
                        renderLine = false;
                        lineRender.render.enabled = false;
                        return;
                    }
                    //

                    if (Manager.Instance.turretReload.numberOfTurretsLeft > 0)
                    {
                        //If we hit a asteroid to build on it!
                        if (hit.collider.GetComponent<SideScript>() != null)
                        {
                            if (hit.collider.GetComponentInChildren<AsteroidHealth>().asteroid.alive == false)
                            {
                                UIMaster uImaster = Manager.Instance.gameObject.GetComponent<UIMaster>();
                                StartCoroutine(uImaster.TextOnDelayOff(uImaster.NobuildTextStart, uImaster.NobuildTextStop));
                                canTeleport = false;
                                renderLine = false;
                                lineRender.render.enabled = false;
                                return;
                            }

                            master.currentHit = hit.collider.GetComponent<SideScript>();

                            RaycastHit tmpRaycastHit = new RaycastHit();

                            prevAsteroidHit = tempPrevAsteroidHit;
                            tmpRaycastHit = tempAsteroidHit;
                            //asteroidHit = tempAsteroidHit;
                            master.AsteroidToBuildOn();

                            master.IncreaseMaxLenght();
                            UpdateLineRenderer();
                        }
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

                //If its a index node that changes our position relative to the current cube/asteroid!
                if (hit.collider.CompareTag("IndexNode"))
                {
                    int newIndex = hit.collider.GetComponent<IndexNode>().index;
                    if (newIndex != master.arrowIndex)
                        master.ChangeIndex(newIndex);
                }
                //
            }

            //Not currently in use!
            //If we hit the confim or deny button!
            if (hit.collider.GetComponent<ConfirmDeny>() != null)
                hit.collider.GetComponent<ConfirmDeny>().DoEffect();
            //
        }
        canTeleport = false;
        renderLine = false;

        lineRender.render.enabled = false;
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

    public void DisableTeleMenu () => teleportorBuildUI.SetActive(false);

    void UpdateLineRenderer () => GetComponent<VRTK_StraightPointerRenderer>().maximumLength = master.GetMaxLenght();

    //Changes the color of the ray/laser based on what you are pointing at!

    public void CanTeleport () => canTeleport = true;
}