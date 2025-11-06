using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

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

    private void Start()
    {
        CreateEnemyPool();

        totalMaxHealthAtStart = enemyPool.Sum(e => e.MaxHealth);

        for (int i = 0; i < initialActive; i++)
            ActivateEnemy();

        spawnRoutine = StartCoroutine(SpawnRoutine());
    }

    private void CreateEnemyPool()
    {
        if (enemyPrefab == null)
        {
            Debug.LogError(" Enemy prefab no asignado en EnemyManager.");
            return;
        }

        for (int i = 0; i < maxEnemies; i++)
        {
            Enemy newEnemy = Instantiate(enemyPrefab, transform);
            newEnemy.gameObject.SetActive(false);
            newEnemy.OnDeath += HandleEnemyDeath;
            enemyPool.Add(newEnemy);
        }
    }

    private IEnumerator SpawnRoutine()
    {
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
            Debug.Log(" Máximo de enemigos activos alcanzado. No se pueden spawnear más enemigos.");
            canSpawnEnemies = false;
            return;
        }

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

        var summary = GetEnemySummary();
        if (summary.TotalEnemies == 0)
            return;

        float ratio = (float)summary.TotalHealth / totalMaxHealthAtStart;

        Debug.Log($"{ratio}");
        if (ratio <= bossSpawnThreshold)
        {
            Debug.Log(" Condiciones cumplidas para spawn del boss.");
            SpawnBoss();
        }
    }

    private void SpawnBoss()
    {
        if (spawnPoints == null || spawnPoints.Length == 0)
        {
            Debug.LogWarning(" No hay puntos de spawn para el boss.");
            return;
        }

        canSpawnEnemies = false;
        bossSpawned = true;

        Vector3 spawnPos = spawnPoints[Random.Range(0, spawnPoints.Length)].position;
        bossInstance = Instantiate(bossPrefab, spawnPos, Quaternion.identity);
        bossInstance.OnDeath += HandleBossDeath;

        Debug.Log(" ¡Boss Spawned!");
    }

    private void HandleBossDeath(Enemy boss)
    {
        Debug.Log(" Boss derrotado. Reactivando spawner...");

        bossSpawned = false;
        canSpawnEnemies = true;

        for (int i = 0; i < initialActive; i++)
            ActivateEnemy();
    }

    public List<Enemy> GetAliveEnemies()
    {
        return enemyPool
            .Where(e => e != null && e.gameObject.activeInHierarchy && e.CurrentHealth > 0)
            .ToList();
    }

    public (string Names, int TotalHealth, int MaxHealth, int TotalEnemies) GetEnemySummary()
    {
        var alive = GetAliveEnemies();

        if (alive.Count == 0)
            return ("No enemies", 0, 0, 0);

        var summary = alive.Aggregate(
            new { Names = "", TotalHealth = 0, MaxHealth = 0 },
            (acc, en) => new
            {
                Names = acc.Names + en.entityName + ", ",
                TotalHealth = acc.TotalHealth + en.CurrentHealth,
                MaxHealth = acc.MaxHealth + en.MaxHealth
            });

        return (summary.Names, summary.TotalHealth, summary.MaxHealth, alive.Count);
    }
}