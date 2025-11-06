using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(PlayerController))]
public class PlayerStats : MonoBehaviour, IStats
{
    public int Level { get; private set; } = 1;
    public int Experience { get; private set; } = 0;
    public int PointsToSpend = 0;

    [Header("Stats Base")]
    public int maxHealth = 10;
    public int currentHealth;
    public int strength = 1;
    public int speed = 1;

    [Header("Progresión")]
    public int xpPerLevel = 100;

    public delegate void OnLevelUpHandler(int newLevel);
    public event OnLevelUpHandler OnLevelUp;

    public delegate void OnStatsChangedHandler();
    public event OnStatsChangedHandler OnStatsChanged;

    private void Awake()
    {
        currentHealth = maxHealth;
    }

    public int Damage => strength * 2;

    public void AddExperience(int xp)
    {
        Experience += xp;

        while (Experience >= xpPerLevel)
        {
            Experience -= xpPerLevel;
            Level++;
            PointsToSpend += 3;

            RecalculateStatsAfterLevelUp();
            currentHealth = maxHealth;

            OnLevelUp?.Invoke(Level);
            OnStatsChanged?.Invoke();
        }
    }

    private void RecalculateStatsAfterLevelUp()
    {
        maxHealth += 1;
        speed += 1;
        strength += 1;
    }

    public bool SpendPoint(string statName)
    {
        if (PointsToSpend <= 0)
            return false;

        switch (statName.ToLower())
        {
            case "maxhealth":
                maxHealth += 1;
                currentHealth = maxHealth;
                break;

            case "strength":
                strength += 1;
                break;

            case "speed":
                speed += 1;
                break;

            default:
                Debug.LogWarning($"[PlayerStats] Stat '{statName}' no reconocida.");
                return false;
        }

        PointsToSpend--;
        OnStatsChanged?.Invoke();
        return true;
    }

    public void NotifyStatsChanged()
    {
        OnStatsChanged?.Invoke();
    }
}