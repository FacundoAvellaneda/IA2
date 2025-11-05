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
        Debug.Log($"Player subió a nivel {newLevel}!");
        // Abrir UI para gastar puntos, etc. Aquí solo demostramos la tupla retorno
        var result = playerStats.SpendPoints(1, 1, 1); 
        Debug.Log($"Nivel ahora: {result.levelAfter}, puntos restantes: {result.pointsLeft}");
    }
}
