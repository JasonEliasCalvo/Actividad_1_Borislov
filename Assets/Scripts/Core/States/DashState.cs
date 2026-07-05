using UnityEngine;

public class DashState : BaseState
{
    // Creamos una referencia específica para el Jugador
    private PlayerFighter player;
    private Vector3 dashDirection;

    public DashState(FighterEntity entity) : base(entity)
    {
        player = entity as PlayerFighter;
    }

    public override void EnterState()
    {
        Debug.Log("Entered Dash State");

        if (player == null)
        {
            fighter.ChangeState(fighter.IdleState);
            return;
        }

        // 1: Usamos GetMovementInput()
        Vector3 inputDir = fighter.GetMovementInput();

        if (inputDir == Vector3.zero)
        {
            // Backflip (Hacia atrás por defecto)
            dashDirection = -fighter.transform.forward;
            fighter.animator?.SetTrigger("Dash");
        }
        else
        {
            // Side-step o Forward Dash según el ángulo del input vs forward
            dashDirection = inputDir.normalized;
            fighter.animator?.SetTrigger("Dash");
        }

        // Para el DUCK: Podrías disparar un estado diferente si Shift está presionado

        // 2: Iniciamos el cooldown en el player
        player.dashTimer = player.dashDuration;
        player.StartDashCooldown();
    }

    public override void UpdateState()
    {
        player.dashTimer -= Time.deltaTime;
        fighter.velocity = dashDirection * player.dashSpeed;

        fighter.verticalVelocity = 0f; 

        if (player.dashTimer <= 0f)
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

    public override void FixedUpdateState()
    {
    }

    public override void ExitState()
    {
        fighter.IsInvulnerable = false;
        fighter.velocity *= player.dashForceStop;
    }
}