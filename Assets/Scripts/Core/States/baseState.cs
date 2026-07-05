using UnityEngine;

public abstract class BaseState
{
    protected FighterEntity fighter; // Referencia genérica (sirve para Player y Enemy)

    public BaseState(FighterEntity fighter)
    {
        this.fighter = fighter;
    }

    public virtual bool CanBeInterrupted => true;

    public abstract void EnterState();
    public abstract void UpdateState();
    public abstract void FixedUpdateState();
    public abstract void ExitState();
}

