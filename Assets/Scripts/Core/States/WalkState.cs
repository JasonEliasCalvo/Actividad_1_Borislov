using UnityEngine;

public class WalkState : BaseState
{
    public WalkState(FighterEntity fighter) : base(fighter) { }

    public override void EnterState()
    {
        Debug.Log("Entered Walk State");
    }

    public override void FixedUpdateState()
    {

    }

    public override void UpdateState()
    {
        Vector3 moveDir = fighter.GetMovementInput();

        fighter.MoveEntity(moveDir, fighter.walkSpeed);
        fighter.RotateEntity(moveDir);

        fighter.animator.SetFloat("Speed", moveDir.magnitude);

        if (moveDir.sqrMagnitude < 0.05f)
            fighter.ChangeState(fighter.IdleState);

        if (!fighter.controller.isGrounded && fighter.currentState != fighter.AttackState && fighter.currentState != fighter.HitState && fighter.currentState != fighter.DeathState)
        {
            if (fighter.currentState != fighter.AirborneState)
            {
                fighter.ChangeState(fighter.AirborneState);
            }
        }

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
    }

    public override void ExitState()
    {

    }
}
