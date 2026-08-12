using UnityEngine;

public class PoisonZone : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        PlayerFighter player = other.GetComponent<PlayerFighter>();

        if (player == null)
            return;

        if (player.gasMaskActive)
            return;

        player.InstantDeath();
    }
}