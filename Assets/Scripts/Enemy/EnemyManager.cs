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
    public int initialActive = 3;       
    public float spawnInterval = 2f;   

    [Header("Boss Settings")]
    [Range(0f, 1f)]
    public float bossSpawnThreshold = 0.2f; 
    private bool bossSpawned = false;

    private List<Enemy> enemyPool = new List<Enemy>();
    private Coroutine spawnRoutine;

    private void Start()
    {
        CreateEnemyPool();

        // Activar los primeros enemigos
        for (int i = 0; i < initialActive; i++)
            ActivateEnemy();

        spawnRoutine = StartCoroutine(ContinuousSpawnRoutine());
    }

    private void CreateEnemyPool()
    {
        if (enemyPrefab == null)
        {
            Debug.LogError(" Enemy prefab no asignado en EnemyManager.");
            return;
        }

        // Instanciamos todos los enemigos una sola vez
        for (int i = 0; i < maxEnemies; i++)
        {
            Enemy newEnemy = Instantiate(enemyPrefab, transform);
            newEnemy.gameObject.SetActive(false);
            newEnemy.OnDeath += HandleEnemyDeath;
            enemyPool.Add(newEnemy);
        }
    }

    private IEnumerator ContinuousSpawnRoutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(spawnInterval);

            int activeCount = enemyPool.Count(e => e.gameObject.activeInHierarchy);
            if (activeCount < maxEnemies)
                ActivateEnemy();

            CheckForBossSpawn();
        }
    }

    private void ActivateEnemy()
    {
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

    public List<Enemy> GetAliveEnemies()
    {
        return enemyPool
            .Where(e => e != null && e.gameObject.activeInHierarchy && e.CurrentHealth > 0)
            .ToList();
    }

    private void CheckForBossSpawn()
    {
        if (bossSpawned || bossPrefab == null)
            return;

        var alive = GetAliveEnemies();
        if (alive.Count == 0)
            return;

        int totalHP = alive.Sum(e => e.CurrentHealth);
        int maxHP = alive.Sum(e => e.MaxHealth);

        float ratio = (float)totalHP / maxHP;

        if (ratio <= bossSpawnThreshold)
        {
            SpawnBoss();
            bossSpawned = true;
        }
    }

    private void SpawnBoss()
    {
        if (spawnPoints == null || spawnPoints.Length == 0)
        {
            Debug.LogWarning(" No hay puntos de spawn para el boss.");
            return;
        }

        Vector3 spawnPos = spawnPoints[Random.Range(0, spawnPoints.Length)].position;
        Instantiate(bossPrefab, spawnPos, Quaternion.identity);
        Debug.Log(" ¡Boss Spawned!");
    }

    public string GetEnemySummary()
    {
        var alive = GetAliveEnemies();
        if (alive.Count == 0) return "No enemies.";

        var summary = alive.Aggregate(
            new { Names = "", TotalHealth = 0 },
            (acc, en) => new
            {
                Names = acc.Names + en.entityName + ", ",
                TotalHealth = acc.TotalHealth + en.CurrentHealth
            });

        return $"Enemies: {summary.Names} TotalHP: {summary.TotalHealth}";
    }
}