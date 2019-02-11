using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Utilities : MonoBehaviour
{
    [Range(0.1f, 5f), Tooltip("Use with caution, changes the time scale of the game,\n only used for dev purposes")]
    public float timeScale = 1;

    void Update()
    {
        Time.timeScale = timeScale;
    }
}
