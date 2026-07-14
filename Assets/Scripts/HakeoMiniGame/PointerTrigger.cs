using UnityEngine;
using UnityEngine.UIElements;

public class PointerTrigger : MonoBehaviour
{
    private HackingMiniGame hackingMiniGame;

    private void Start()
    {
        hackingMiniGame = FindAnyObjectByType<HackingMiniGame>();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        Debug.Log(other);
        if (other.CompareTag("Triangle"))
        {
            Debug.Log("Puedes conectar");
            TriangleHacking triangle = other.GetComponent<TriangleHacking>();
            if (triangle != null)
            {
                
                hackingMiniGame.SetCurrentTarget(triangle);
            }
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other == null) return;

        TriangleHacking triangle = other.GetComponentInParent<TriangleHacking>();
        if (triangle != null && hackingMiniGame.CurrentTarget == triangle)
        {
            hackingMiniGame.ClearCurrentTarget();
        }
    }
}
