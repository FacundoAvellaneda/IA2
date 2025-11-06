using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelingSystem : MonoBehaviour
{
    public PlayerStats playerStats;


    void Start()
    {
        if (playerStats != null)
            playerStats.OnLevelUp += HandleLevelUp;
    }


    void HandleLevelUp(int newLevel)
    {


    }
}