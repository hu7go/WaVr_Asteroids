using UnityEngine;

[CreateAssetMenu(fileName = "DamageDoneTrigger", menuName = "ScriptableObject/Wave/DamageDoneTrigger", order = 3)]
public class DamageDoneTrigger : TriggerManager
{
    float currentMaxHealth = 100;

    public override bool Trigger()
    {
        if (prevSpawnPoint == null)
        {
            currentMaxHealth = Manager.Instance.healthPercent;
            prevSpawnPoint = Manager.Instance.currentSpawnPoint;
        }

        if (prevSpawnPoint.damageDonePercent >= 100 - prevSpawnPoint.myWaveInfo.damageThreshHold)
        {
            prevSpawnPoint = null;
            return true;
        }
        else
        {
            return false;
        }
    }
}