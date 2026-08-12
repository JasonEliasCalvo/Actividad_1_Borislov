using UnityEngine;

public class EnemyManyBoss : FighterEntity
{
    [Header("AI Parameters")]
    private Transform target;
    public float detectionRange = 12f;
    public float attackRange = 2.5f;
    public float attackCooldown = 2.5f;

    [Header("Mecha Ejection Settings")]
    public FighterEntity manyEntity;      
    public GameObject ejectParticle; 

    private bool attackRequested;
    private float cooldownTimer;

    protected override void Start()
    {
        base.Start();

        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
        {
            target = playerObj.transform;
        }

        if (activeCombo != null && activeCombo.attacks.Count > 0)
        {
            currentAttack = activeCombo.attacks[comboIndex];
        }

        health.OnDeath += HandleMechaDestruction;
    }

    protected override void Update()
    {
        cooldownTimer -= Time.deltaTime;

        if (target != null && currentState != DeathState)
        {
            float distance = Vector3.Distance(transform.position, target.position);

            if (distance <= attackRange && cooldownTimer <= 0f && currentState != AttackState)
            {
                attackRequested = true;
                cooldownTimer = attackCooldown;
                ChangeState(AttackState);
            }
        }

        base.Update();
    }

    public override Vector3 GetMovementInput()
    {
        if (target == null)
            return Vector3.zero;

        float distance = Vector3.Distance(transform.position, target.position);

        if (distance > detectionRange || distance <= attackRange)
            return Vector3.zero;

        Vector3 dir = target.position - transform.position;
        dir.y = 0;

        return dir.normalized;
    }

    public override void TakeDamage(float amount, float hitStun)
    {
        base.TakeDamage(amount, hitStun);

        if (health.CurrentHealth > 0 && manyEntity != null)
        {
            manyEntity.HitState.stunDuration = hitStun;
            manyEntity.ChangeState(manyEntity.HitState);
        }
    }

    public override bool GetAttackInput()
    {
        return attackRequested;
    }

    public override void ConsumeAttackInput()
    {
        attackRequested = false;
    }

    
    private void HandleMechaDestruction()
    {
        if (ejectParticle != null)
        {
            Instantiate(ejectParticle, transform.position, Quaternion.identity);
        }

        if (manyEntity != null)
        {
            manyEntity.transform.SetParent(null);

            if (manyEntity.TryGetComponent<HealthComponent>(out var manyHealth))
            {
                float damageNeeded = manyHealth.MaxHealth - 1f;
                if (damageNeeded > 0)
                {
                    manyHealth.ApplyDamage(damageNeeded);
                }
            }
        }
    }

    private void OnDestroy()
    {
        if (health != null)
        {
            health.OnDeath -= HandleMechaDestruction;
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectionRange);

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }
}