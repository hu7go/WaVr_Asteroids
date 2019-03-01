using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class WaveSpawnCondition
{
    public delegate bool Function();

    public static bool Trigger (Function newTrigger)
    {
        if (newTrigger())
        {
            return true;
        }
        else
        {
            return false;
        }
    }
}
