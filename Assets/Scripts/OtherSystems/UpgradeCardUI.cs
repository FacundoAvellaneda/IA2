using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UpgradeCardUI : MonoBehaviour
{
    [SerializeField] private TMP_Text titleText;
    [SerializeField] private TMP_Text descriptionText;
    [SerializeField] private Image iconImage;
    [SerializeField] private Button selectButton;

    private AbilityData data;
    private AbilitySystem system;

    public void Setup(AbilityData upgrade, AbilitySystem sys)
    {
        data = upgrade;
        system = sys;

        titleText.text = upgrade.abilityName;
        descriptionText.text = upgrade.description;
        if (iconImage != null && upgrade.icon != null)
            iconImage.sprite = upgrade.icon;

        selectButton.onClick.RemoveAllListeners();
        selectButton.onClick.AddListener(() => system.SelectUpgrade(data));
    }
}