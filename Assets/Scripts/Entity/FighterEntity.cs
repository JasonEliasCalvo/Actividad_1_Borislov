using System;
using UnityEngine;

// Requerimos estos componentes obligatoriamente
[RequireComponent(typeof(CharacterController))]
[RequireComponent(typeof(Animator))]
public abstract class FighterEntity : MonoBehaviour, IDamageable
{
    [Header("Core Components")]
    public Animator animator;
    protected HealthComponent health;
    public CharacterController controller;

    [Header("Movement Stats")]
    public float walkSpeed = 5f;
    public float acceleration = 15f;
    public float deceleration = 20f;
    public float gravity = -9.81f;
    public float rotationSmoothTime = 0.12f;

    // Físicas
    [HideInInspector] public Vector3 velocity;
    [HideInInspector] public float verticalVelocity;
    protected float rotationVelocity;

    // Estado Actual
    protected BaseState currentState;

    // --- ESTADOS (Instancias) ---
    public IdleState IdleState;
    public WalkState WalkState;
    public AirborneState AirborneState;
    public AttackState AttackState;
    public HitState HitState;
    public DeathState DeathState;

    // --- COMBAT REFERENCES ---
    [Header("Combat System")]
    public CombatHitbox rightHandBox; // 0
    public CombatHitbox leftHandBox; // 1
    public CombatHitbox rightFootBox; // 2
    public CombatHitbox leftFootBox; // 3
    public CombatHitbox weaponBox; // 4

    [Header("Combo Settings")]
    public ComboSequence activeCombo;
    [HideInInspector] public int comboIndex = 0;

    public virtual void ConsumeAttackInput() { }

    public bool IsStunned { get; private set; }
    public bool IsVulnerable { get; private set; }
    public bool IsInvulnerable { get; set; }

    // El ataque que se está ejecutando
    public AttackBase currentAttack;

    protected virtual void Awake()
    {
        controller = GetComponent<CharacterController>();
        health = GetComponent<HealthComponent>();
        animator = GetComponent<Animator>();

        // Inicializamos estados pasando "this" (la entidad)
        IdleState = new IdleState(this);
        WalkState = new WalkState(this);
        AirborneState = new AirborneState(this);

        AttackState = new AttackState(this);
        HitState = new HitState(this);
        DeathState = new DeathState(this);

        // SUSCRIPCIONES IMPORTANTES
        health.OnDeath += HandleDeath;
    }

    protected virtual void Start()
    {
        ChangeState(IdleState);
    }

    protected virtual void Update()
    {
        if (currentState == DeathState) return;

        currentState?.UpdateState();
        ApplyGravity();

        Vector3 finalMove = velocity + Vector3.up * verticalVelocity;
        controller.Move(finalMove * Time.deltaTime);
    }

    protected virtual void FixedUpdate()
    {
        currentState?.FixedUpdateState();
    }

    // --- SISTEMA DE DAÑO ---
    public virtual void TakeDamage(float amount, float hitStun)
    {
        if (IsInvulnerable || currentState == DeathState) return;

        health.ApplyDamage(amount);
        Debug.Log($"{gameObject.name} recibió {amount} de daño. Vida: {health.CurrentHealth}");

        if (health.CurrentHealth > 0)
        {
            if (currentState == HitState)
            {
                // Si ya estamos golpeados, llamamos al método especial de refresco
                HitState.RefreshHit(hitStun);
            }
            else
            {
                // Si es el primer golpe, entramos al estado normalmente
                HitState.stunDuration = hitStun;
                ChangeState(HitState);
            }
        }
    }

    public void Heal(float amount)
    {
        health.Heal(amount);
    }

    private void HandleDeath()
    {
        if (currentState == DeathState) return;

        ChangeState(DeathState);
        controller.enabled = false;
    }

    // --- FÍSICAS COMPARTIDAS ---
    public void MoveEntity(Vector3 direction, float speed)
    {
        if (direction.sqrMagnitude > 0.01f)
        {
            // Rotación
            float targetAngle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg;
            float angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref rotationVelocity, rotationSmoothTime);
            transform.rotation = Quaternion.Euler(0f, angle, 0f);

            // Velocidad
            velocity = direction * speed;
        }
        else
        {
            velocity = Vector3.zero;
        }

        // Animación
        animator.SetFloat("Speed", velocity.magnitude);
    }

    public void RotateEntity(Vector3 direction)
    {
        if (direction.sqrMagnitude < 0.01f) return;

        float targetAngle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg;
        float angle = Mathf.SmoothDampAngle(
            transform.eulerAngles.y,
            targetAngle,
            ref rotationVelocity,
            rotationSmoothTime
        );

        transform.rotation = Quaternion.Euler(0f, angle, 0f);
    }

    private void ApplyGravity()
    {
        if (controller.isGrounded && verticalVelocity < 0) verticalVelocity = -2f;
        else verticalVelocity += gravity * Time.deltaTime;
    }

    // --- GESTIÓN DE ESTADOS ---
    public void ChangeState(BaseState newState)
    {
        if (currentState == newState) return;
        currentState?.ExitState();
        currentState = newState;
        currentState.EnterState();
    }

    public void ResetState(BaseState newState)
    {
        currentState?.ExitState();
        currentState = newState;
        currentState.EnterState();
    }

    public void ResetCombo()
    {
        comboIndex = 0;
    }

    // --- HITBOX MANAGEMENT ---
    public void AnimEvent_OpenHitbox(int limbIndex)
    {
        if (currentAttack == null) return;

        // Extraemos datos de tu ScriptableObject
        float dmg = currentAttack.damage;
        float stun = currentAttack.hitStun;
        float knock = currentAttack.knockbackForce;

        switch (limbIndex)
        {
            case 0: rightHandBox?.EnableHitbox(dmg, stun, knock); break;
            case 1: leftHandBox?.EnableHitbox(dmg, stun, knock); break;
            case 2: rightFootBox?.EnableHitbox(dmg, stun, knock); break;
            case 3: leftFootBox?.EnableHitbox(dmg, stun, knock); break;
        }
    }

    public void AnimEvent_CloseHitbox(int limbIndex)
    {
        switch (limbIndex)
        {
            case 0: rightHandBox?.DisableHitbox(); break;
            case 1: leftHandBox?.DisableHitbox(); break;
            case 2: rightFootBox?.DisableHitbox(); break;
            case 3: leftFootBox?.DisableHitbox(); break;
        }
    }

    // --- MÉTODOS ABSTRACTOS ---
    public abstract Vector3 GetMovementInput();
    public abstract bool GetAttackInput();
}
