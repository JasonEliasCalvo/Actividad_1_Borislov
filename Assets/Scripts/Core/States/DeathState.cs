using System.Collections;
using UnityEngine;

public class DeathState : BaseState
{
    public DeathState(FighterEntity fighter) : base(fighter) { }

    public override bool CanBeInterrupted => false;
    private float timer;
    public bool IsDeathAnimation;

    public override void EnterState()
    {
        Debug.Log("Entered Death State");
        fighter.velocity = Vector3.zero;
        fighter.animator.CrossFade("Death", 0.1f);
        fighter.controller.enabled = false;

        timer = 1f;
        IsDeathAnimation = true;
    }

    public override void UpdateState()
    {
        if (!IsDeathAnimation) return;

        Debug.Log("Death Animation Playing");

        timer -= Time.deltaTime;

        if (timer <= 0)
        {
            fighter.onDeath?.Invoke();
        }
    }

    public override void ExitState() { }
    public override void FixedUpdateState() { }
}
