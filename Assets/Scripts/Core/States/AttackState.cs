using System;
using UnityEngine;

public class AttackState : BaseState
{
    public AttackState(FighterEntity fighter) : base(fighter) { }

    public override void EnterState()
    {
       Debug.Log("Entered Attack State");
        fighter.verticalVelocity = 0f;
        fighter.velocity = Vector3.zero;
        fighter.ConsumeAttackInput();

        PlayAttackAnimation();
    }

    public override void UpdateState()
    {   
        if (fighter.currentAttack == null)
            return;

        AnimatorStateInfo info = fighter.animator.GetCurrentAnimatorStateInfo(0);

        fighter.velocity = Vector3.zero;

        // --- VENTANA DE CANCELACIÓN ---
        if (info.normalizedTime > 0.6f)
        {
            if (fighter.GetAttackInput())
            {
                Debug.Log("Input detected for next attack in combo!");
                AdvanceCombo();
            }
        }

        if (info.normalizedTime >= 0.92f)
        {
            fighter.ChangeState(fighter.IdleState);
        }
    }

    public void AdvanceCombo()
    {
        fighter.comboIndex++;
        if (fighter.comboIndex >= fighter.activeCombo.attacks.Count)
        {
            fighter.comboIndex = 0;
        }

        fighter.currentAttack = fighter.activeCombo.attacks[fighter.comboIndex];
        fighter.ConsumeAttackInput();

        PlayAttackAnimation();
    }

    private void PlayAttackAnimation()
    {
        AttackBase attack = fighter.currentAttack;

        if (attack == null)
        {
            fighter.ChangeState(fighter.IdleState);
            return;
        }

        if (attack.hitSound != null && fighter.audioSource != null)
        {
            fighter.audioSource.PlayOneShot(attack.hitSound);
        }

        fighter.animator.CrossFade(
            attack.animationStateName,
            0.05f
        );

        fighter.AnimEvent_CloseHitbox(0);
        fighter.AnimEvent_CloseHitbox(1);
        fighter.AnimEvent_CloseHitbox(2);
        fighter.AnimEvent_CloseHitbox(3);
        fighter.AnimEvent_CloseHitbox(4);
    }

    public override void ExitState()
    {
        fighter.AnimEvent_CloseHitbox(0);
        fighter.AnimEvent_CloseHitbox(1);
        fighter.AnimEvent_CloseHitbox(2);
        fighter.AnimEvent_CloseHitbox(3);
        fighter.AnimEvent_CloseHitbox(4);
    }

    public override void FixedUpdateState()
    {

    }
}