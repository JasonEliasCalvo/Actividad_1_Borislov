using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[Flags]
public enum InteractionType
{
    None = 0,
    InvokeEvent = 1 << 0,
    GrabAndDrop = 1 << 1, // <- Nueva opción
}

public class InteractableOptions : MonoBehaviour
{
    [SerializeField] private InteractionType interactionTypes;
    [SerializeField] private bool justOneInteraction = false;
    [SerializeField] private bool canInteract = true;

    public UnityEvent onInteract;
    public UnityEvent endInteract;

    [Tooltip("Si está en FALSE, el objeto no será destruido al seleccionar")]
    public bool destroyOnSelect = false;

    public InteractionType InteractionTypes => interactionTypes;

    public void TryInteract()
    {
        if (!canInteract) {
            AudioManager.Instance.PlayFailSound();
            return;
        }

        if (justOneInteraction)
        {
            canInteract = false;
        }
        AudioManager.Instance.PLayInteractSound();
        ExecuteInteraction();
    }

    public void EndInteract()
    {
        EndInteraction();
    }

    public IEnumerable<InteractionType> GetActiveFlags()
    {
        foreach (InteractionType flag in System.Enum.GetValues(typeof(InteractionType)))
        {
            if (flag != InteractionType.None && interactionTypes.HasFlag(flag))
                yield return flag;
        }
    }

    private void ExecuteInteraction()
    {
        foreach (var type in GetActiveFlags())
        {
            switch (type)
            {
                case InteractionType.InvokeEvent:
                    onInteract?.Invoke();
                    break;

                case InteractionType.GrabAndDrop:
                    break;
            }
        }

        // Evitamos destruir el objeto si lo queremos agarrar
        Destroy();
    }

    public void Destroy()
    {
        if (destroyOnSelect)
        {
            Destroy(gameObject);
        }
    }

    private void EndInteraction()
    {
        foreach (var type in GetActiveFlags())
        {
            switch (type)
            {
                case InteractionType.InvokeEvent:
                    endInteract?.Invoke();
                    break;

                case InteractionType.GrabAndDrop:
                    break;
            }
        }
    }

    public void EnableInteraction() => canInteract = true;
    public void DisableInteraction() => canInteract = false;
}