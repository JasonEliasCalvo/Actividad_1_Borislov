using UnityEngine;
using UnityEngine.UIElements;

public class PointerTrigger : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D other)
    {
        Debug.Log(other);
        if (other.CompareTag("Triangle"))
        {
            TriangleHacking triangle = other.GetComponent<TriangleHacking>();

            if (triangle != null && HackingMiniGame.Active != null)
            {
                HackingMiniGame.Active.SetCurrentTarget(triangle);
            }
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other == null) return;

        TriangleHacking triangle = other.GetComponent<TriangleHacking>();

        if (triangle != null &&
            HackingMiniGame.Active.CurrentTarget == triangle)
        {
            HackingMiniGame.Active.ClearCurrentTarget();
        }
    }
}
