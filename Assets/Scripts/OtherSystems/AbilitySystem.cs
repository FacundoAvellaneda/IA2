using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class AbilitySystem : MonoBehaviour
{
    [Header("Referencias")]
    [SerializeField] private PlayerStats playerStats;
    [SerializeField] private GameObject abilityUIPanel;

    [Header("Configuración")]
    [SerializeField] private int abilityChoices = 3;
    [SerializeField] private float revealDelay = 0.5f;

    private List<AbilityData> allAbilities = new List<AbilityData>();
    private List<AbilityData> currentChoices = new List<AbilityData>();

    private void Start()
    {
        allAbilities = GenerateAbilities().ToList();
    }

    private IEnumerable<AbilityData> GenerateAbilities()
    {
        yield return new AbilityData("Fuerza del Dragón", "Aumenta daño", 10, 0, 0, "Incrementa el daño base en 10 puntos.");
        yield return new AbilityData("Reflejo Áureo", "Aumenta velocidad", 0, 2, 0, "Aumenta la velocidad en +2.");
        yield return new AbilityData("Escama Ancestral", "Aumenta vida", 0, 0, 20, "Aumenta la vida máxima en 20 puntos.");
        yield return new AbilityData("Llama Carmesí", "Ataque especial", 25, 0, 0, "Permite lanzar un ataque ardiente con +25 daño.");
        yield return new AbilityData("Vínculo Dorado", "Defensa", 0, 0, 10, "Aumenta la regeneración de vida.");
    }

    public IEnumerator ShowRandomAbilities()
    {
        abilityUIPanel.SetActive(true);
        currentChoices.Clear();

        // Group1: OrderBy + Take
        var selected = allAbilities
            .OrderBy(a => Random.value)  
            .Take(abilityChoices)       
            .ToList();

        // Group2: Aggregate -> sumar atributos totales como curiosidad
        int totalBuff = selected.Aggregate(0, (acc, ab) => acc + ab.damageBonus + ab.healthBonus + ab.speedBonus);
        Debug.Log($"?? Habilidades generadas. Buff total combinado = {totalBuff}");

        // Group3: Skip/Take (time slicing para efecto de aparición progresiva)
        for (int i = 0; i < selected.Count; i++)
        {
            var ability = selected.Skip(i).Take(1).First();
            currentChoices.Add(ability);
            Debug.Log($"?? Nueva habilidad disponible: {ability.name} ({ability.description})");

            yield return new WaitForSeconds(revealDelay);
        }
    }

    // =========================================================
    // ?? Tupla que devuelve información de la habilidad elegida
    // =========================================================
    public (string name, string desc, int dmg, int spd, int hp) SelectAbility(int index)
    {
        if (index < 0 || index >= currentChoices.Count)
        {
            Debug.LogWarning("? Índice de habilidad inválido.");
            return ("", "", 0, 0, 0);
        }

        var chosen = currentChoices[index];
        ApplyAbilityToPlayer(chosen);

        return (chosen.name, chosen.description, chosen.damageBonus, chosen.speedBonus, chosen.healthBonus);
    }

    private void ApplyAbilityToPlayer(AbilityData ability)
    {
        if (playerStats == null) return;

        playerStats.strength += ability.damageBonus / 2;
        playerStats.speed += ability.speedBonus;
        playerStats.maxHealth += ability.healthBonus;
        playerStats.currentHealth = Mathf.Min(playerStats.currentHealth + ability.healthBonus, playerStats.maxHealth);

        Debug.Log($" Habilidad adquirida: {ability.name} — {ability.description}");

        playerStats.NotifyStatsChanged();

        abilityUIPanel?.SetActive(false);
    }
}

