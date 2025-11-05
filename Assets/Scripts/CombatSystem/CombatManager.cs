using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CombatManager : MonoBehaviour
{
    public static CombatManager Instance { get; private set; }


    void Awake()
    {
        if (Instance != null) Destroy(gameObject);
        else Instance = this;
    }


    public PlayerStats playerStats;


    public void LogAttack(string attacker, string action, int damage)
    {
        var log = new { Attacker = attacker, Action = action, Damage = damage, Time = DateTime.Now };
        Debug.Log($"[LOG] {log.Attacker} did {log.Action} ({log.Damage}) at {log.Time}");
    }


    public void OnEnemyKilled(Enemy enemy)
    {
        if (playerStats != null)
        {
            playerStats.AddExperience(enemy.xpGiven);
            LogAttack(enemy.entityName, "KilledByPlayer", 0);
        }
    }
}
