using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class AbilityData
{
    public string name;
    public string type;
    public int damageBonus;
    public int speedBonus;
    public int healthBonus;
    public string description;

    public AbilityData(string name, string type, int dmg, int spd, int hp, string desc)
    {
        this.name = name;
        this.type = type;
        this.damageBonus = dmg;
        this.speedBonus = spd;
        this.healthBonus = hp;
        this.description = desc;
    }
}