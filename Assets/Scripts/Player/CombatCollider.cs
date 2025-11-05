using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class CombatCollider : MonoBehaviour
{
    public int damage = 25;
    Collider2D col;
    void Awake()
    {
        col = GetComponent<Collider2D>();
        col.isTrigger = true;
        gameObject.SetActive(false);
    }


    public void EnableForFrames(int frames = 2)
    {
        StopAllCoroutines();
        StartCoroutine(EnableCoroutine(frames));
    }


    System.Collections.IEnumerator EnableCoroutine(int frames)
    {
        for (int i = 0; i < frames; i++)
            yield return null;
    }


    void OnTriggerEnter2D(Collider2D other)
    {
        var dmg = other.GetComponent<Idamageable>();
        if (dmg != null)
        {
            dmg.TakeDamage(damage);
        }
    }

    public void EndAttack()
    {
        gameObject.SetActive(false);
    }

    public void SetDamage(int dmg)
    {
        damage = dmg;
    }
}
