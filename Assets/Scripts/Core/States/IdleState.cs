using UnityEngine;

public class IdleState : BaseState
{
    public IdleState(FighterEntity pm) : base(pm) { }

    public override void EnterState()
    {
        Debug.Log("Entered Idle State");
        fighter.animator?.SetFloat("Speed", 0f);
    }

    public override void UpdateState()
    {
        Vector3 moveDir = fighter.GetMovementInput();

        if (moveDir.sqrMagnitude > 0.01f)
        {
            fighter.ChangeState(fighter.WalkState);
            return;
        }

        // Verificar si queremos atacar
        if (fighter.GetAttackInput())
        {
            fighter.ResetCombo();

            if (fighter.activeCombo != null && fighter.activeCombo.attacks.Count > 0)
            {
                fighter.currentAttack = fighter.activeCombo.attacks[0];
                fighter.ChangeState(fighter.AttackState);
            }
            return;
        }

        if (!fighter.controller.isGrounded && fighter.currentState != fighter.AttackState && fighter.currentState != fighter.HitState && fighter.currentState != fighter.DeathState)
        {
            if (fighter.currentState != fighter.AirborneState)
            {
                fighter.ChangeState(fighter.AirborneState);
            }
        }
    }

    public override void FixedUpdateState()
    {

    }

    public override void ExitState()
    {

    }
}
