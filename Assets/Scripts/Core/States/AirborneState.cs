using UnityEngine;

public class AirborneState : BaseState
{
    public AirborneState(FighterEntity fighter) : base(fighter) { }

    public override void EnterState()
    {
        Debug.Log("Entered Airborne State");
        fighter.animator?.SetBool("IsGrounded", false);
    }

    public override void UpdateState()
    {
        Vector3 moveDir = fighter.GetMovementInput();
        float airSpeed = fighter.walkSpeed;
        fighter.MoveEntity(moveDir, airSpeed);

        fighter.RotateEntity(moveDir);

        if (fighter.controller.isGrounded)
        {
            if (fighter.GetMovementInput().sqrMagnitude > 0.05f)
            {
                fighter.ChangeState(fighter.WalkState);
            }
            else
            {
                fighter.ChangeState(fighter.IdleState);
            }
        }
    }

    public override void ExitState()
    {
        fighter.animator?.SetBool("IsGrounded", true);
    }

    public override void FixedUpdateState()
    {
    }
}
