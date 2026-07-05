using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "CombatDatabase", menuName = "Combat Database")]
public class AttackDataSO : ScriptableObject
{
    public List<AttackBase> allAttacks;
}