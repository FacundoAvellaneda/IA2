using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

//Facundo Avellaneda
public class EnemyManager : MonoBehaviour
{
    [Header("Enemy Settings")]
    public Enemy enemyPrefab;
    public Enemy bossPrefab;
    public Transform[] spawnPoints;

    [Header("Spawn Settings")]
    public int maxEnemies = 10;
    public int initialActive = 5;
    public float spawnInterval = 2f;

    [Header("Boss Settings")]
    [Range(0f, 1f)]
    public float bossSpawnThreshold = 0.2f;
    private bool bossSpawned = false;
    private bool canSpawnEnemies = true;

    private List<Enemy> enemyPool = new List<Enemy>();
    private Coroutine spawnRoutine;
    private Enemy bossInstance;
    private int totalMaxHealthAtStart;
    private AbilitySystem abilitySystem;

    private void Awake()
    {
        abilitySystem = FindObjectOfType<AbilitySystem>();
    }

    private void Start()
    {
        CreateEnemyPool();

        totalMaxHealthAtStart = enemyPool.Sum(e => e.MaxHealth);

        for (int i = 0; i < initialActive; i++)
            ActivateEnemy();

        // TIME SLICING: se inicia una coroutine que reparte la carga de spawn en el tiempo
        spawnRoutine = StartCoroutine(SpawnRoutine());
    }

    private void CreateEnemyPool()
    {
        if (enemyPrefab == null)
        {
            Debug.LogError("Enemy prefab no asignado en EnemyManager.");
            return;
        }

        int index = 0;
        foreach (var modifiers in GenerateEnemyModifiers(maxEnemies))
        {
            Enemy newEnemy = Instantiate(enemyPrefab, transform);
            newEnemy.gameObject.SetActive(false);

            newEnemy.SetMaxHealth(Mathf.RoundToInt(newEnemy.MaxHealth * modifiers.hpMultiplier));
            newEnemy.health = newEnemy.MaxHealth;

            var collider = newEnemy.GetComponentInChildren<CombatCollider>();
            if (collider != null)
            {
                collider.SetDamage(Mathf.RoundToInt(collider.damage * modifiers.damageMultiplier));
            }

            newEnemy.OnDeath += HandleEnemyDeath;
            enemyPool.Add(newEnemy);

            Debug.Log($"Enemigo {index} creado con HP={newEnemy.MaxHealth} y daño={collider?.damage}");
            index++;
        }

        Debug.Log($"{enemyPool.Count} enemigos generados.");
    }

    //Generator que crea modificadores aleatorios para los enemigos
    private IEnumerable<(float hpMultiplier, float damageMultiplier)> GenerateEnemyModifiers(int count)
    {
        for (int i = 0; i < count; i++)
        {
            float hpMult = Random.Range(0.8f, 1.5f);
            float dmgMult = Random.Range(0.9f, 1.4f);

            yield return (hpMult, dmgMult); 
        }
    }

    private IEnumerator SpawnRoutine()
    {
        // TIME SLICING FUNCTION
        // Spawn de enemigos en el pool
        while (true)
        {
            yield return new WaitForSeconds(spawnInterval);

            if (canSpawnEnemies)
                ActivateEnemy();

            CheckBossSpawnFromSummary();
        }
    }

    private void ActivateEnemy()
    {
        var aliveEnemies = GetAliveEnemies();

        if (aliveEnemies.Count >= maxEnemies)
        {
            Debug.Log("Máximo de enemigos activos alcanzado. No se pueden spawnear más enemigos.");
            canSpawnEnemies = false;
            return;
        }

        // LINQ (Group 3: FirstOrDefault)
        // Busca el primer enemigo inactivo en la lista del pool
        Enemy inactiveEnemy = enemyPool.FirstOrDefault(e => !e.gameObject.activeInHierarchy);
        if (inactiveEnemy == null) return;
        if (spawnPoints == null || spawnPoints.Length == 0) return;

        Vector3 spawnPos = spawnPoints[Random.Range(0, spawnPoints.Length)].position;

        inactiveEnemy.transform.position = spawnPos;
        inactiveEnemy.ResetEnemy();
        inactiveEnemy.gameObject.SetActive(true);
    }

    private void HandleEnemyDeath(Enemy e)
    {
        e.gameObject.SetActive(false);
    }

    private void CheckBossSpawnFromSummary()
    {
        if (bossSpawned || bossPrefab == null)
            return;

        // Uso de TUPLA
        // Utilizacion de una tupla obtener los enemigos vivos y verificaar su salud total
        var summary = GetEnemySummary();
        if (summary.TotalEnemies == 0)
            return;

        float ratio = (float)summary.TotalHealth / totalMaxHealthAtStart;

        Debug.Log($"{ratio}");
        if (ratio <= bossSpawnThreshold)
        {
            SpawnBoss();
        }
    }

    private void SpawnBoss()
    {
        if (spawnPoints == null || spawnPoints.Length == 0)
        {
            return;
        }

        canSpawnEnemies = false;
        bossSpawned = true;

        Vector3 spawnPos = spawnPoints[Random.Range(0, spawnPoints.Length)].position;
        bossInstance = Instantiate(bossPrefab, spawnPos, Quaternion.identity);
        bossInstance.OnDeath += HandleBossDeath;
    }

    private void HandleBossDeath(Enemy boss)
    {
        bossSpawned = false;
        canSpawnEnemies = true;

        StartCoroutine(abilitySystem.ShowUpgrades());
        StartCoroutine(AdjustEnemyDamageOverTime());

        for (int i = 0; i < initialActive; i++)
            ActivateEnemy();
    }

    public List<Enemy> GetAliveEnemies()
    {
        // LINQ (Group 1: Where y ToList)
        // Filtra los enemigos vivos
        return enemyPool
            .Where(e => e != null && e.gameObject.activeInHierarchy && e.CurrentHealth > 0)
            .ToList();
    }

    public (string Names, int TotalHealth, int MaxHealth, int TotalEnemies) GetEnemySummary()
    {
        var alive = GetAliveEnemies();

        if (alive.Count == 0)
            return ("No enemies", 0, 0, 0);

        //TIPO ANONIMO
        // Agrega la informacion de los enemigos vivos
        var summary = alive.Aggregate(
            new { Names = "", TotalHealth = 0, MaxHealth = 0 },
            (acc, en) => new
            {
                Names = acc.Names + en.entityName + ", ",
                TotalHealth = acc.TotalHealth + en.CurrentHealth,
                MaxHealth = acc.MaxHealth + en.MaxHealth
            });

        // TUPPLA con informacion resumida de los enemigos 
        return (summary.Names, summary.TotalHealth, summary.MaxHealth, alive.Count);
    }

    private IEnumerator AdjustEnemyDamageOverTime()
    {
        // Ordenar los enemigos vivos segun su salud actual (de menor a mayor)
        var orderedEnemies = GetAliveEnemies()
            .OrderBy(e => e.CurrentHealth) // Grupo 2 (OrderBy)
            .ToList();                      // Grupo 3 (ToList)

        foreach (var enemy in orderedEnemies)
        {
            var combatCollider = enemy.GetComponentInChildren<CombatCollider>();
            if (combatCollider == null)
            {
                continue;
            }

            float healthRatio = (float)enemy.CurrentHealth / enemy.MaxHealth;

            int adjustedDamage = Mathf.RoundToInt(Mathf.Lerp(10, 40, healthRatio));

            combatCollider.SetDamage(adjustedDamage);

            yield return null;
        }
    }
}