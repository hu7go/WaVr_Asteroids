using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class MapGenerator : MonoBehaviour
{
    public GameObject asteroid;
    public int width;
    public int height;
    public int depth;
    public int distance;

    [Range(0, 100)]
    public float mapFillPercent;
    public bool useRandomSeed;

    private List<Node> map;
    private int totalPositions;

    public string seed;

    private GameObject middleAsteroid;

    private List<AsteroidPos> asteroids;

    private void Start()
    {
        GenerateMap();
        SpawnAsteroids();
        ProcessMap();

        Manager.Instance.WaitForMapGeneration(middleAsteroid);

        map.Clear();
    }

    private void GenerateMap()
    {
        if (useRandomSeed)
        {
            seed = Mathf.RoundToInt(Random.Range(0, 10000)).ToString();
        }

        System.Random psuedoRandom = new System.Random(seed.GetHashCode());

        map = new List<Node>();

        for (int x = -width / 2; x < width / 2; x++)
        {
            for (int y = -height / 2; y < height / 2; y++)
            {
                for (int z = -depth / 2; z < depth / 2; z++)
                {
                    int tmp = (psuedoRandom.Next(0, 100 * 100) < mapFillPercent * 100) ? 1 : 0;

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
    }

    private void SpawnAsteroids()
    {
        asteroids = new List<AsteroidPos>();
        Manager.Instance.asteroidList = new List<AsteroidHealth>();

        foreach (Node node in map)
        {
            if (node.onOff == 1)
            {
                Vector3 spawnPos = new Vector3(node.x, node.y, node.z);
                GameObject newAsteroid = Instantiate(asteroid, spawnPos, transform.rotation, transform);
                Manager.Instance.asteroidList.Add(newAsteroid.GetComponent<TurretMenuMaster>().asteroid);

                asteroids.Add(new AsteroidPos(false, newAsteroid, null));

                if (node.middleAsteroid)
                {
                    middleAsteroid = newAsteroid;
                }
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
                        asteroids[i].oppositeObj = asteroids[j].obj;
                    }
                }
            }
        }

        bool doAgain = false;

        for (int i = 0; i < asteroids.Count; i++)
        {
            if (asteroids[i].tooClose)
            {
                Vector3 direction = (asteroids[i].obj.transform.position - asteroids[i].oppositeObj.transform.position).normalized;
                asteroids[i].obj.transform.position += direction * 2;

                if (Vector3.Distance(asteroids[i].obj.transform.position, asteroids[i].oppositeObj.transform.position) < distance)
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
        Vector3 topRight = new Vector3(width / 2, height / 2, depth / 2);
        Vector3 topLeft = new Vector3(-width / 2, height / 2, depth / 2);
        Vector3 bottomLeft = new Vector3(-width / 2, -height / 2, depth / 2);
        Vector3 bottomRight = new Vector3(width / 2, -height / 2, depth / 2);
        Vector3 bottomBackLeft = new Vector3(-width / 2, -height / 2, -depth / 2);
        Vector3 bottomBackRight = new Vector3(width / 2, -height / 2, -depth / 2);
        Vector3 topBackLeft = new Vector3(-width / 2, height / 2, -depth / 2);
        Vector3 topBackRight = new Vector3(width / 2, height / 2, -depth / 2);

        Gizmos.color = Color.green;
        Gizmos.DrawCube(topRight, Vector3.one);
        Gizmos.color = Color.blue;
        Gizmos.DrawCube(topLeft, Vector3.one);
        Gizmos.color = Color.cyan;
        Gizmos.DrawCube(bottomLeft, Vector3.one);
        Gizmos.color = Color.black;
        Gizmos.DrawCube(bottomBackLeft, Vector3.one);
        Gizmos.color = Color.magenta;
        Gizmos.DrawCube(bottomRight, Vector3.one);
        Gizmos.color = Color.red;
        Gizmos.DrawCube(bottomBackRight, Vector3.one);
        Gizmos.color = Color.white;
        Gizmos.DrawCube(topBackLeft, Vector3.one);
        Gizmos.color = Color.yellow;
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
    public GameObject obj;
    public GameObject oppositeObj;

    public AsteroidPos (bool close, GameObject newObj, GameObject newOpposite)
    {
        tooClose = close;
        obj = newObj;
        oppositeObj = newOpposite;
    }
}
