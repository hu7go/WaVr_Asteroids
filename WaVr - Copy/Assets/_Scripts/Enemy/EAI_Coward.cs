using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EAI_Coward : EnemyAI
{
    public override void Initialize(List<AsteroidHealth> newList, float newHealthThreshHold, EnemySpawnPoint newMaster, Spawner newSpawner, int newWaveIndex)
    {
        base.Initialize(newList, newHealthThreshHold, newMaster, newSpawner, newWaveIndex);
    }
}
