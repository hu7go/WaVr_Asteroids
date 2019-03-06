using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActiveAbilities : MonoBehaviour
{
    public SO_Abilities currentAbility;

    public void ActivateAbility ()
    {
        currentAbility.ability.Effect();
    }
}
