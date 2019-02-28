using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EAI_Coward : EnemyAI
{
    public override void TakeDamage(int damage)
    {
        base.TakeDamage(damage);
        if (onTheWay)
            nextTargetIndex++;
    }

    public override void KilledTarget()
    {
        nextTargetIndex = 0;
    }
}
