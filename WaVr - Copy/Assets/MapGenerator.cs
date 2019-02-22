using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class MapGenerator : MonoBehaviour
{
    public GameObject asteroid;
    public List<GameObject> spaceJunkPrefabs;
    public int width;
    public int height;
    public int depth;
    [Tooltip("The min distance between the asteroids in bounds!")]
    public int distance;
    [Tooltip("The distance from the edge of the bounds to the portals!")]
    public float portalDistanceOffset = 10;

    [Range(0, 100)]
    public float mapFillPercent;
    [Range(0, 100)]
    public float junkFillPercent;
    public bool useRandomSeed;

    private List<Node> map;
    private int totalPositions;

    public string seed;

    private GameObject middleAsteroid;

    private List<AsteroidPos> asteroids;
    private List<Transform> spaceJunkList = new List<Transform>();
    private Transform spaceShip;

    private Vector3 topRight;
    private Vector3 topLeft;
    private Vector3 bottomLeft;
    private Vector3 bottomRight;
    private Vector3 bottomBackLeft;
    private Vector3 bottomBackRight;
    private Vector3 topBackLeft;
    private Vector3 topBackRight;

    private Vector3 above;
    private Vector3 forward;
    private Vector3 back;
    private Vector3 down;
    private Vector3 left;
    private Vector3 right;

    List<Vector3> spawns = new List<Vector3>();

    private void Start()
    {
        map = new List<Node>();

        if (useRandomSeed)
            seed = Mathf.RoundToInt(Random.Range(0, 10000)).ToString();

        topRight = new Vector3(width / 2, height / 2, depth / 2);
        topLeft = new Vector3(-width / 2, height / 2, depth / 2);
        bottomLeft = new Vector3(-width / 2, -height / 2, depth / 2);
        bottomRight = new Vector3(width / 2, -height / 2, depth / 2);
        bottomBackLeft = new Vector3(-width / 2, -height / 2, -depth / 2);
        bottomBackRight = new Vector3(width / 2, -height / 2, -depth / 2);
        topBackLeft = new Vector3(-width / 2, height / 2, -depth / 2);
        topBackRight = new Vector3(width / 2, height / 2, -depth / 2);

        above = transform.position + (topRight + topLeft + topBackRight + topBackLeft).normalized * height / 2 * portalDistanceOffset;
        forward = transform.position + (topRight + topLeft + bottomLeft + bottomRight).normalized * depth / 2 * portalDistanceOffset;
        back = transform.position + (topBackLeft + topBackRight + bottomBackLeft + bottomBackRight).normalized * depth / 2 * portalDistanceOffset;
        down = transform.position + (bottomBackLeft + bottomBackRight + bottomLeft + bottomRight).normalized * height / 2 * portalDistanceOffset;
        left = transform.position + (topLeft + topBackLeft + bottomLeft + bottomBackLeft).normalized * width / 2 * portalDistanceOffset;
        right = transform.position + (bottomRight + bottomBackRight + topRight + topBackRight).normalized * width / 2 * portalDistanceOffset;
        spawns.Add(left);
        spawns.Add(right);
        spawns.Add(down);
        spawns.Add(above);
        spawns.Add(forward);
        spawns.Add(left);

        //mapFillPercent = ((Manager.Instance.wantedMaxHealth) / (width * height * depth * Manager.Instance.tAe.asteroidHealth)) * 100f;

        //float currentPercent = Manager.Instance.wantedMaxHealth;
        //float maxPercent = (width * height * depth) * Manager.Instance.tAe.asteroidHealth;
        //mapFillPercent = currentPercent / maxPercent;

        //mapFillPercent = (Manager.Instance.wantedMaxHealth / (Manager.Instance.wantedMaxHealth * 100f));

        GenerateMap();

        if (Manager.Instance.graphicsSettings.worldVersion == Manager.GraphicsSettings.WorldVersion.three)
            spaceShip = Manager.Instance.ReturnSpaceShip();

        SpawnAsteroids();
        ProcessMap();

        Manager.Instance.WaitForMapGeneration(middleAsteroid, spawns);
        map.Clear();
    }

    private void GenerateMap()
    {
        System.Random psuedoRandom = new System.Random(seed.GetHashCode());
        System.Random psuedoRandomJunk = new System.Random(seed.GetHashCode());


        for (int x = -width / 2; x < width / 2; x++)
        {
            for (int y = -height / 2; y < height / 2; y++)
            {
                for (int z = -depth / 2; z < depth / 2; z++)
                {
                    int tmp = (psuedoRandom.Next(0, 100 * 1000000) < mapFillPercent * 1000000) ? 1 : 0;

                    bool tmpBool = false;

                    if (tmp == 0)
                        tmpBool = (psuedoRandomJunk.Next(0, 100 * 1000000) < junkFillPercent * 1000000) ? true : false;

                    Node newNode = new Node(x, y, z, tmp, false, tmpBool);
                    if (x == 0 && y == 0 && z == 0)
                    {
                        newNode.onOff = 1;
                        newNode.middleAsteroid = true;
                    }
                    map.Add(newNode);
                }
            }
        }
    }

    private void SpawnAsteroids()
    {
        asteroids = new List<AsteroidPos>();
        Manager.Instance.asteroidList = new List<AsteroidHealth>();

        System.Random psuedoRandom = new System.Random(seed.GetHashCode());
        System.Random psuedoRandomRot = new System.Random(seed.GetHashCode());

        foreach (Node node in map)
        {
            if (node.onOff == 1)
            {
                Vector3 spawnPos = new Vector3(node.x, node.y, node.z);
                GameObject newAsteroid = Instantiate(asteroid, spawnPos, transform.rotation, transform);
                Manager.Instance.asteroidList.Add(newAsteroid.GetComponent<TurretMenuMaster>().asteroid);

                asteroids.Add(new AsteroidPos(false, newAsteroid, null));

                if (node.middleAsteroid)
                    middleAsteroid = newAsteroid;
            }
            if (node.spaceJunk)
            {
                Vector3 spawnPos = new Vector3(node.x, node.y, node.z);

                Quaternion randomRot = Quaternion.Euler(psuedoRandomRot.Next(0, 360), psuedoRandomRot.Next(0, 360), psuedoRandomRot.Next(0, 360));
                GameObject newJunk = Instantiate(spaceJunkPrefabs[psuedoRandom.Next(0, spaceJunkPrefabs.Count)], spawnPos, randomRot, transform);
                spaceJunkList.Add(newJunk.transform);
            }
        }
    }

    int failSafe = 0;

    private void ProcessMap()
    {
        for (int i = 0; i < asteroids.Count; i++)
        {
            for (int j = 0; j < asteroids.Count; j++)
            {
                if (i != j)
                {
                    if (Vector3.Distance(asteroids[i].obj.transform.position, asteroids[j].obj.transform.position) < distance
                        || Vector3.Distance(asteroids[i].obj.transform.position, Manager.Instance.graphicsSettings.spaceShip.transform.position) < distance * 2)
                    {
                        asteroids[i].tooClose = true;

                        asteroids[i].oppositeObj = asteroids[j].obj.transform;
                    }
                }

                if (spaceJunkList.Count != 0)
                {
                    for (int k = 0; k < spaceJunkList.Count; k++)
                    {
                        if (Vector3.Distance(asteroids[i].obj.transform.position, spaceJunkList[k].position) < distance * 1.5f)
                        {
                            asteroids[i].tooClose = true;
                            asteroids[i].oppositeObj = spaceJunkList[k];
                        }
                    }
                }
            }
        }

        bool doAgain = false;

        for (int i = 0; i < asteroids.Count; i++)
        {
            if (asteroids[i].tooClose)
            {
                Vector3 direction = (asteroids[i].obj.transform.position - asteroids[i].oppositeObj.position).normalized;
                asteroids[i].obj.transform.position += direction * 2;

                if (Vector3.Distance(asteroids[i].obj.transform.position, asteroids[i].oppositeObj.position) < distance)
                    asteroids[i].tooClose = true;
                else
                    asteroids[i].tooClose = false;

                if (doAgain == false)
                    failSafe++;

                doAgain = true;
            }
        }

        if (doAgain)
            ProcessMap();
    }

    //! Gizmos!
    private void OnDrawGizmos()
    {
        topRight = new Vector3(width / 2, height / 2, depth / 2);
        topLeft = new Vector3(-width / 2, height / 2, depth / 2);
        bottomLeft = new Vector3(-width / 2, -height / 2, depth / 2);
        bottomRight = new Vector3(width / 2, -height / 2, depth / 2);
        bottomBackLeft = new Vector3(-width / 2, -height / 2, -depth / 2);
        bottomBackRight = new Vector3(width / 2, -height / 2, -depth / 2);
        topBackLeft = new Vector3(-width / 2, height / 2, -depth / 2);
        topBackRight = new Vector3(width / 2, height / 2, -depth / 2);

        Gizmos.color = Color.yellow;
        Gizmos.DrawCube(topRight, Vector3.one);
        Gizmos.DrawCube(topLeft, Vector3.one);
        Gizmos.DrawCube(bottomLeft, Vector3.one);
        Gizmos.DrawCube(bottomBackLeft, Vector3.one);
        Gizmos.DrawCube(bottomRight, Vector3.one);
        Gizmos.DrawCube(bottomBackRight, Vector3.one);
        Gizmos.DrawCube(topBackLeft, Vector3.one);
        Gizmos.DrawCube(topBackRight, Vector3.one);

        Gizmos.DrawLine(topRight, topLeft);
        Gizmos.DrawLine(topLeft, topBackLeft);
        Gizmos.DrawLine(topBackLeft, topBackRight);
        Gizmos.DrawLine(topBackRight, topRight);
        Gizmos.DrawLine(topRight, bottomRight);
        Gizmos.DrawLine(bottomRight, bottomLeft);
        Gizmos.DrawLine(bottomLeft, bottomBackLeft);
        Gizmos.DrawLine(bottomBackLeft, bottomBackRight);
        Gizmos.DrawLine(bottomBackRight, bottomRight);
        Gizmos.DrawLine(bottomBackRight, topBackRight);
        Gizmos.DrawLine(bottomBackLeft, topBackLeft);
        Gizmos.DrawLine(bottomLeft, topLeft);

        Gizmos.color = Color.red;
        
        if (Application.isPlaying)
        {
            foreach (AsteroidPos asteroidPos in asteroids)
            {
                if (asteroidPos.tooClose == true)
                {
                    Gizmos.DrawCube(asteroidPos.obj.transform.position, Vector3.one * 2);
                }
            }
        }

        Gizmos.color = Color.green;

        Gizmos.DrawCube(above, Vector3.one * 3);
        Gizmos.DrawCube(forward, Vector3.one * 3);
        Gizmos.DrawCube(back, Vector3.one * 3);
        Gizmos.DrawCube(down, Vector3.one * 3);
        Gizmos.DrawCube(right, Vector3.one * 3);
        Gizmos.DrawCube(left, Vector3.one * 3);
    }
}

[System.Serializable]
public struct Node
{
    public int x;
    public int y;
    public int z;
    //If onOff = 1 its on, if its 0 its off!
    public int onOff;
    public bool spaceJunk;
    public bool middleAsteroid;

    public Node (int newX, int newY, int newZ, int newOnOff, bool middle, bool spaceJunk)
    {
        x = newX;
        y = newY;
        z = newZ;
        onOff = newOnOff;
        middleAsteroid = middle;
        this.spaceJunk = spaceJunk;
    }
}

public class AsteroidPos
{
    public bool tooClose;
    public GameObject obj;
    public Transform oppositeObj;

    public AsteroidPos (bool close, GameObject newObj, Transform newOpposite)
    {
        tooClose = close;
        obj = newObj;
        oppositeObj = newOpposite;
    }
}
