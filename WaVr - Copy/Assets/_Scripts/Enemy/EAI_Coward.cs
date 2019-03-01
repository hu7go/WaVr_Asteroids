using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EAI_Coward : EnemyAI
{
    public override void TakeDamage(int damage)
    {
        base.TakeDamage(damage);
        if (!onTheWay)
            nextTargetIndex += 2;
    }

    public override void KilledTarget()
    {
        base.KilledTarget();
        nextTargetIndex = 0;
    }

    public override void SetPath(List<AsteroidHealth> newPath)
    {
        if (objectiveOrder[nextTargetIndex].asteroid.alive == true)
        {
            nextTargetIndex--;
        }
        base.SetPath(newPath);
    }
}
