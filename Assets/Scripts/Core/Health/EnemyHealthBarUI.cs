using UnityEngine;
using UnityEngine.UI;

public class EnemyHealthBarUI : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private HealthComponent health;
    [SerializeField] private Image fillImage;
    [SerializeField] private GameObject barObject;

    [Header("Settings")]
    [SerializeField] private float hideDelay = 2f;

    private float hideTimer;

    private void Start()
    {
        if (health == null)
            health = GetComponentInParent<HealthComponent>();

        if (health == null)
        {
            Debug.LogError($"EnemyHealthBarUI: No se encontró HealthComponent.");
            return;
        }

        health.OnDamageTaken += OnDamageTaken;
        health.OnHealed += UpdateBar;

        if (barObject != null)
            barObject.SetActive(false);

        UpdateBar();
    }

    private void Update()
    {
        if (barObject == null)
            return;

        if (!barObject.activeSelf)
            return;

        hideTimer -= Time.deltaTime;

        if (hideTimer <= 0f)
        {
            barObject.SetActive(false);
        }
    }

    private void OnDamageTaken(float damage)
    {
        UpdateBar();

        if (barObject != null)
            barObject.SetActive(true);

        hideTimer = hideDelay;
    }

    private void UpdateBar()
    {
        if (fillImage == null || health == null)
            return;

        float percentage =
            health.CurrentHealth / health.MaxHealth;

        fillImage.fillAmount = percentage;
    }

    private void OnDestroy()
    {
        if (health == null)
            return;

        health.OnDamageTaken -= OnDamageTaken;
        health.OnHealed -= UpdateBar;
    }
}