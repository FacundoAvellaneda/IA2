using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class Enemy : Entity
{
    public int xpGiven = 20;
    public bool isBoss = false;
    public int health = 20;

    private EnemyBehavior behavior;

    public event Action<Enemy> OnDeath;

    private void Awake()
    {
        behavior = GetComponent<EnemyBehavior>();
        maxHealth = health;
        currentHealth = maxHealth;
    }

    protected override void Die()
    {
        base.Die();

        if (behavior != null)
            behavior.enabled = false;

        OnDeath?.Invoke(this);

        CombatManager.Instance?.OnEnemyKilled(this);
    }

    public void ResetEnemy()
    {
        currentHealth = MaxHealth;
        if (behavior != null)
            behavior.enabled = true;
    }

}