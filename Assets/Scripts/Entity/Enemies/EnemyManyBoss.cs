using UnityEngine;

public class EnemyManyBoss : EnemyManyNormal
{
    [Header("Boss")]
    public float enragedHealthPercent = 0.5f;

    public float normalSpeed = 2.5f;
    public float enragedSpeed = 4f;

    public float normalCooldown = 2f;
    public float enragedCooldown = 1f;

    private bool enraged;

    protected override void Update()
    {
        if (!enraged)
        {
            float hp = health.CurrentHealth / health.MaxHealth;

            if (hp <= enragedHealthPercent)
            {
                EnterEnragedMode();
            }
        }

        base.Update();
    }

    void EnterEnragedMode()
    {
        enraged = true;

        walkSpeed = enragedSpeed;
        attackCooldown = enragedCooldown;

        animator.SetTrigger("Enrage");

        Debug.Log("Boss Enraged!");
    }
}