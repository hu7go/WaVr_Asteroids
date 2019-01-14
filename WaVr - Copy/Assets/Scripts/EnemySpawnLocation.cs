using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawnLocation : MonoBehaviour {
    public GameObject objective;
	// Use this for initialization
	void Start () {
        transform.LookAt(objective.transform.position);
		
	}
	
	// Update is called once per frame
	void Update () {

	}
}