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
        // 1. Obtener Input (Del Player o de la IA)
        Vector3 moveDir = fighter.GetMovementInput();

        // 2. Moverse
        fighter.MoveEntity(moveDir, fighter.walkSpeed);

        fighter.RotateEntity(moveDir);
        fighter.animator.SetFloat("Speed", moveDir.magnitude);

        // 3. Chequear Salidas
        if (moveDir.sqrMagnitude < 0.05f)
            fighter.ChangeState(fighter.IdleState);

        // 4. Chequear Caída
        if (!fighter.controller.isGrounded && fighter.verticalVelocity < -2f)
        {
            fighter.ChangeState(fighter.AirborneState);
        }

        if (fighter.GetAttackInput())
        {
            fighter.ResetCombo(); // Ponemos index a 0

            // Asignamos el primer ataque
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
