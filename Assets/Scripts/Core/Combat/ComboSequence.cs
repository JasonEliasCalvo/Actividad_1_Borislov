using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Combo", menuName = "Combat/Combo Sequence")]
public class ComboSequence : ScriptableObject
{
    public List<AttackBase> attacks;
}