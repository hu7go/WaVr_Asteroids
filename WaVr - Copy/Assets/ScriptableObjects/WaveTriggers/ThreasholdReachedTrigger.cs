using UnityEngine;

[CreateAssetMenu(fileName = "ThreasholdReachedTrigger", menuName = "ScriptableObject/Wave/ThreasholdReachedTrigger", order = 2)]
public class ThreasholdReachedTrigger : TriggerManager
{
    float currentMaxHealth = 100;

    public override bool Trigger()
    {
        if (prevSpawnPoint == null)
        {
            currentMaxHealth = Manager.Instance.healthPercent;
            prevSpawnPoint = Manager.Instance.currentSpawnPoint;
        }

        if (Manager.Instance.healthPercent <= prevSpawnPoint.myWaveInfo.damageThreshHold)
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