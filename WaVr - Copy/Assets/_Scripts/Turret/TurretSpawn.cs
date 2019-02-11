using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurretSpawn : MonoBehaviour
{
    public Turret turret;
    [SerializeField] private GameObject turretRange;

    [HideInInspector] public TurretMenuMaster master;
    [HideInInspector] public Transform lookAt;
    [HideInInspector] public Transform turretSpawnpos;
    [Range(0, 5)]
    [HideInInspector] public int index;
    private Transform spawnPos;
    [HideInInspector] public Vector3 rangeOffset;

    GameObject currentRangeIndicator;
    GameObject currentTurret;
    bool tmp = true;

    private void Update()
    {
        if (lookAt != null)
            transform.LookAt(lookAt);
    }

    //Spawns the actual turret!
    public void Confirm ()
    {
        currentTurret = Instantiate(turret.model, spawnPos);
        master.SpawnedTurret(index);

        Manager.Instance.ToggleConfirmDenyButtons();
        Manager.Instance.BuiltTurret(currentTurret);

        RemoveRangeIndicator();
    }

    public void Decline ()
    {
        if (currentTurret != null)
            Destroy(currentTurret);

        Manager.Instance.ToggleConfirmDenyButtons();

        if (!Manager.Instance.turretsAndEnemies.turretHover)
            RemoveRangeIndicator();
    }

    public void SpawnEm()
    {
        spawnPos = turretSpawnpos;

        if (Manager.Instance.turretsAndEnemies.turretHover)
        {
            Confirm();
            Manager.Instance.ToggleConfirmDenyButtons();
        }
        else
        {
            Manager.Instance.ToggleConfirmDenyButtons();
            currentRangeIndicator = Instantiate(turretRange, spawnPos);
        }
        master.RemoveButtons();
    }

    void RemoveRangeIndicator ()
    {
        Destroy(currentRangeIndicator);
    }

    //This is for if the onHover bool is true!
    public void ShowRangeIndicator ()
    {
        spawnPos = turretSpawnpos;

        Vector3 spawn = spawnPos.position + rangeOffset;

        if (currentRangeIndicator == null)
        {
            currentRangeIndicator = Instantiate(turretRange, spawn, spawnPos.rotation, spawnPos);
            currentRangeIndicator.transform.localScale = new Vector3(turret.rangeRadius, turret.rangeRadius, turret.rangeRadius);
        }

        currentRangeIndicator.SetActive(true);
        currentRangeIndicator.transform.position = spawnPos.position;

        StartCoroutine(AutoOff());
    }

    public void DisableRangeIndicator ()
    {
        currentRangeIndicator.SetActive(false);
        StopAllCoroutines();
    }

    IEnumerator AutoOff ()
    {
        yield return new WaitForSeconds(5f);
        DisableRangeIndicator();
    }
}