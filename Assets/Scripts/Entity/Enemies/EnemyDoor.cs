using UnityEngine;

public class EnemyDoor : FighterEntity
{
    public override Vector3 GetMovementInput()
    {
        return Vector3.zero;
    }

    public override bool GetAttackInput()
    {
        return false;
    }


    protected override void Update()
    {
        base.Update();
    }
}