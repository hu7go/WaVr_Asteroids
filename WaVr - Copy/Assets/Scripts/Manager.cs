using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SpatialTracking;
using UnityEngine.UI;
using VRTK;

public class Manager : MonoBehaviour
{
    [System.Serializable]
    public class Enums
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
        public TeleVersion teleportVersion = TeleVersion.onTop;
        [Tooltip("This enum decides which state the player is in currently to make sure you can only do one thing at a time!")]
        public PointerState pointerState;
    }
    public Enums enums;

    [System.Serializable]
    public class DayDreamSettings
    {
        public bool usingDayDream = false;
        public BoxCollider[] dayDreamBoxIncreases;
        public VRTK_InteractableObject gun;
        public GameObject gunObj;
        public VRTK_StraightPointerRenderer[] renderers;
    }
    [Space(10)]
    public DayDreamSettings daydreamSettings;

    [System.Serializable]
    public class TurretAndEnemiesSettings
    {
        public bool turretHover = false;
        public bool towerDefence;

        public GameObject tDObjective;
        public GameObject tDObjectiveSpawnPoints;
        public GameObject[] enemySpawnPoints;
        public GameObject enemySpawner;
        public GameObject enemyPrefab;
        public GameObject enemySpawnPoint;
        [HideInInspector] public int waveCounter = 0;
        public int maxNumberOfEnemies = 0;
        //!? public int totalNumberOfEnemiesAllowedToSpawn = 10;
        //!? public int totalNumberOfEnemiesSpawned = 0;
        public bool holdingGun = false;
    }
    [Space(10)]
    public TurretAndEnemiesSettings turretsAndEnemies;

    [System.Serializable]
    public class GraphicsSettings
    {
        public Material skybox;
        [Tooltip("Toggle between asteroid and cube meshes")]
        public bool cubesOn = true;
        public MeshRenderer[] cubes;
        public GameObject asteroids;
        public Material defaultMat;
        public Material selectedMat;
    }
    [Space(10)]
    public GraphicsSettings graphicsSettings;

    [System.Serializable]
    public class UISettings
    {
        public GameObject startUI;
        public GameObject endUI;
        public GameObject objective;
        public GameObject startButton;
        public GameObject confrimDenyButtons;
        public Text timerText;
        public Text overText;
        public Text pizzaCounter;
        public Text waveCount;
        public MeshRenderer[] stateButtons;
        public Slider slider;
        [Tooltip("This bool decides if the new UI is used this current scene")]
        public bool useNewUI;
        [Tooltip("The new three button UI for teleporting, building and rotating.")]
        public GameObject newUIButtons;
        public GameObject arrowsUI;
    }
    [Space(10)]
    public UISettings uISettings;

    [HideInInspector] public Vector3 teleportOffset;

    [Space(10)]
    public UIManager uiManager;
    public GameObject cameraEye;
    public bool useGhostLine = true;
    public bool positiontrackingOn = true;

    //This is used for when position tracking is off!
    public OVRManager occulus;

    [Space(20)]
    [SerializeField] private GameObject player;

    [Space(20)]
    [SerializeField] private bool startTimer = false;
    [SerializeField] private bool gameStarted = false;
    //If its true the game will spawn enemies from the start!
    [SerializeField] private bool startGameWithEnemies = false;
    [SerializeField] private float myTimer = 0f;
    [SerializeField] private int playerHealth = 10;

    [Space(20)]
    public int nmbrOfPizzas = 0;

    [Space(20)]
    public IndexNode[] indexNodes;
    [HideInInspector]
    public GameObject referenceTD;
    private int killedEnemies;
    //!? private int currentNumberOFenemies = 0;
    private int counter;

    int minutes;
    int seconds;

    private GameObject enemyParent;


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

        graphicsSettings.skybox.SetFloat("_Exposure", 1);
        PositionTracking();
    }

    private void Start()
    {
        enemyParent = new GameObject("enemyParent");

        //add a enemyspawnlocation look at objective
        SetPointerState(Enums.PointerState.Teleport);

        switch (graphicsSettings.cubesOn)
        {
            case true:
                foreach (MeshRenderer mesh in graphicsSettings.cubes)
                    mesh.enabled = true;
                graphicsSettings.asteroids.SetActive(false);
                break;
            case false:
                foreach (MeshRenderer mesh in graphicsSettings.cubes)
                    mesh.enabled = false;
                graphicsSettings.asteroids.SetActive(true);
                break;
            default:
                break;
        }
    }

    public void UsingDayDream()
    {
        daydreamSettings.usingDayDream = true;

        graphicsSettings.skybox.SetFloat("_Exposure", 2.5f);

        for (int i = 0; i < daydreamSettings.dayDreamBoxIncreases.Length; i++)
            daydreamSettings.dayDreamBoxIncreases[i].size *= 4;

        daydreamSettings.gun.grabOverrideButton = VRTK_ControllerEvents.ButtonAlias.TriggerPress;
    }

    public void GrabbedGun()
    {
        daydreamSettings.gun.grabOverrideButton = VRTK_ControllerEvents.ButtonAlias.GripPress;
    }

    public void StartSpawningEnemies()
    {
        if (turretsAndEnemies.towerDefence)
        {
            turretsAndEnemies.waveCounter++;
            uISettings.waveCount.text = ("Wave: " + turretsAndEnemies.waveCounter);
            turretsAndEnemies.maxNumberOfEnemies += 5;
            if(turretsAndEnemies.maxNumberOfEnemies <= 5)
                referenceTD = Instantiate(turretsAndEnemies.tDObjective, turretsAndEnemies.tDObjectiveSpawnPoints.transform); 
            StartCoroutine(EnemySpawner());
        }
    }
    
    private IEnumerator EnemySpawner()
    {
        yield return new WaitForSeconds(15);
        int rnd = Random.Range(0, 4);
        GameObject localEnemySpawner = Instantiate(turretsAndEnemies.enemySpawner, turretsAndEnemies.enemySpawnPoints[0/*rnd*/].transform.position,transform.rotation); 
        localEnemySpawner.transform.rotation = Quaternion.LookRotation(referenceTD.transform.position, Vector3.up);
        counter = 0;
        turretsAndEnemies.enemySpawnPoint = localEnemySpawner;
        yield return StartCoroutine(SpawnEnemyObjective(localEnemySpawner));
    }

    private IEnumerator SpawnEnemyObjective(GameObject spawner)
    {
        while (counter < turretsAndEnemies.maxNumberOfEnemies)
        {
            counter++;
            yield return new WaitForSeconds(2);
            InstantiateEnemy();
        }
            //if objective now completed new function with end result of time + kills? Calls GAMEOVER from ObjectiveHP script when HP = 0;

        Destroy(spawner,2);
    }

    public void RoutineOpener()
    {
        StartCoroutine(EnemySpawner());
    }

    public void InstantiateEnemy()
    {
        Instantiate(turretsAndEnemies.enemyPrefab, turretsAndEnemies.enemySpawnPoint.transform.position, turretsAndEnemies.enemySpawnPoint.transform.rotation, enemyParent.transform);
    }

    public void GameOver()
    {
        //if objective dies
    }

    public void RemoveEnemy()
    {
        killedEnemies++;
        if (killedEnemies == turretsAndEnemies.maxNumberOfEnemies)
        {
            StartSpawningEnemies();
            killedEnemies = 0;
        }
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

            uISettings.timerText.text = minutes.ToString() + ": " + seconds.ToString("00");
        }
    }

    //The function that happens when you click the start game button!
    public void StartGame()
    {
        if (uISettings.useNewUI)
            uISettings.newUIButtons.SetActive(true);

        gameStarted = true;
        uISettings.startUI.SetActive(false);
        startTimer = true;
        Destroy(uISettings.startButton);
    }
    //

    public bool StartedGame()
    {
        return gameStarted;
    }

    public void ObjectiveReached()
    {
        startTimer = false;
        uISettings.endUI.SetActive(true);
        uISettings.overText.text = ("Well done! You completed it in: " + minutes.ToString() + ": " + seconds.ToString("00") + " seconds");
    }

    public void ObjectiveFailed()
    {
        //Game over!
    }

    private void FirstPizzaFound()
    {
        uISettings.pizzaCounter.gameObject.SetActive(true);
    }

    public void UpdatePizzaCounter()
    {
        if (nmbrOfPizzas < 5)
            nmbrOfPizzas++;

        if (nmbrOfPizzas == 1)
            FirstPizzaFound();

        if (nmbrOfPizzas == 5)
        {
            uISettings.objective.SetActive(true);
            uISettings.pizzaCounter.text = ("The drones are hungry for your pizza!");
            Invoke("Spawner", 2f);
        }

        uISettings.pizzaCounter.text = ("Pizza slices: " + nmbrOfPizzas);
    }

    private void Spawner()
    {
        uISettings.objective.SetActive(false);
        StartSpawningEnemies();
        uISettings.pizzaCounter.gameObject.SetActive(false);
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
            daydreamSettings.gunObj.SetActive(true);
            foreach (VRTK_StraightPointerRenderer pointer in daydreamSettings.renderers)
                pointer.enabled = false;
        }
        daydreamSettings.gunObj.GetComponent<GunOnOff>().RefreshGun();
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
            uISettings.confrimDenyButtons.SetActive(false);
        else
            uISettings.confrimDenyButtons.SetActive(true);

        toggle = !toggle;
    }

    public void UpdateAsteroidLine()
    {
        if (useGhostLine)
            GetComponent<GhostLine>().UpdateLine();
    }

    public void HoldingGun()
    {
        turretsAndEnemies.holdingGun = true;
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

    public void SetPointerState(Enums.PointerState newPointerState)
    {
        enums.pointerState = newPointerState;

        switch (newPointerState)
        {
            case Enums.PointerState.Teleport:
                DeactivateRotationArrows();

                uISettings.stateButtons[0].material = graphicsSettings.selectedMat;
                uISettings.stateButtons[1].material = graphicsSettings.defaultMat;
                uISettings.stateButtons[2].material = graphicsSettings.defaultMat;

                foreach (IndexNode index in indexNodes)
                    index.Off();
                break;
            case Enums.PointerState.Build:
                DeactivateRotationArrows();

                uISettings.stateButtons[0].material = graphicsSettings.defaultMat;
                uISettings.stateButtons[1].material = graphicsSettings.selectedMat;
                uISettings.stateButtons[2].material = graphicsSettings.defaultMat;

                foreach (IndexNode index in indexNodes)
                    index.Off();
                break;
            case Enums.PointerState.Rotate:
                ActivateRotationArrows();

                uISettings.stateButtons[0].material = graphicsSettings.defaultMat;
                uISettings.stateButtons[1].material = graphicsSettings.defaultMat;
                uISettings.stateButtons[2].material = graphicsSettings.selectedMat;

                foreach (IndexNode index in indexNodes)
                    index.On();
                break;
            default:
                break;
        }
    }

    public void ActivateRotationArrows ()
    {
        uISettings.arrowsUI.SetActive(true);
    }

    public void DeactivateRotationArrows ()
    {
        uISettings.arrowsUI.SetActive(false);
    }
}