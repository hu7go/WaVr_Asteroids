using UnityEngine;

[CreateAssetMenu(fileName = "EnemiesDiedTrigger", menuName = "ScriptableObject/Wave/EnemiesDiedTrigger", order = 3)]
public class EnemiesDiedTrigger : TriggerManager
{
    public override bool Trigger()
    {
        if (prevSpawnPoint == null)
            prevSpawnPoint = Manager.Instance.currentSpawnPoint;

        if (prevSpawnPoint.mySpawner == null)
        {
            return false;
        }

        if (prevSpawnPoint.mySpawner.enemies.Count == 0 && prevSpawnPoint.spawned == true)
        {
            if (prevSpawnPoint.mySpawner.doneSpawning)
            {
                prevSpawnPoint = null;
                return true;
            }
            else
            {
                return false;
            }
        }
        else
        {
            return false;
        }
    }
}