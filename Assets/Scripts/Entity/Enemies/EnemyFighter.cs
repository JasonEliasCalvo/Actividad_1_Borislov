using UnityEngine;

public class EnemyFighter : FighterEntity
{
    // Lógica simple de IA (Dummy)

    public override Vector3 GetMovementInput()
    {
        // Por ahora el dummy no se mueve
        return Vector3.zero;
    }

    public override bool GetAttackInput()
    {
        // Por ahora el dummy no ataca, solo recibe golpes
        return false;
    }

    // Si tuvieras que añadir lógica específica (como mirar al jugador), iría en Update
    protected override void Update()
    {
        base.Update(); // Importante mantener el update del padre
    }
}