using UnityEngine;

public class HitState : BaseState
{
    public float stunDuration = 0.5f;
    private float timer;

    private bool mirrorToggle = false;

    public HitState(FighterEntity fighter) : base(fighter) { }
    public override bool CanBeInterrupted => false;

    public override void EnterState()
    {
        Debug.Log("Entered Hit State");
        PlayHitAnimation();
    }

    public override void UpdateState()
    {
        timer -= Time.deltaTime;
        if (timer <= 0)
        {
            // Regresar a Idle
            fighter.ChangeState(fighter.IdleState);
        }
    }

    public void RefreshHit(float newDuration)
    {
        Debug.Log("Hit Refreshed!");
        stunDuration = newDuration;

        // Invertimos el valor del espejo (True -> False -> True)
        mirrorToggle = !mirrorToggle;

        PlayHitAnimation();
    }

    private void PlayHitAnimation()
    {
        fighter.velocity = Vector3.zero;
        timer = stunDuration;

        // 1. Configuramos el espejo antes de reproducir
        fighter.animator.SetBool("MirrorHit", mirrorToggle);

        // 2. Forzamos la reproducción desde el tiempo 0 inmediatamente
        // "Hit" es el nombre del Estado en el Animator, no del Clip.
        // -1 es la capa base, 0f es el tiempo normalized (principio).
        fighter.animator.Play("Hit", -1, 0f);

        // Reset de Hitboxes por seguridad
        fighter.AnimEvent_CloseHitbox(0);
        fighter.AnimEvent_CloseHitbox(1);
        fighter.AnimEvent_CloseHitbox(2);
        fighter.AnimEvent_CloseHitbox(3);
    }

    public override void ExitState()
    {
        // Opcional: Resetear el mirror al salir para que el próximo golpe empiece "normal"
        // mirrorToggle = false; 
    }

    public override void FixedUpdateState() { }
}