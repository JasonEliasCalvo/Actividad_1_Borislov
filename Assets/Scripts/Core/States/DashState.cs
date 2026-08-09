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

        // Backflip (Hacia atrás por defecto)
        dashDirection = -fighter.transform.forward;
        fighter.animator.Play("Backflip", -1, 0f);

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