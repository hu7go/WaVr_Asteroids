using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
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
        public VRTK_StraightPointerRenderer[] renderers;
    }
    [Space(10)]
    public DayDreamSettings daydreamSettings;

    [System.Serializable]
    public class TurretAndEnemiesSettings
    {
        public bool turretHover = false;
        public bool towerDefence;

        public GameObject enemySpawner;
        public GameObject healer;
        [HideInInspector] public GameObject enemySpawnPoint;
        public int waveCount = 0;
        public Transform currentActiveSpawner;

        public float asteroidHealth = 200;

        [Tooltip("This is a percent value so: 0 = 0%, 100 = 100%")]
        [Range(0, 100)]
        public float loseThreshHold = 51;

        [HideInInspector] public List<EnemySpawnPoint> spawnPoints = new List<EnemySpawnPoint>();
    }
    [Space(10)]
    [Tooltip("The turrets and enemies settings!")]
    public TurretAndEnemiesSettings tAe;

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
        public Text healthPercentText;
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
    [SerializeField] private GameObject playerBody;

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

    /*[HideInInspector]*/ public float masterCurrentHealth;
    /*[HideInInspector]*/ public float masterMaxHealth;
    public float wantedMaxHealth = 11200;
    public float healthPercent;

    int minutes ,minutes2;
    int seconds,seconds2;

    [HideInInspector] public GameObject enemyParent;

    [Space(20)]
    private List<GameObject> turrets;

    [HideInInspector] public bool spawnedFirstTurret = false;

    private int numberOfEnemies = 0;

    public List<AsteroidHealth> asteroidList;
    public HealerInfo healerInfoTemplate;

    [Header("Control the waves!")]
    [Space(20)]
    public List<Wave> waves;
    [Space(20)]

    public float minWaveWaitTime = 20;
    public float maxWaveWaitTime = 40;

    public MeshRenderer arrowRenderer;
    public TeleportMaster tpMaster;

    [HideInInspector] public TurretReloader turretReload;

    public bool bendLine = false;

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

    public Transform ReturnSpaceShip ()
    {
        return graphicsSettings.spaceShip.transform;
    }

    public void WaitForMapGeneration (GameObject middleAsteroid, List<Vector3> enemySpawnPositions)
    {
        int j = 0;
        foreach (Wave wave in waves)
        {
            wave.spawnPosition.position = enemySpawnPositions[j];
            j++;
        }

        turretReload = GetComponent<TurretReloader>();

        turrets = new List<GameObject>();
        enemiesSpawned = new List<GameObject>();
        enemyParent = new GameObject("enemyParent");

        //add a enemyspawnlocation look at objective
        SetPointerState(Enums.PointerState.Teleport);

        switch (graphicsSettings.cubesOn)
        {
            case true:
                foreach (AsteroidHealth mesh in asteroidList)
                    mesh.GetComponent<MeshRenderer>().enabled = true;
                graphicsSettings.asteroids.SetActive(false);
                break;
            case false:
                foreach (AsteroidHealth mesh in asteroidList)
                    mesh.GetComponent<MeshRenderer>().enabled = false;
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

        tpMaster.SetFirstAsteroid(middleAsteroid.GetComponent<SideScript>());
    }

    public void UpdateHealth (float newHealth)
    {
        masterCurrentHealth += newHealth;
        uISettings.healthSlider.value = masterCurrentHealth;

        if (healthPercent <= tAe.loseThreshHold)
            GameOver();
    }

    public void UsingDayDream()
    {
        daydreamSettings.usingDayDream = true;

        graphicsSettings.skyboxOne.SetFloat("_Exposure", 2.5f);

        for (int i = 0; i < daydreamSettings.dayDreamBoxIncreases.Length; i++)
            daydreamSettings.dayDreamBoxIncreases[i].size *= 4;
    }

    public IEnumerator SpawnThemNewEnemies()
    {
        if (tAe.waveCount + 1 > waves.Count)
        {
            StartCoroutine(WaitForAllEnemies());
            yield return null;
        }
        
        yield return new WaitForSeconds(minWaveWaitTime);

        float waveDelayPercent = (waves[tAe.waveCount - 1].currentNumberOfEnemies / waves[tAe.waveCount - 1].maxNumberOfEnemies);

        if (waveDelayPercent == 0)
            waveDelayPercent = .1f;

        float timeToWait = maxWaveWaitTime * waveDelayPercent;

        Invoke("SpawnEnemie", timeToWait);
    }

    bool lost = false;

    private IEnumerator WaitForAllEnemies ()
    {
        while (enemiesSpawned.Count > 0)
        {
            yield return null;
        }

        if (lost == false)
        {
            ObjectiveReached();
        }

    }

    void SpawnEnemie () => StartSpawningEnemies();

    public void StartSpawningEnemies()
    {
        if (tAe.towerDefence)
        {
            uISettings.waveCount.text = ("Wave: " + (tAe.waveCount + 1));

            EnemySpawner();
        }
    }

    private void EnemySpawner ()
    {
        ClearNullRefs();

        if (tAe.waveCount > waves.Count + 1)
            return;

        GameObject localEnemySpawner = Instantiate(tAe.enemySpawner, waves[tAe.waveCount].spawnPosition.position, transform.rotation);
        EnemySpawnPoint tmpSpawnPoint = localEnemySpawner.GetComponent<EnemySpawnPoint>();
        tAe.spawnPoints.Add(tmpSpawnPoint);
        //Starts the spawning process for the enemies, spawns 'Y' amount of enemies after 'X' amount of time!
        //                                                             X   '...            Y               ...'
        tmpSpawnPoint.StartSpawner(20, (int)waves[tAe.waveCount].maxNumberOfEnemies, asteroidList, (int)waves[tAe.waveCount].damageThreshHold, tAe.waveCount, waves[tAe.waveCount]);
        //localEnemySpawner.GetComponent<EnemySpawnPoint>().StartSpawner(20, turretsAndEnemies.maxNumberOfEnemies, asteroidList, enemyDestructionPercent);
        //

        tAe.currentActiveSpawner = waves[tAe.waveCount].spawnPosition;

        counter = 0;
        tAe.enemySpawnPoint = localEnemySpawner;

        //This start the next wave after x amount of time!
        //Invoke("StartSpawningEnemies", 30);

        tAe.waveCount++;
    }

    public void RoutineOpener() => EnemySpawner();

    public void InstantiateEnemy(GameObject newEnemy, int index)
    {
        waves[index].currentNumberOfEnemies++;

        enemiesSpawned.Add(newEnemy);

        numberOfEnemies++;
    }

    public void RemoveEnemy(int index)
    {
        waves[index].currentNumberOfEnemies--;

        numberOfEnemies--;

        Invoke("ClearNullRefs", .2f);
    }

    public void GameOver()
    {
        lost = true;

        //if objective dies
        StopCoroutine("EnemySpawner");
        StopCoroutine("SpawnEnemyObjective");
        startTimer = false;
        tAe.waveCount = 0;
        myTimer = 0;
        masterCurrentHealth = masterMaxHealth;
        uISettings.healthSlider.value = masterCurrentHealth;

        foreach (AsteroidHealth asteroid in asteroidList)
        {
            asteroid.asteroid.health = tAe.asteroidHealth;
            asteroid.Revive();
            asteroid.UpdateColor();
        }

        for (var i = enemiesSpawned.Count - 1; i > -1; i--)
        {
            if (enemiesSpawned[i] == null)
            {
                enemiesSpawned.RemoveAt(i);
            }
        }

        foreach (GameObject obj in enemiesSpawned)
        {
            obj.GetComponent<EnemyAI>().GoHome();
        }

        if(lifeLeft > 0)
        {
            uISettings.tdEndUI.SetActive(true);
            uISettings.tdGameOverText.text = "You died, press to restart";
        }
        if(lifeLeft == 0)
            uISettings.tdGameOverText.text = "You died. Thanks for playing!";

        ClearTurrets();
    }

    public void Restarter()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);

        return;

        startTimer = true;
        uISettings.tdEndUI.SetActive(false);
        killedEnemies = 0;
        tAe.waveCount = 0;

        spawnedFirstTurret = false;

        uISettings.waveCount.text = ("Wave: " + (tAe.waveCount + 1));

        RoutineOpener();
    }

    public void ClearNullRefs ()
    {
        for (var i = enemiesSpawned.Count - 1; i > -1; i--)
            if (enemiesSpawned[i] == null)
                enemiesSpawned.RemoveAt(i);

        if (tAe.waveCount > waves.Count && (enemiesSpawned.Count == 0))
        {
            ObjectiveReached();
        }
    }

    public void StartTimer() => startTimer = !startTimer;

    public bool GetStartBool() => startTimer;

    //! Update Function!!!!!
    private void Update()
    {
        healthPercent = ((masterCurrentHealth / masterMaxHealth) * 100);
        uISettings.healthPercentText.text = Mathf.Floor(healthPercent) + "%";
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
        //Destroy(uISettings.startButton);
    }

    public void StartEnemyWaves ()
    {
        if (spawnedFirstTurret == false)
        {
            StartSpawningEnemies();
            spawnedFirstTurret = true;
        }
    }

    public bool StartedGame() => gameStarted;

    public void ObjectiveReached()
    {
        startTimer = false;
        uISettings.endUI.SetActive(true);
        uISettings.overText.text = ("Nice! You destroyed the enemies in: " + minutes.ToString() + ": " + seconds.ToString("00") + " seconds.\n\nWith " + (int)healthPercent + "% health left!");
    }

    public GameObject ReturnPlayer() => player;

    public GameObject ReturnPlayerBody() => playerBody;

    TurretSpawn turretSpawn;

    public void CurrentBuildTarget(TurretSpawn current) => turretSpawn = current;

    public TurretSpawn GetCurrentBuildTarget() => turretSpawn;

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

                foreach (AsteroidHealth asteroid in asteroidList)
                    asteroid.ColliderOn();

                foreach (IndexNode index in indexNodes)
                    index.Off();
                break;
            case Enums.PointerState.Build:
                DeactivateRotationArrows();

                uISettings.stateButtons[0].material = graphicsSettings.defaultMat;
                uISettings.stateButtons[1].material = graphicsSettings.selectedMat;
                uISettings.stateButtons[2].material = graphicsSettings.defaultMat;

                foreach (AsteroidHealth asteroid in asteroidList)
                    asteroid.ColliderOff();

                foreach (IndexNode index in indexNodes)
                    index.Off();
                break;
            case Enums.PointerState.Rotate:
                ActivateRotationArrows();

                uISettings.stateButtons[0].material = graphicsSettings.defaultMat;
                uISettings.stateButtons[1].material = graphicsSettings.defaultMat;
                uISettings.stateButtons[2].material = graphicsSettings.selectedMat;

                foreach (AsteroidHealth asteroid in asteroidList)
                    asteroid.ColliderOff();

                foreach (IndexNode index in indexNodes)
                    index.On();
                break;
        }
    }

    public void ActivateRotationArrows () => uISettings.arrowsUI.SetActive(true);

    public void DeactivateRotationArrows () => uISettings.arrowsUI.SetActive(false);

    public float RetAsteroidSize () => asteroidSize;

    public void BuiltTurret (GameObject newTurret) => turrets.Add(newTurret);

    public void ClearTurrets ()
    {
        foreach (GameObject turret in turrets)
        {
            Destroy(turret);
        }
        turrets.Clear();
    }

    //This currently only works with one active portal at a time!
    public void UpdatePath (Vector3 pos, EnemySpawnPoint currentSpawnPoint) => currentSpawnPoint.FindPath(pos);

    public void SwitchPortalTarget () => tAe.currentActiveSpawner = waves[tAe.waveCount].spawnPosition;
}

[System.Serializable]
public class Wave
{
    public float maxNumberOfEnemies;
    public float damageThreshHold;
    public Transform spawnPosition;
    public List<Enemies> enemyTypes;
    [HideInInspector] public float currentNumberOfEnemies;
    [HideInInspector] public List<EnemyAI> enemies;
    [HideInInspector] public float damageDone;
}