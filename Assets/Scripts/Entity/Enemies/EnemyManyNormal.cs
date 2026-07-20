using UnityEngine;

public class EnemyManyNormal : FighterEntity
{
    [Header("AI")]
    public Transform target;

    public float detectionRange = 8f;
    public float attackRange = 1.8f;
    public float attackCooldown = 2f;

    private bool attackRequested;
    private float cooldownTimer;

    protected override void Update()
    {
        cooldownTimer -= Time.deltaTime;

        if (target != null)
        {
            float distance = Vector3.Distance(transform.position, target.position);

            if (distance <= attackRange && cooldownTimer <= 0f)
            {
                attackRequested = true;
                cooldownTimer = attackCooldown;
            }
        }

        base.Update();
    }

    public override Vector3 GetMovementInput()
    {
        if (target == null)
            return Vector3.zero;

        float distance = Vector3.Distance(transform.position, target.position);

        if (distance > detectionRange)
            return Vector3.zero;

        if (distance <= attackRange)
            return Vector3.zero;

        Vector3 dir = target.position - transform.position;
        dir.y = 0;

        return dir.normalized;
    }

    public override bool GetAttackInput()
    {
        return attackRequested;
    }

    public override void ConsumeAttackInput()
    {
        attackRequested = false;
    }
}