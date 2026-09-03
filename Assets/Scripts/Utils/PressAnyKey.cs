using UnityEngine;
using UnityEngine.Events;

public class PressAnyKey : MonoBehaviour
{
    [SerializeField] private UnityEvent onInputDetected;

    private bool triggered;

    private void Update()
    {
        if (triggered)
            return;

        if (Input.anyKeyDown)
        {
            triggered = true;
            onInputDetected?.Invoke();
        }
    }
}