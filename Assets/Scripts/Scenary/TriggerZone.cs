using UnityEngine;
using UnityEngine.Events;

public class TriggerZone : MonoBehaviour
{
    public string interactionTag;
    public bool destroyAfterTrigger = false;

    [Space(20)]

    public UnityEvent onTriggerEnter;
    public UnityEvent onTriggerStay;
    public UnityEvent onTriggerExit;

    public void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(interactionTag))
        {
            if (destroyAfterTrigger)
            {
                Destroy(other.gameObject);
            }
            onTriggerEnter.Invoke();
        }
    }

    public void OnTriggerStay(Collider other)
    {
        if (other.CompareTag(interactionTag))
        {
            onTriggerStay.Invoke();
        }
    }

    public void OnTriggerExit(Collider other)
    {
        if (other.CompareTag(interactionTag))
        {
            onTriggerExit.Invoke();
        }
    }
}
