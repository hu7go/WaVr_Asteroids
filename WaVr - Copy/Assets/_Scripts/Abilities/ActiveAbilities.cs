using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActiveAbilities : MonoBehaviour
{
    public float cooldown = 5;
    public SO_Abilities currentAbility;

    private TeleportRotation controller;
    private bool canUse = true;

    private void Start()
    {
        controller = GetComponent<TeleportRotation>();
    }

    public void ActivateAbility ()
    {
        if (canUse)
        {
            canUse = false;
            currentAbility.ability.Effect(controller.transform);
            StartCoroutine(Cooldown());
        }
    }

    private IEnumerator Cooldown ()
    {
        yield return new WaitForSeconds(cooldown);
        canUse = true;
    }
}
