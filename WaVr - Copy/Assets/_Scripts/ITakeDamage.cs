using System.Collections;
using UnityEngine;

public interface ITakeDamage<T>
{
    void TakeDamage(T damageToGive);
}