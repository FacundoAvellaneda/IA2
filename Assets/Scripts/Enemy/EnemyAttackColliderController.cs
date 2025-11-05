using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAttackColliderController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Transform player;   
    [SerializeField] private Transform enemy;    
    [SerializeField] private float distanceFromEnemy = 1f; 

    private void Awake()
    {
        if (enemy == null)
            enemy = transform.root; 

        if (player == null)
        {
            GameObject p = GameObject.FindGameObjectWithTag("Player");
            if (p != null)
                player = p.transform;
        }
    }

    public void AlignInstantly()
    {
        if (enemy == null || player == null) return;

        Vector2 direction = (player.position - enemy.position).normalized;

        Vector2 newPos = (Vector2)enemy.position + direction * distanceFromEnemy;
        transform.position = newPos;

        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0, 0, angle);
    }
}
