using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EAI_Persistant : EnemyAI
{
    [Space(20)]
    public float newDamageThreshHold = 0;

    public override void Initialize(List<AsteroidHealth> newList, float newHealthThreshHold, EnemySpawnPoint newMaster, Spawner newSpawner, int newWaveIndex)
    {
        base.Initialize(newList, newDamageThreshHold, newMaster, newSpawner, newWaveIndex);
    }
}
