using UnityEngine;

public enum EnemyState
{
    Normal,
    Stunned,
    Downed
}

public class enemyStateHandler : MonoBehaviour
{
    public bool IsStunned { get; private set; }
    public bool IsDowned { get; private set; }
    public bool CanBeParried => !IsDowned && !IsStunned;

    //public void ApplyStun(float duration);
    //public void Knockdown();
    //public void TryParry();
}
