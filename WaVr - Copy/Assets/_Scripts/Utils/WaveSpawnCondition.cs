using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaveSpawnCondition
{
    public delegate bool Function();

    public int Trigger (List<Function> newTrigger)
    {
        int numberOfDoneConditions = 0;

        for (int i = 0; i < newTrigger.Count; i++)
        {
            if (newTrigger[i]() == true)
            {
                numberOfDoneConditions++;
            }
            else
            {

            }
        }

        return numberOfDoneConditions;
    }
}
