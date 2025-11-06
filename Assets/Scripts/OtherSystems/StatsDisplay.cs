using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class StatsDisplay : MonoBehaviour
{
    [SerializeField] private TMP_Text statNameText;
    [SerializeField] private TMP_Text statValueText;
    [SerializeField] private Button addButton;

    private string statKey;
    private System.Action<string> onAddPressed;

    public void Initialize(string displayName, string statKey, int value, System.Action<string> onAddPressed)
    {
        this.statKey = statKey;
        this.onAddPressed = onAddPressed;

        statNameText.text = displayName;
        statValueText.text = value.ToString();
        addButton.onClick.AddListener(() => this.onAddPressed?.Invoke(statKey));
    }

    public void UpdateValue(int newValue)
    {
        statValueText.text = newValue.ToString();
    }

    public void SetButtonInteractable(bool canSpend)
    {
        addButton.interactable = canSpend;
    }
}