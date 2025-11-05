using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface Idamageable
{
    public void TakeDamage(int damageAmount);
}

public interface  IStats
{
    int Level { get; }
    int Experience { get; }
    void AddExperience(int xp);
}

