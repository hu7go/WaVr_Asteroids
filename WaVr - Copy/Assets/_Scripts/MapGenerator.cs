using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapGenerator : MonoBehaviour
{
    public GameObject asteroid;
    public int width;
    public int height;
    public int depth;
    [Tooltip("The min distance between the asteroids in bounds!")]
    public int distance;
    [Tooltip("The distance from the edge of the bounds to the portals!")]
    public float portalDistanceOffset = 10;

    [Range(0, 100)]
    public float mapFillPercent;
    [Space(20)]
    [Range(0, 100)]
    public float junkFillPercent;
    public bool minorSpaceJunk = true;
    public List<GameObject> spaceJunkPrefabs;
    public bool majorSpaceJunk = false;
    public int numberOfMajorSpaceJunk = 1;
    public List<GameObject> majorSpaceJunkPrefabs;

    [Space(20)]
    public bool useRandomSeed;
    public string seed;

    private List<Node> map;
    private int totalPositions;
    private GameObject middleAsteroid;
    private List<AsteroidPos> asteroids;
    private List<Transform> spaceJunkList = new List<Transform>();
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
    private List<Vector3> spawns = new List<Vector3>();
    private float numberOfAsteroids = 100;
    private int halfWidth;
    private int halfHeight;
    private int halfDepth;

    private void Start()
    {
        map = new List<Node>();

        if (useRandomSeed)
            seed = Mathf.RoundToInt(Random.Range(0, 10000)).ToString();

        System.Random psuedoRandom = new System.Random(seed.GetHashCode());

        halfWidth = width / 2;
        halfHeight = height / 2;
        halfDepth = depth / 2;
        topRight = new Vector3(halfWidth, halfHeight, halfDepth);
        topLeft = new Vector3(-halfWidth, halfHeight, halfDepth);
        bottomLeft = new Vector3(-halfWidth, -halfHeight, halfDepth);
        bottomRight = new Vector3(halfWidth, -halfHeight, halfDepth);
        bottomBackLeft = new Vector3(-halfWidth, -halfHeight, -halfDepth);
        bottomBackRight = new Vector3(halfWidth, -halfHeight, -halfDepth);
        topBackLeft = new Vector3(-halfWidth, halfHeight, -halfDepth);
        topBackRight = new Vector3(halfWidth, halfHeight, -halfDepth);

        int min = -25;
        int max = 25;

        above = (transform.position + (topRight + topLeft + topBackRight + topBackLeft).normalized * halfHeight * portalDistanceOffset) + RandomVector(psuedoRandom, min, max);
        forward = (transform.position + (topRight + topLeft + bottomLeft + bottomRight).normalized * halfDepth * portalDistanceOffset) + RandomVector(psuedoRandom, min, max);
        back = (transform.position + (topBackLeft + topBackRight + bottomBackLeft + bottomBackRight).normalized * halfDepth * portalDistanceOffset) + RandomVector(psuedoRandom, min, max);
        down = (transform.position + (bottomBackLeft + bottomBackRight + bottomLeft + bottomRight).normalized * halfHeight * portalDistanceOffset) + RandomVector(psuedoRandom, min, max);
        left = (transform.position + (topLeft + topBackLeft + bottomLeft + bottomBackLeft).normalized * halfWidth * portalDistanceOffset) + RandomVector(psuedoRandom, min, max);
        right = (transform.position + (bottomRight + bottomBackRight + topRight + topBackRight).normalized * halfWidth * portalDistanceOffset) + RandomVector(psuedoRandom, min, max);

        spawns.Add(left);
        spawns.Add(right);
        spawns.Add(down);
        spawns.Add(above);
        spawns.Add(forward);
        spawns.Add(left);

        numberOfAsteroids = (Manager.Instance.wantedMaxHealth / Manager.Instance.tAe.asteroidHealth);
        numberOfAsteroids = Mathf.RoundToInt((numberOfAsteroids / Manager.Instance.tAe.asteroidHealth) * Manager.Instance.tAe.asteroidHealth);

        GenerateMap();

        SpawnAsteroids();
        ProcessMap();

        Manager.Instance.WaitForMapGeneration(middleAsteroid, spawns, seed);
        map.Clear();
    }

    Vector3 RandomVector(System.Random psuedoRandom,int min, int max) => new Vector3(psuedoRandom.Next(min, max), psuedoRandom.Next(min, max), psuedoRandom.Next(min, max));

    private void GenerateMap()
    {
        System.Random psuedoRandom = new System.Random(seed.GetHashCode());
        System.Random psuedoRandomJunk = new System.Random(seed.GetHashCode());


        for (int x = -halfWidth; x < halfWidth; x++)
        {
            for (int y = -halfHeight; y < halfHeight; y++)
            {
                for (int z = -halfDepth; z < halfDepth; z++)
                {
                    int tmp = 0;

                    //Spawns junk!
                    if (minorSpaceJunk)
                    {
                        if (tmp == 0)
                            tmp = (psuedoRandomJunk.Next(0, 100 * 1000000) < junkFillPercent * 1000000) ? 2 : 0;
                    }
                    //

                    Node newNode = new Node(x, y, z, tmp, false);
                    if (x == 0 && y == 0 && z == 0)
                    {
                        newNode.onOff = 1;
                        newNode.middleAsteroid = true;
                    }
                    map.Add(newNode);
                }
            }
        }

        for (int i = 0; i < numberOfAsteroids - 1; i++)
        {
            int ast = psuedoRandom.Next(0, map.Count);

            if (map[ast].onOff == 0)
            {
                map[ast].onOff = 1;
            }
        }

        if (majorSpaceJunk)
        {
            for (int i = 0; i < numberOfMajorSpaceJunk; i++)
            {
                int randomNumber = psuedoRandomJunk.Next(0, map.Count);
                if (map[randomNumber].onOff == 0)
                {
                    map[randomNumber].onOff = 3;
                }
                else
                {
                    i--;
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
            Vector3 spawnPos = new Vector3(node.x, node.y, node.z);

            if (node.onOff == 1)
            {
                GameObject newAsteroid = Instantiate(asteroid, spawnPos, transform.rotation, transform);
                Manager.Instance.asteroidList.Add(newAsteroid.GetComponent<TurretMenuMaster>().asteroid);

                bool mid = false;

                if (node.middleAsteroid)
                {
                    middleAsteroid = newAsteroid;
                    mid = true;
                }

                asteroids.Add(new AsteroidPos(false, newAsteroid, null, mid));

            }
            if (node.onOff == 2)
            {
                Quaternion randomRot = Quaternion.Euler(psuedoRandomRot.Next(0, 360), psuedoRandomRot.Next(0, 360), psuedoRandomRot.Next(0, 360));
                GameObject newJunk = Instantiate(spaceJunkPrefabs[psuedoRandom.Next(0, spaceJunkPrefabs.Count)], spawnPos, randomRot, transform);
                spaceJunkList.Add(newJunk.transform);
            }
            if (node.onOff == 3)
            {
                Quaternion randomRot = Quaternion.Euler(psuedoRandomRot.Next(0, 360), psuedoRandomRot.Next(0, 360), psuedoRandomRot.Next(0, 360));
                GameObject newJunk = Instantiate(majorSpaceJunkPrefabs[psuedoRandom.Next(0, majorSpaceJunkPrefabs.Count)], spawnPos, randomRot, transform);
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
                    if (Vector3.Distance(asteroids[i].obj.transform.position, asteroids[j].obj.transform.position) < distance)
                    {
                        asteroids[i].tooClose = true;

                        asteroids[i].oppositeObj = asteroids[j].obj.transform;
                    }
                }

                if (spaceJunkList.Count != 0)
                {
                    for (int k = 0; k < spaceJunkList.Count; k++)
                    {
                        if (Vector3.Distance(asteroids[i].obj.transform.position, spaceJunkList[k].position) < distance * 2f)
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
public class Node
{
    public int x;
    public int y;
    public int z;
    //If onOff = 1 its on, if its 0 its off!
    public int onOff;
    public bool middleAsteroid;

    public Node (int newX, int newY, int newZ, int newOnOff, bool middle)
    {
        x = newX;
        y = newY;
        z = newZ;
        onOff = newOnOff;
        middleAsteroid = middle;
    }
}

public class AsteroidPos
{
    public bool tooClose;
    public bool middle;
    public GameObject obj;
    public Transform oppositeObj;

    public AsteroidPos (bool close, GameObject newObj, Transform newOpposite, bool middle)
    {
        tooClose = close;
        obj = newObj;
        oppositeObj = newOpposite;
        this.middle = middle;
    }
}