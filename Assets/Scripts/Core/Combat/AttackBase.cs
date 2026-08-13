using System;
using UnityEngine;

[CreateAssetMenu(fileName = "New Attack", menuName = "Combat/Attack")]
public class AttackBase : ScriptableObject
{
    [Header("Identidad")]
    public string attackName;

    [Header("Animation")]
    public string animationStateName;

    [Header("Stats Básicos")]
    public float damage = 10f;
    public float hitStun = 0.5f;
    public float knockbackForce = 5f;
    public float hitstopDuration = 0.1f;

    [Header("VFX & SFX")]
    public GameObject hitParticle;
    public AudioClip hitSound;
}
