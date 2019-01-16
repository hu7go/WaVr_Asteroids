using System.Collections;
using UnityEngine;
using UnityEngine.SpatialTracking;
using UnityEngine.UI;
using VRTK;

public class Manager : MonoBehaviour
{
    public enum TeleVersion
    {
        anywhere,
        onTop,
        arrows,
        onTopSide,
        arrowsSide
    }
    public enum PointerState
    {
        Teleport,
        Build,
        Rotate
    }

    [HideInInspector] public Vector3 teleportOffset;

    public bool usingDayDream = false;
    public Material skybox;
    [Tooltip("Toggle between asteroid and cube meshes")]
    public bool cubesOn = true;
    public MeshRenderer[] cubes;
    public GameObject asteroids;
    public GameObject cameraEye;
    public bool useGhostLine = true;
    public bool positiontrackingOn = true;

    public OVRManager occulus;

    [Space(20)]
    public TeleVersion teleportVersion = TeleVersion.onTop;

    [Space(20)]
    public bool turretHover = false;
    [Space(20)]
    [SerializeField] private GameObject player;

    [Space(20)]
    [SerializeField] private bool startTimer = false;
    [SerializeField] private bool gameStarted = false;
    [SerializeField] private bool towerDefence;
    [SerializeField] private bool startGameWithEnemies = false;
    [SerializeField] private float myTimer = 0f;
    [SerializeField] private int playerHealth = 10;
    [SerializeField] private GameObject startUI;
    [SerializeField] private GameObject endUI;
    [SerializeField] private GameObject objective;
    [SerializeField] private GameObject startButton;
    [SerializeField] private GameObject confrimDenyButtons;
    [SerializeField] public GameObject tDObjective;
    [SerializeField] private GameObject[] tDObjectiveSpawnPoints;
    [SerializeField] private GameObject enemySpawner;
    [SerializeField] private GameObject enemyPrefab;
    private GameObject enemySpawnPoint;
    [SerializeField] private Text timerText;
    [SerializeField] private Text overText;
    [SerializeField] private Text pizzaCounter;

    [Space(20)]
    [Tooltip("This bool decides if the new UI is used this current scene")]
    public bool useNewUI;
    [Tooltip("The new three button UI for teleporting, building and rotating.")]
    [SerializeField] private GameObject newUIButtons;
    [Tooltip("This enum decides which state the player is in currently to make sure you can only do one thing at a time!")]
    public PointerState pointerState;
    [SerializeField] private GameObject arrowsUI;
    [SerializeField] private MeshRenderer[] stateButtons;
    [SerializeField] private Material defaultMat;
    [SerializeField] private Material selectedMat;
    [Space(20)]
    public int nmbrOfPizzas = 0;

    [Space(20)]
    [Header("Enemies:")]
    [SerializeField] private GameObject enemy;
    [SerializeField] private int maxNumberOfEnemies = 5;
    [SerializeField] private int totalNumberOfEnemiesAllowedToSpawn = 10;
    [SerializeField] private int totalNumberOfEnemiesSpawned = 0;
    public bool holdingGun = false;

    [Space(20)]
    public BoxCollider[] dayDreamBoxIncreases;
    public VRTK_InteractableObject gun;
    public GameObject gunObj;
    public VRTK_StraightPointerRenderer[] renderers;

    [Space(20)]
    public IndexNode[] indexNodes;
    [HideInInspector]
    public GameObject referenceTD;
    private int killedEnemies;
    private int currentNumberOFenemies = 0;
    private int counter;

    int minutes;
    int seconds;

    private static bool created = false;
    public static Manager Instance { get; private set; }

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else if (Instance != this)
            Destroy(gameObject);

        if (!created)
            created = true;

