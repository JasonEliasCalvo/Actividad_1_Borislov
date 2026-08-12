using UnityEngine;
using UnityEngine.UI;

public class ProximityHealthBarUI : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private HealthComponent health;
    [SerializeField] private GameObject barObject;
    [SerializeField] private Image fillImage;

    [Header("Settings")]
    [SerializeField] private float showDistance = 8f;

    private Transform player;
    private void Start()
    {
        GameObject playerObject = GameObject.FindGameObjectWithTag("Player");

        if (playerObject != null)
        {
            player = playerObject.transform;
        }

        if (health == null)
            health = GetComponentInParent<HealthComponent>();

        if (barObject != null)
            barObject.SetActive(false);

        UpdateBar();
    }

    private void Update()
    {
        if (health == null || player == null)
            return;

        float distance = Vector3.Distance(
            player.position,
            transform.position
        );

        bool shouldShow = distance <= showDistance;

        if (barObject != null)
            barObject.SetActive(shouldShow);

        if (shouldShow)
            UpdateBar();
    }

    private void UpdateBar()
    {
        if (fillImage == null)
            return;

        fillImage.fillAmount =
            health.CurrentHealth / health.MaxHealth;
    }
}