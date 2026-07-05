using UnityEngine;

public class AirborneState : BaseState
{
    public AirborneState(FighterEntity fighter) : base(fighter) { }

    public override void EnterState()
    {
        Debug.Log("Entered Airborne State");
        // Usamos un bool en el Animator para las animaciones de caída/salto
        fighter.animator?.SetBool("IsGrounded", false);
    }

    public override void UpdateState()
    {
        // 1. Obtener Input
        Vector3 moveDir = fighter.GetMovementInput();

        // 2. Aplicar Movimiento en el aire
        // En God Hand el control aéreo es limitado. 
        // Podemos multiplicar la velocidad por un valor pequeño si quieres menos control.
        float airSpeed = fighter.walkSpeed;
        fighter.MoveEntity(moveDir, airSpeed);

        // 3. Rotación (opcional en el aire, usualmente se permite para orientar el aterrizaje)
        fighter.RotateEntity(moveDir);

        // 4. Verificación de Aterrizaje
        // Importante: Chequeamos que la velocidad vertical sea hacia abajo o casi cero
        // para evitar que salga del estado apenas "roce" una rampa al subir.
        if (fighter.controller.isGrounded && fighter.verticalVelocity <= 0)
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
        // Al salir, avisamos al animator que ya tocamos tierra
        fighter.animator?.SetBool("IsGrounded", true);
    }

    public override void FixedUpdateState()
    {
    }
}
