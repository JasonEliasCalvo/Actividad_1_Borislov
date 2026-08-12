using UnityEngine;
using UnityEngine.UI;

public class HealthBarUI : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private HealthComponent health;
    [SerializeField] private Image fillImage;

    [Header("Settings")]
    [SerializeField] private bool hideWhenFull = false;

    private void Start()
    {
        if (health == null)
            health = GetComponentInParent<HealthComponent>();

        if (health == null)
        {
            Debug.LogError($"HealthBarUI: No se encontró HealthComponent en {gameObject.name}");
            return;
        }

        health.OnDamageTaken += OnDamageTaken;
        health.OnHealed += OnHealed;

        UpdateBar();
    }

    private void OnDestroy()
    {
        if (health == null)
            return;

        health.OnDamageTaken -= OnDamageTaken;
        health.OnHealed -= OnHealed;
    }

    private void OnDamageTaken(float damage)
    {
        UpdateBar();
    }

    private void OnHealed()
    {
        UpdateBar();
    }

    private void UpdateBar()
    {
        if (fillImage == null || health == null)
            return;

        float percentage = health.CurrentHealth / health.MaxHealth;

        fillImage.fillAmount = percentage;

        if (hideWhenFull)
            fillImage.gameObject.SetActive(percentage < 1f);
    }
}