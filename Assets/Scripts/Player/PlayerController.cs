using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerController : Entity
{
    [Header("Movement")]
    public float moveSpeed = 5f;
    private Rigidbody2D rb;
    private Vector2 input;
    private PlayerStats stats;

    [Header("Attack Settings")]
    [SerializeField] private CombatCollider combatCollider;
    [SerializeField] private CombatColliderMouseFollower colliderFollower;
    [SerializeField] private float attackCooldown = 0.6f;
    [SerializeField] private int attackFrames = 4;

    private bool canAttack = true;

    public enum PlayerState { Idle, Moving, Attacking }
    public PlayerState CurrentState { get; private set; } = PlayerState.Idle;

    public delegate void StateChanged(PlayerState newState);
    public event StateChanged OnStateChanged;

    protected override void Awake()
    {
        base.Awake();
        rb = GetComponent<Rigidbody2D>();
        stats = GetComponent<PlayerStats>();
    
        if (stats != null)
        {
            maxHealth = stats.maxHealth;
            stats.currentHealth = maxHealth;
        }
    }

    void Update()
    {
        if (CurrentState != PlayerState.Attacking)
            ReadInput();

        UpdateState();
    }

    void FixedUpdate()
    {
        if (CurrentState == PlayerState.Moving)
            Move();
        else if (CurrentState == PlayerState.Idle)
            rb.velocity = Vector2.zero;
    }

    void ReadInput()
    {
        float h = Input.GetAxisRaw("Horizontal");
        float v = Input.GetAxisRaw("Vertical");
        input = new Vector2(h, v).normalized;
    }

    void Move()
    {
        rb.velocity = input * (moveSpeed + stats.speed * 0.2f);
    }

    void UpdateState()
    {
        var prev = CurrentState;

        if (Input.GetKeyDown(KeyCode.Mouse0) && canAttack)
        {
            CurrentState = PlayerState.Attacking;
            combatCollider.gameObject.SetActive(true);
            StartCoroutine(AttackRoutine());
        }
        else if (CurrentState != PlayerState.Attacking)
        {
            if (input.sqrMagnitude > 0.01f)
                CurrentState = PlayerState.Moving;
            else
                CurrentState = PlayerState.Idle;
        }

        if (prev != CurrentState)
            OnStateChanged?.Invoke(CurrentState);
    }

    private IEnumerator AttackRoutine()
    {
        canAttack = false;
        rb.velocity = Vector2.zero;

        colliderFollower?.PrepareForAttack();

        if (combatCollider != null)
        {
            combatCollider.SetDamage(stats.Damage);
            combatCollider.EnableForFrames(attackFrames);
        }

        colliderFollower?.StopFollowing();

        yield return new WaitForSeconds(attackCooldown);
        canAttack = true;
        CurrentState = PlayerState.Idle;
        combatCollider.gameObject.SetActive(false);
    }

    public override void TakeDamage(int amount)
    {
        if (stats == null) return;

        stats.currentHealth -= amount;
        if (stats.currentHealth <= 0)
        {
            stats.currentHealth = 0;
            Die();
        }
    }

    protected override void Die()
    {
        Debug.Log("El jugador ha muerto");
        rb.velocity = Vector2.zero;
        enabled = false;
    }
}