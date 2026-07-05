using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CombatHitbox : MonoBehaviour
{
    private float damage;
    private float hitStun;

    private FighterEntity owner; // Para no pegarnos a nosotros mismos
    public Collider myCollider;
    private List<IDamageable> victims = new List<IDamageable>(); // Lista de interfaz

    void Awake()
    {
        myCollider = GetComponent<Collider>();
        owner = GetComponentInParent<FighterEntity>();
        myCollider.enabled = false;
    }

    public void EnableHitbox(float dmg, float stun, float force)
    {
        damage = dmg;
        hitStun = stun;

        victims.Clear();
        myCollider.enabled = true;
    }

    public void DisableHitbox()
    {
        myCollider.enabled = false;
    }

    public static class CombatEffects
    {
        public static IEnumerator Hitstop(float duration)
        {
            float originalScale = Time.timeScale;
            Time.timeScale = 0f;
            yield return new WaitForSecondsRealtime(duration);
            Time.timeScale = originalScale;
        }
    }

    void OnTriggerEnter(Collider other)
    {
        // 1. Evitar al dueño
        if (owner != null && other.gameObject == owner.gameObject) return;

        // 2. Buscar si tiene la interfaz de daño
        IDamageable target = other.GetComponent<IDamageable>();

        if (target != null)
        {
            // 3. Evitar golpear al mismo objetivo dos veces en el mismo ataque
            if (victims.Contains(target)) return;
            victims.Add(target);

            // 4. Aplicar daño
            target.TakeDamage(damage, hitStun);

            // Opcional: Instanciar particulas aquí
            Debug.Log($"Hit confirmado en {other.name}");
        }
    }
}
