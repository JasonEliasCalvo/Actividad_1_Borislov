using UnityEngine;

public class DeathState : BaseState
{
    public DeathState(FighterEntity fighter) : base(fighter) { }

    public override bool CanBeInterrupted => false; // De la muerte no se vuelve (fácilmente)

    public override void EnterState()
    {
        Debug.Log("Entered Death State");
        fighter.velocity = Vector3.zero;
        fighter.animator.CrossFade("Death", 0.1f);
        fighter.controller.enabled = false; // Desactivar colisiones físicas para que no estorbe
    }

    public override void UpdateState()
    {
        // Nada. Se queda muerto.
    }

    public override void ExitState() { }
    public override void FixedUpdateState() { }
}
