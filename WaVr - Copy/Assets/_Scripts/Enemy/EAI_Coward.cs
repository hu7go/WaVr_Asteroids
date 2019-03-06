using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EAI_Coward : EnemyAI
{
    public override void TakeDamage(float damage)
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
}