        skybox.SetFloat("_Exposure", 1);
        PositionTracking();
    }

    private void Start()
    {
        //add a enemyspawnlocation look at objective
        SetPointerState(PointerState.Teleport);

        if (startGameWithEnemies)
            StartSpawningEnemies();

        switch (cubesOn)
        {
            case true:
                foreach (MeshRenderer mesh in cubes)
                    mesh.enabled = true;
                asteroids.SetActive(false);
                break;
            case false:
                foreach (MeshRenderer mesh in cubes)
                    mesh.enabled = false;
                asteroids.SetActive(true);
                break;
            default:
                break;
        }
    }

    public void UsingDayDream()
    {
        usingDayDream = true;

        skybox.SetFloat("_Exposure", 2.5f);

        for (int i = 0; i < dayDreamBoxIncreases.Length; i++)
            dayDreamBoxIncreases[i].size *= 4;

        gun.grabOverrideButton = VRTK_ControllerEvents.ButtonAlias.TriggerPress;
    }

    public void GrabbedGun()
    {
        gun.grabOverrideButton = VRTK_ControllerEvents.ButtonAlias.GripPress;
    }

    public void StartSpawningEnemies()
    {
        if(!towerDefence)
        for (int i = 0; i < maxNumberOfEnemies; i++)
        {
            if (totalNumberOfEnemiesSpawned >= totalNumberOfEnemiesAllowedToSpawn)
                return;

            Spawn();
        }
        if (towerDefence)
        {
            int rnd = Random.Range(0, 4);
            referenceTD = Instantiate(tDObjective,tDObjectiveSpawnPoints[1].transform); //change to rnd
            StartCoroutine(EnemySpawner());
        }
    }
    private IEnumerator EnemySpawner()
    {
        yield return new WaitForSeconds(15);
        Vector3 random = new Vector3(Random.Range(0, 50), Random.Range(0, 50), Random.Range(0, 50));
        GameObject localEnemySpawner = Instantiate(enemySpawner,transform.position + new Vector3(0,10,0),transform.rotation); //change back to random when done with testing Instantiate(enemySpawner,random,transform.rotation,transform);
        counter = 0;
        localEnemySpawner.transform.parent = null;
        enemySpawnPoint = localEnemySpawner.transform.GetChild(0).gameObject;
        yield return StartCoroutine(SpawnEnemyObjective());
    }

    private IEnumerator SpawnEnemyObjective()
    {
        while (counter < maxNumberOfEnemies)
        {
            counter++;
            yield return new WaitForSeconds(2);
            //print(counter);
            //print("is this thing on");
            InstantiateEnemy();
            //complete wave go back to "StartSpawningEnemies" for wave 2;
            //else new function with end result of time + kills? Calls GAMEOVER from ObjectiveHP script when HP = 0;
        }
        //yield return new WaitForSeconds(15);
        //RoutineOpener();
    }

    public void RoutineOpener()
    {
        StartCoroutine(EnemySpawner());
    }
    public void InstantiateEnemy()
    {
        Instantiate(enemyPrefab, enemySpawnPoint.transform);
        //print(enemySpawnPoint);
    }
    public void GameOver()
    {
        //if objective dies
    }

    public void RemoveEnemie()
    {
        currentNumberOFenemies--;
        killedEnemies++;
        if (killedEnemies == totalNumberOfEnemiesAllowedToSpawn)
            ObjectiveReached();
        StartCoroutine(SpawnEnemy());
    }

    private IEnumerator SpawnEnemy()
    {
        yield return new WaitForSeconds(4);
        if (totalNumberOfEnemiesSpawned < totalNumberOfEnemiesAllowedToSpawn)
            Spawn();
    }

    private void Spawn()
    {
        Instantiate(enemy, new Vector3(Random.Range(-50, 50), Random.Range(-50, 50), Random.Range(-50, 50)), transform.rotation);
        currentNumberOFenemies++;
        totalNumberOfEnemiesSpawned++;
    }

    public void StartTimer()
    {
        startTimer = !startTimer;
    }

    public bool GetStartBool()
    {
        return startTimer;
    }

    private void Update()
    {
        if (transform.position != new Vector3(0, 0, 0))
            transform.position = new Vector3(0, 0, 0);
        if (transform.rotation != new Quaternion(0, 0, 0, 0))
            transform.rotation = new Quaternion(0, 0, 0, 0);
        if (startTimer)
        {
            myTimer += Time.deltaTime;

            minutes = (int)myTimer / 60;
            seconds = (int)myTimer % 60;

            timerText.text = minutes.ToString() + ": " + seconds.ToString("00");
        }
    }

    //The function that happens when you click the start game button!
    public void StartGame()
    {
        if (useNewUI)
            newUIButtons.SetActive(true);

        gameStarted = true;
        startUI.SetActive(false);
        startTimer = true;
        Destroy(startButton);
        if (towerDefence)
        {
            StartSpawningEnemies();
        }
    }
    //

    public bool StartedGame()
    {
        return gameStarted;
    }

    public void ObjectiveReached()
    {
        startTimer = false;
        endUI.SetActive(true);
        overText.text = ("Well done! You completed it in: " + minutes.ToString() + ": " + seconds.ToString("00") + " seconds");
    }

    private void FirstPizzaFound()
    {
        pizzaCounter.gameObject.SetActive(true);
    }

    public void UpdatePizzaCounter()
    {
        if (nmbrOfPizzas < 5)
            nmbrOfPizzas++;

        if (nmbrOfPizzas == 1)
            FirstPizzaFound();

        if (nmbrOfPizzas == 5)
        {
            objective.SetActive(true);
            pizzaCounter.text = ("The drones are hungry for your pizza!");
            Invoke("Spawner", 2f);
        }

        pizzaCounter.text = ("Pizza slices: " + nmbrOfPizzas);
    }

    private void Spawner()
    {
        objective.SetActive(false);
        StartSpawningEnemies();
        pizzaCounter.gameObject.SetActive(false);
    }

    public void PlayerHit(int damage)
    {
        playerHealth -= damage;

        if (playerHealth <= 0)
        {
            //Lose State!
        }
    }

    public GameObject ReturnPlayer()
    {
        return player;
    }

    public void RefreshGun()
    {
        if (nmbrOfPizzas == 5)
        {
            gunObj.SetActive(true);
            foreach (VRTK_StraightPointerRenderer pointer in renderers)
                pointer.enabled = false;
        }
        gunObj.GetComponent<GunOnOff>().RefreshGun();
    }

    TurretSpawn turretSpawn;

    public void CurrentBuildTarget(TurretSpawn current)
    {
        turretSpawn = current;
    }

    public TurretSpawn GetCurrentBuildTarget()
    {
        return turretSpawn;
    }

    private bool toggle = true;
    public void ToggleConfirmDenyButtons()
    {
        if (!toggle)
            confrimDenyButtons.SetActive(false);
        else
            confrimDenyButtons.SetActive(true);

        toggle = !toggle;
    }

    public void UpdateAsteroidLine()
    {
        if (useGhostLine)
            GetComponent<GhostLine>().UpdateLine();
    }

    public void HoldingGun()
    {
        holdingGun = true;
    }

    public void PositionTracking()
    {
        if (positiontrackingOn && cameraEye != null)
            cameraEye.GetComponent<TrackedPoseDriver>().trackingType = TrackedPoseDriver.TrackingType.RotationAndPosition;
        if (!positiontrackingOn && cameraEye != null)
        {
            cameraEye.GetComponent<TrackedPoseDriver>().trackingType = TrackedPoseDriver.TrackingType.RotationOnly;
            cameraEye.transform.position += new Vector3(0, 1.5f, 0);
        }

        if (!positiontrackingOn)
        {
            occulus.transform.position += new Vector3(0, 1.5f, 0);
            occulus.usePositionTracking = false;
            occulus._trackingOriginType = OVRManager.TrackingOrigin.EyeLevel;
        }
        else
        {
            occulus.usePositionTracking = true;
            occulus._trackingOriginType = OVRManager.TrackingOrigin.FloorLevel;
        }
    }

    public void SetPointerState(PointerState newPointerState)
    {
        pointerState = newPointerState;

        switch (newPointerState)
        {
            case PointerState.Teleport:
                DeactivateRotationArrows();

                stateButtons[0].material = selectedMat;
                stateButtons[1].material = defaultMat;
                stateButtons[2].material = defaultMat;

                foreach (IndexNode index in indexNodes)
                    index.Off();
                break;
            case PointerState.Build:
                DeactivateRotationArrows();

                stateButtons[0].material = defaultMat;
                stateButtons[1].material = selectedMat;
                stateButtons[2].material = defaultMat;

                foreach (IndexNode index in indexNodes)
                    index.Off();
                break;
            case PointerState.Rotate:
                ActivateRotationArrows();

                stateButtons[0].material = defaultMat;
                stateButtons[1].material = defaultMat;
                stateButtons[2].material = selectedMat;

                foreach (IndexNode index in indexNodes)
                    index.On();
                break;
            default:
                break;
        }
    }

    public void ActivateRotationArrows ()
    {
        arrowsUI.SetActive(true);
    }

    public void DeactivateRotationArrows ()
    {
        arrowsUI.SetActive(false);
    }
}