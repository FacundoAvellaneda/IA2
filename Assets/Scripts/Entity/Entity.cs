using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Entity : MonoBehaviour, Idamageable
{
    [Header("Base Stats")]
    public string entityName = "Entity";
    protected int maxHealth = 100;

    protected int currentHealth;

    public int CurrentHealth => currentHealth;
    public int MaxHealth => maxHealth;

    protected virtual void Awake()
    {
        currentHealth = maxHealth;
    }


    public virtual void TakeDamage(int amount)
    {
        Debug.Log($"{entityName} took {amount} damage.");
        Debug.Log($"Health before damage: {currentHealth}");
        currentHealth -= amount;
        if (currentHealth <= 0) Die();
    }


    protected virtual void Die()
    {
        gameObject.SetActive(false);
    }

    public int GetCurrentHealth() => currentHealth;
    public int GetMaxHealth() => maxHealth;

    public void SetMaxHealth(int value)
    {
        maxHealth = value;
        currentHealth = Mathf.Min(currentHealth, maxHealth);
    }
}
