using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewAbility", menuName = "Abilities/New Ability", order = 0)]
public class AbilityData: ScriptableObject
{
    [Header("Información básica")]
    public string id;
    public string abilityName;
    [TextArea] public string description;
    public Sprite icon;

    [Header("Efectos que aplica")]
    public List<StatEffect> effects = new List<StatEffect>();
}

[System.Serializable]
public struct StatEffect
{
    public StatType stat;
    public int amount;
}


public enum StatType
{
    Strength,
    Speed,
    MaxHealth
}