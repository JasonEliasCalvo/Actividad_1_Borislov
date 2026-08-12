using UnityEngine;
using UnityEngine.UI;

public class GasMaskBarUI : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private PlayerFighter player;
    [SerializeField] private Image fillImage;
    [SerializeField] private GameObject barObject;

    private void Start()
    {
        if (player == null)
            player = FindAnyObjectByType<PlayerFighter>();

        if (barObject != null)
            barObject.SetActive(false);
    }

    private void Update()
    {
        if (player == null)
            return;

        if (!player.hasGasMask)
        {
            barObject.SetActive(false);
            return;
        }

        if (!player.gasMaskActive)
        {
            barObject.SetActive(false);
            return;
        }

        barObject.SetActive(true);

        float percentage =
            player.gasMaskTimer / player.gasMaskDuration;

        fillImage.fillAmount = percentage;
    }
}