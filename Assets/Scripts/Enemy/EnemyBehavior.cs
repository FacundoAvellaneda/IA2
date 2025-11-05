using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class EnemyBehavior : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Transform player;
    [SerializeField] private CombatCollider attackCollider; 

    [Header("Movement Settings")]
    [SerializeField] private float moveSpeed = 2f;
    [SerializeField] private float patrolRadius = 5f;
    [SerializeField] private float visionRange = 6f;
    [SerializeField] private float attackRange = 1.5f;
    [SerializeField] private float repathTime = 2f;

    [Header("Combat Settings")]
    [SerializeField] private float attackCooldown = 1.5f;
    [SerializeField] private int attackFrames = 4;

    private Rigidbody2D rb;
    private Vector2 patrolTarget;
    private float repathTimer;
    private bool canAttack = true;

    private enum EnemyState { Patrol, Chase, Attack }
    private EnemyState currentState = EnemyState.Patrol;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();

        if (player == null)
        {
            GameObject p = GameObject.FindGameObjectWithTag("Player");
            if (p != null)
                player = p.transform;
        }
    }

    private void Start()
    {
        ChooseNewPatrolPoint();
    }

    private void Update()
    {
        if (player == null) return;

        float distanceToPlayer = Vector2.Distance(transform.position, player.position);

        switch (currentState)
        {
            case EnemyState.Patrol:
                Patrol();
                if (distanceToPlayer <= visionRange)
                    currentState = EnemyState.Chase;
                break;

            case EnemyState.Chase:
                ChasePlayer(distanceToPlayer);
                if (distanceToPlayer > visionRange * 1.5f)
                    currentState = EnemyState.Patrol;
                break;

            case EnemyState.Attack:
                break;
        }

        if (currentState == EnemyState.Chase || currentState == EnemyState.Attack)
        {
            RotateTowards(player.position);
        }
        else if (currentState == EnemyState.Patrol)
        {
            RotateTowards(patrolTarget);
        }
    }

    private void FixedUpdate()
    {
        if (currentState == EnemyState.Patrol)
        {
            MoveTowards(patrolTarget);
        }
        else if (currentState == EnemyState.Chase && player != null)
        {
            MoveTowards(player.position);
        }
    }

    private void Patrol()
    {
        repathTimer -= Time.deltaTime;
        if (repathTimer <= 0f || Vector2.Distance(transform.position, patrolTarget) < 0.5f)
            ChooseNewPatrolPoint();
    }

    private void ChooseNewPatrolPoint()
    {
        Vector2 randomDirection = Random.insideUnitCircle * patrolRadius;
        patrolTarget = (Vector2)transform.position + randomDirection;
        repathTimer = repathTime;
    }

    private void MoveTowards(Vector2 target)
    {
        Vector2 newPosition = Vector2.MoveTowards(rb.position, target, moveSpeed * Time.fixedDeltaTime);
        rb.MovePosition(newPosition);
    }

    private void RotateTowards(Vector2 target)
    {
        Vector2 direction = (target - (Vector2)transform.position).normalized;
        if (direction.sqrMagnitude > 0.001f)
        {
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            rb.rotation = angle;
        }
    }

    private void ChasePlayer(float distanceToPlayer)
    {
        if (distanceToPlayer <= attackRange && canAttack)
        {
            attackCollider.gameObject.SetActive(true);
            StartCoroutine(AttackRoutine());
        }
    }

    private IEnumerator AttackRoutine()
    {
        currentState = EnemyState.Attack;
        canAttack = false;
        rb.velocity = Vector2.zero;

        if (attackCollider != null)
        {
            attackCollider.EnableForFrames(attackFrames);
        }
                  
        yield return new WaitForSeconds(attackCooldown);
        attackCollider.EndAttack();
        canAttack = true;

        currentState = EnemyState.Chase;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, visionRange);

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);

        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, patrolRadius);
    }
}