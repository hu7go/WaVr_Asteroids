using UnityEngine;

public class TriggerManager : ScriptableObject
{
    protected EnemySpawnPoint prevSpawnPoint;

    public virtual bool Trigger()
    {
        return false;
    }
}