using UnityEngine;

public class Door : MovableObject
{
    public enum DoorType
    {
        Sliding, // Corrediza (Posición)
        Rotation // Bisagra (Rotación)
    }

    [Header("Door Settings")]
    [SerializeField] private DoorType type = DoorType.Sliding;
    [SerializeField] private bool isOpen = false;
    [SerializeField] private float closeDelay = 0f;
    [SerializeField] private AudioClip Sound;

    private void Start()
    {
        if (movePoints == null || movePoints.Length < 2)
        {
            Debug.LogError($"[Door] {gameObject.name} necesita al menos 2 puntos en 'movePoints' (0: Cerrado, 1: Abierto).", this);
            return;
        }

        UpdateTarget();

        if (currentTarget != null)
        {
            if (type == DoorType.Sliding)
            {
                transform.position = currentTarget.position;
            }
            else
            {
                transform.rotation = currentTarget.rotation;
            }
        }
    }

    protected override void Move()
    {
        if (currentTarget == null) return;

        bool arrived = false;

        if (type == DoorType.Sliding)
        {
            // Movimiento lineal para puertas corredizas
            transform.position = Vector3.MoveTowards(transform.position, currentTarget.position, moveSpeed * Time.deltaTime);
            arrived = Vector3.Distance(transform.position, currentTarget.position) < 0.01f;

            if (arrived) transform.position = currentTarget.position; // Snap
        }
        else
        {
            // Movimiento angular para puertas con bisagra (moveSpeed actúa como grados por segundo)
            transform.rotation = Quaternion.RotateTowards(transform.rotation, currentTarget.rotation, moveSpeed * Time.deltaTime);
            arrived = Quaternion.Angle(transform.rotation, currentTarget.rotation) < 0.1f;

            if (arrived) transform.rotation = currentTarget.rotation; // Snap
        }

        // Si llegó a su destino (abierta o cerrada), detenemos el Update
        if (arrived)
        {
            StopMoving();

            // Auto-cierre opcional si se acaba de abrir
            if (isOpen && closeDelay > 0f)
            {
                Invoke(nameof(CloseDoor), closeDelay);
            }
        }
    }

    /// <summary>
    /// Cambia el estado de la puerta (Si está abierta se cierra, si está cerrada se abre)
    /// </summary>
    public void ToggleDoor()
    {
        CancelInvoke(nameof(CloseDoor)); // Cancela auto-cierres pendientes si el jugador interactúa manualmente
        isOpen = !isOpen;
        UpdateTarget();
        StartMoving();
        if (Sound != null)
        {
            AudioManager.Instance.PlaySound(Sound);
        }
    }

    public void OpenDoor()
    {
        if (isOpen) return;
        CancelInvoke(nameof(CloseDoor));
        isOpen = true;
        UpdateTarget();
        StartMoving();

        if (Sound != null)
        {
            AudioManager.Instance.PlaySound(Sound);
        }
    }

    public void CloseDoor()
    {
        if (!isOpen) return;
        isOpen = false;
        UpdateTarget();
        StartMoving();

        if (Sound != null)
        {
            AudioManager.Instance.PlaySound(Sound);
        }
    }

    private void UpdateTarget()
    {
        // movePoints[1] = Abierta || movePoints[0] = Cerrada
        if (movePoints != null && movePoints.Length >= 2)
        {
            currentTarget = isOpen ? movePoints[1] : movePoints[0];
        }
    }
}