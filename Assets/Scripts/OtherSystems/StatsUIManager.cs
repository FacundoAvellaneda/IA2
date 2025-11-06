using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class StatsUIManager : MonoBehaviour
{
    [Header("Referencias")]
    [SerializeField] private PlayerStats playerStats;
    [SerializeField] private GameObject statDisplayPrefab;
    [SerializeField] private Transform statsContainer;
    [SerializeField] private TMP_Text pointsText;

    private Dictionary<string, StatsDisplay> statDisplays = new();

    private void Start()
    {
        if (playerStats == null)
            playerStats = FindObjectOfType<PlayerStats>();

        BuildStatList();
        UpdateUI();

        playerStats.OnStatsChanged += UpdateUI;
        playerStats.OnLevelUp += OnLevelUp;
    }

    private void OnDestroy()
    {
        if (playerStats != null)
        {
            playerStats.OnStatsChanged -= UpdateUI;
            playerStats.OnLevelUp -= OnLevelUp;
        }
    }

    private void BuildStatList()
    {
        foreach (Transform child in statsContainer)
            Destroy(child.gameObject);
        statDisplays.Clear();

        AddStat("Max Health", "maxHealth", playerStats.maxHealth);
        AddStat("Strength", "strength", playerStats.strength);
        AddStat("Speed", "speed", playerStats.speed);
    }

    private void AddStat(string displayName, string statKey, int value)
    {
        GameObject go = Instantiate(statDisplayPrefab, statsContainer);
        var display = go.GetComponent<StatsDisplay>();
        display.Initialize(displayName, statKey, value, OnAddPoint);
        statDisplays[statKey] = display;
    }

    private void OnAddPoint(string statKey)
    {
        if (playerStats.PointsToSpend > 0)
        {
            playerStats.SpendPoint(statKey);
            UpdateUI();
        }
    }

    private void UpdateUI()
    {
        if (playerStats == null) return;

        pointsText.text = $"Points: {playerStats.PointsToSpend}";

        statDisplays["maxHealth"].UpdateValue(playerStats.maxHealth);
        statDisplays["strength"].UpdateValue(playerStats.strength);
        statDisplays["speed"].UpdateValue(playerStats.speed);

        bool canSpend = playerStats.PointsToSpend > 0;
        foreach (var display in statDisplays.Values)
            display.SetButtonInteractable(canSpend);
    }

    private void OnLevelUp(int newLevel)
    {
        UpdateUI();
    }
}