using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class AbilitySystem : MonoBehaviour
{
    [Header("Referencias")]
    [SerializeField] private PlayerStats playerStats;
    [SerializeField] private GameObject upgradePanel;
    [SerializeField] private Transform cardParent;
    [SerializeField] private UpgradeCardUI cardPrefab;

    [Header("Configuración")]
    [SerializeField] private int upgradesToShow = 3;
    [SerializeField] private float revealDelay = 0.3f;

    [SerializeField] private List<AbilityData> allAbilities = new List<AbilityData>();
    private List<AbilityData> acquiredAbilities = new List<AbilityData>();

    private void Start()
    {
        upgradePanel.SetActive(false);
    }

    //TIME SLICING y GENERATOR
    //Genera las cartas de mejora una a una con retraso entre ellas
    public IEnumerator ShowUpgrades()
    {
        Time.timeScale = 0f;
        upgradePanel.SetActive(true);

        foreach (Transform t in cardParent)
            Destroy(t.gameObject);

        //LINQ Grupo 1: Where / Any / Take
        //Filtra las habilidades no adquiridas y selecciona aleatoriamente algunas para mostrar
        var available = allAbilities
            .Where(u => !acquiredAbilities.Any(a => a.id == u.id))
            .OrderBy(u => Random.value)
            .Take(upgradesToShow)
            .ToList();

        foreach (var ability in available)
        {
            var card = Instantiate(cardPrefab, cardParent);
            card.Setup(ability, this);
            yield return new WaitForSecondsRealtime(revealDelay);
        }
    }

    public void SelectUpgrade(AbilityData ability)
    {
        //LINQ Grupo 1: FirstOrDefault
        var existing = acquiredAbilities.FirstOrDefault(u => u.id == ability.id);

        if (existing != null)
        {
            foreach (var effect in ability.effects)
            {
                var match = existing.effects.Find(e => e.stat == effect.stat);
                if (match.stat != 0)
                {
                    match.amount += effect.amount;
                }
                else
                {
                    existing.effects.Add(effect);
                }
            }
            ApplyAbilityEffect(existing);
        }
        else
        {
            acquiredAbilities.Add(ability);
            ApplyAbilityEffect(ability);
        }

        ClosePanel();
    }

    private void ApplyAbilityEffect(AbilityData ability)
    {
        foreach (var effect in ability.effects)
        {
            switch (effect.stat)
            {
                case StatType.Strength:
                    playerStats.strength += effect.amount;
                    break;
                case StatType.Speed:
                    playerStats.speed += effect.amount;
                    break;
                case StatType.MaxHealth:
                    playerStats.maxHealth += effect.amount;
                    playerStats.currentHealth = playerStats.maxHealth;
                    break;
            }
        }

        playerStats.NotifyStatsChanged();
    }

    // LINQ Grupo 1: Select
    // Crea una descripción legible de los efectos de la habilidad
    private string DescribeAbility(AbilityData ability)
    {
        return string.Join(", ", ability.effects.Select(e => $"+{e.amount} {e.stat}"));
    }

    private void ClosePanel()
    {
        upgradePanel.SetActive(false);
        Time.timeScale = 1f;
    }
}