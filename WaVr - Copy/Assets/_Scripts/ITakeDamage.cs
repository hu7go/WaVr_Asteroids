using System.Collections;
using UnityEngine;

public interface ITakeDamage<T, E>
{
    void TakeDamage(T damageToGive, E spawn);
}