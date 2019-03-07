using UnityEngine;

[CreateAssetMenu(fileName = "TimerTrigger", menuName = "ScriptableObject/Wave/TimerTrigger", order = 4)]
public class TimerTrigger : TriggerManager
{
    private float timer = 0;

    public override bool Trigger()
    {
        if (prevSpawnPoint == null)
        {
            timer = 0;
            prevSpawnPoint = Manager.Instance.currentSpawnPoint;
        }

        if (timer > prevSpawnPoint.myWaveInfo.waitTime)
        {
            prevSpawnPoint = null;
            return true;
        }
        else
        {
            timer += Time.deltaTime;
            return false;
        }
    }
}