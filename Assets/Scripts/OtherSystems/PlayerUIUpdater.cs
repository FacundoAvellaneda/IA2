using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerUIUpdater : MonoBehaviour
{
    [Header("Referencias")]
    [SerializeField] private PlayerStats playerStats;

    [Header("UI Elements (usando imágenes)")]
    [SerializeField] private Image healthFill;
    [SerializeField] private Image xpFill;
    [SerializeField] private TMP_Text healthText;
    [SerializeField] private TMP_Text xpText;
    [SerializeField] private TMP_Text levelText;

    private void Start()
    {
        if (playerStats == null)
            playerStats = FindObjectOfType<PlayerStats>();

        UpdateUI();

        playerStats.OnLevelUp += OnLevelUp;
        playerStats.OnStatsChanged += UpdateUI;
    }

    private void OnDestroy()
    {
        if (playerStats != null)
        {
            playerStats.OnLevelUp -= OnLevelUp;
            playerStats.OnStatsChanged -= UpdateUI;
        }
    }

    private void Update()
    {
        if (playerStats == null) return;
        UpdateUI();
    }

    private void UpdateUI()
    {
        if (healthFill != null)
            healthFill.fillAmount = Mathf.Clamp01((float)playerStats.currentHealth / playerStats.maxHealth);

        if (xpFill != null)
            xpFill.fillAmount = Mathf.Clamp01((float)playerStats.Experience / playerStats.xpPerLevel);

        if (healthText != null)
            healthText.text = $"{playerStats.currentHealth} / {playerStats.maxHealth}";

        if (xpText != null)
            xpText.text = $"{playerStats.Experience} / {playerStats.xpPerLevel} XP";

        if (levelText != null)
            levelText.text = $"Lvl {playerStats.Level}";
    }

    private void OnLevelUp(int newLevel)
    {
        UpdateUI();
    }
}