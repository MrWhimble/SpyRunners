using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class PowerupEffect : ScriptableObject
{
    public float duration;
    public bool instant;

    public abstract void Apply(GameObject target);
    public abstract void Pickup(GameObject target);
    public abstract void Remove(GameObject target);
}
