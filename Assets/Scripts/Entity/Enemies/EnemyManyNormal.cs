using UnityEngine;

public class EnemyManyNormal : FighterEntity
{
    [Header("AI & Targeting")]
    private Transform target;
    public float detectionRange = 8f;
    public float attackRange = 2f;

    [Header("Poison Setup")]
    public GameObject poisonZoneArea;

    private bool attackRequested;

    protected override void Start()
    {
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
        {
            target = playerObj.transform;
        }

        if (poisonZoneArea != null)
        {
            poisonZoneArea.SetActive(false);
        }

        if (activeCombo != null && activeCombo.attacks.Count > 0)
        {
            currentAttack = activeCombo.attacks[0];
        }
        base.Start();
    }

    protected override void Update()
    {
        if (target != null && currentState != DeathState)
        {
            float distance = Vector3.Distance(transform.position, target.position);

            // 1. Iniciar Ataque
            if (distance <= attackRange && currentState != AttackState)
            {
                attackRequested = true;
                ChangeState(AttackState);
            }

            // 2. Cancelar Ataque
            if (currentState == AttackState)
            {
                AnimatorStateInfo info = animator.GetCurrentAnimatorStateInfo(0);

                if (distance > attackRange && info.normalizedTime < 0.5f)
                {
                    ConsumeAttackInput();
                    animator.CrossFade("Idle", 0.05f);
                    ChangeState(IdleState);
                }
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

    public override bool GetAttackInput()
    {
        return attackRequested;
    }

    public override void ConsumeAttackInput()
    {
        attackRequested = false;
    }

    public void AnimEvent_Explode()
    {
        Debug.Log("EnemyManyNormal: AnimEvent_Explode");

        if (poisonZoneArea != null)
        {
            poisonZoneArea.transform.SetParent(null);
            poisonZoneArea.SetActive(true);
        }

        InstantDeath();
    }

    private void OnDrawGizmosSelected()
    {
        // Radio de Detección
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectionRange);

        // Radio de Ataque 
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }
}