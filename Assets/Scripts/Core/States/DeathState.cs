using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

public class DeathState : BaseState
{
    public DeathState(FighterEntity fighter) : base(fighter) { }

    public override bool CanBeInterrupted => false;

    public override void EnterState()
    {
        Debug.Log("Entered Death State");
        fighter.velocity = Vector3.zero;
        fighter.animator.CrossFade("Death", 0.1f);
        fighter.controller.enabled = false;
    }

    public override void UpdateState()
    {
        if (fighter.animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1f)
        {
            Debug.Log("Death animation finished");
            fighter.onDeath?.Invoke();
        }
    }

    public override void ExitState() { }
    public override void FixedUpdateState() { }
}
