﻿using System.Collections;
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

        public GameObject tDObjectiveSpawnPoints;
        public GameObject[] enemySpawnPoints;
        public GameObject enemySpawner;
        public GameObject enemyPrefab;
        [HideInInspector] public GameObject enemySpawnPoint;
        public int maxNumberOfEnemies = 0;
        public bool heartrotator = false;
        public int waveCounter = 0;
        public Transform currentActiveSpawner;

        public float asteroidHealth = 200;
    }
    [Space(10)]
    public TurretAndEnemiesSettings turretsAndEnemies;

    [System.Serializable]
    public class GraphicsSettings
    {
        public enum WorldVersion
        {
            //Normal stary night skybox
            one,
            //Big planet close up skybox
            two,
            //Either skybox but a ship somewhere in the midst of the asteroids that you have to get around in order to getto certain asteroids!
            three
        }

        [Tooltip("Toggle between asteroid and cube meshes")]
        public bool cubesOn = true;
        public MeshRenderer[] cubes;
        public GameObject asteroids;
        public Material defaultMat;
        public Material selectedMat;

        [Space(20)]
        public WorldVersion worldVersion = WorldVersion.one;
        public Material skyboxOne;
        public Material skyboxTwo;
        public GameObject spaceShip;
    }
    [Space(10)]
    public GraphicsSettings graphicsSettings;

    [System.Serializable]
    public class UISettings
    {
        public GameObject startUI;
        public GameObject endUI;
        public GameObject tdEndUI;
        public GameObject objective;
        public GameObject startButton;
        public GameObject confrimDenyButtons;
        public Text tdGameOverText;
        public Text timerText;
        public Text countDownText;
        public Text overText;
        public Text waveCount;
        public MeshRenderer[] stateButtons;
        public Slider healthSlider;
        [Tooltip("This bool decides if the new UI is used this current scene")]
        public bool useNewUI;
        [Tooltip("The new three button UI for teleporting, building and rotating.")]
        public GameObject newUIButtons;
        public GameObject arrowsUI;
    }
    [Space(10)]
    public UISettings uISettings;

    [Space(10)]
    [SerializeField] private float asteroidSize = 1f;

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
    public bool freeze = false;
    [SerializeField] private bool startTimer = false;
    [SerializeField] private bool gameStarted = false;
    //If its true the game will spawn enemies from the start!
    [SerializeField] private bool startGameWithEnemies = false;
    [SerializeField] private float myTimer = 0f;
    [SerializeField] private float countdownTimer = 20;
    [Space(20)]
    public IndexNode[] indexNodes;
    [HideInInspector]
    public GameObject objective;
    private int killedEnemies;
    private int counter;
    private int lifeLeft = 3;
    [HideInInspector]
    public float objectiveHealth = 100;
    private List<GameObject> enemiesSpawned;

    public float masterCurrentHealth;
    public float masterMaxHealth;

    int minutes ,minutes2;
    int seconds,seconds2;

    [HideInInspector] public GameObject enemyParent;

    [Space(20)]
    private List<GameObject> turrets;

    [HideInInspector] public bool spawnedFirstTurret = false;

    private int numberOfEnemies = 0;

    public List<AsteroidHealth> asteroidList;

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

        SetWorldVersion();

        graphicsSettings.skyboxOne.SetFloat("_Exposure", 1);
        PositionTracking();
    }

    public void SetWorldVersion ()
    {
        switch (graphicsSettings.worldVersion)
        {
            case GraphicsSettings.WorldVersion.one:
                RenderSettings.skybox = graphicsSettings.skyboxOne;
                graphicsSettings.spaceShip.SetActive(false);
                break;
            case GraphicsSettings.WorldVersion.two:
                RenderSettings.skybox = graphicsSettings.skyboxTwo;
                graphicsSettings.spaceShip.SetActive(false);
                break;
            case GraphicsSettings.WorldVersion.three:
                //The skybox version is not decided!
                RenderSettings.skybox = graphicsSettings.skyboxOne;
                graphicsSettings.spaceShip.SetActive(true);
                break;
        }
    }

    private void Start()
    {
        turrets = new List<GameObject>();
        enemiesSpawned = new List<GameObject>();
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

        for (int i = 0; i < asteroidList.Count; i++)
        {
            masterCurrentHealth += asteroidList[i].asteroid.health;
        }
        uISettings.healthSlider.maxValue = masterCurrentHealth;
        uISettings.healthSlider.value = masterCurrentHealth;

        masterMaxHealth = masterCurrentHealth;
    }

    public void UpdateHealth (float damage)
    {
        masterCurrentHealth -= damage;
        uISettings.healthSlider.value = masterCurrentHealth;

        if (masterCurrentHealth <= 0)
            GameOver();
    }

    public void UsingDayDream()
    {
        daydreamSettings.usingDayDream = true;

        graphicsSettings.skyboxOne.SetFloat("_Exposure", 2.5f);

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
            turretsAndEnemies.maxNumberOfEnemies = 20;

            EnemySpawner();
        }
    }
    public IEnumerator SpawnThemNewEnemies()
    {
        yield return new WaitForSeconds(15);
        StartSpawningEnemies();
    }

    private void EnemySpawner ()
    {
        if (turretsAndEnemies.waveCounter > 4)
            return;

        int rnd = Random.Range(0, 4);
        GameObject localEnemySpawner = Instantiate(turretsAndEnemies.enemySpawner, turretsAndEnemies.enemySpawnPoints[turretsAndEnemies.waveCounter - 1].transform.position, transform.rotation);
        //Starts the spawning process for the enemies, spawns 'Y' amount of enemies after 'X' amount of time!
        //                                                             X   '...            Y               ...'
        localEnemySpawner.GetComponent<EnemySpawnPoint>().StartSpawner(20, turretsAndEnemies.maxNumberOfEnemies, asteroidList, 95);
        //

        turretsAndEnemies.currentActiveSpawner = localEnemySpawner.transform;

        counter = 0;
        turretsAndEnemies.enemySpawnPoint = localEnemySpawner;

        //This start the next wave after x amount of time!
        //Invoke("StartSpawningEnemies", 30);
    }

    public void RoutineOpener()
    {
        EnemySpawner();
    }

    public void InstantiateEnemy(GameObject newEnemy)
    {
        enemiesSpawned.Add(newEnemy);

        AddEnemy();
    }

    public void GameOver()
    {
        //if objective dies

        StopCoroutine("EnemySpawner");
        StopCoroutine("SpawnEnemyObjective");
        startTimer = false;
        myTimer = 0;
        //This needs to be changed at some point!
        objectiveHealth = 100;
        uISettings.healthSlider.value = objectiveHealth;
        //
        for (var i = enemiesSpawned.Count - 1; i > -1; i--)
        {
            if (enemiesSpawned[i] == null)
            {
                enemiesSpawned.RemoveAt(i);
            }
        }
        foreach (GameObject obj in enemiesSpawned)
        {
            Destroy(obj);
        }
        if(lifeLeft > 0)
        {
            uISettings.tdEndUI.SetActive(true);
            uISettings.tdGameOverText.text = "You died, you have "+lifeLeft+" lives left";
        }
        if(lifeLeft == 0)
            uISettings.tdGameOverText.text = "You died. Thanks for playing!";

        ClearTurrets();
    }
    public void Restarter()
    {
        startTimer = true;
        uISettings.tdEndUI.SetActive(false);
        killedEnemies = 0;
        turretsAndEnemies.waveCounter = 0;
        RoutineOpener();
    }

    public void AddEnemy ()
    {
        numberOfEnemies++;
    }

    public void RemoveEnemy()
    {
        numberOfEnemies--;

        Invoke("ClearNullRefs", .2f);
    }

    public void ClearNullRefs ()
    {
        for (var i = enemiesSpawned.Count - 1; i > -1; i--)
            if (enemiesSpawned[i] == null)
                enemiesSpawned.RemoveAt(i);

        if (turretsAndEnemies.waveCounter > 4 && (enemiesSpawned.Count == 0))
        {
            ObjectiveReached();
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

    //! Update Function!!!!!
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
    //! !!!!!

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

    public void StartEnemyWaves ()
    {
        if (spawnedFirstTurret == false)
        {
            StartSpawningEnemies();
            spawnedFirstTurret = true;
        }
    }

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

    public GameObject ReturnPlayer()
    {
        return player;
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

    public float RetAsteroidSize ()
    {
        return asteroidSize;
    }

    public void BuiltTurret (GameObject newTurret)
    {
        turrets.Add(newTurret);
    }

    public void ClearTurrets ()
    {
        foreach (GameObject turret in turrets)
        {
            Destroy(turret);
        }
        turrets.Clear();
    }
}
