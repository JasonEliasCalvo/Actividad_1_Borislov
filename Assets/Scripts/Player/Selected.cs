using UnityEngine;

public class Selected : MonoBehaviour
{
    [Header("Configuración de Interacción")]
    [SerializeField] private LayerMask interactableMask = ~0;
    [SerializeField] private string interactableTag = "Interactable";
    [SerializeField] private float distance = 2f;
    [SerializeField] private InteractableOptions currentInteractable;
    [SerializeField] private InteractableOptions enableInteractable;

    [Header("Fuentes de Rayo (Cámara y/o Control)")]
    [SerializeField] private Transform[] raySources;

    private GameObject lastSelectedObject;
    private Transform hightlight;
    public bool hitDetected = false;

    // -------------------------
    // Ciclo de vida
    // -------------------------
    private void OnEnable()
    {
        GameInputManager.OnInteractStarted += StartdHandleInteractInput;
        GameInputManager.OnInteractCanceled += EndHandleInteractInput;
    }

    private void OnDisable()
    {
        GameInputManager.OnInteractStarted -= StartdHandleInteractInput;
        GameInputManager.OnInteractCanceled -= EndHandleInteractInput;
    }

    void Update()
    {
        hitDetected = false;
        if (UIManager.instance != null && UIManager.instance.IsPanelActive())
        {
            DeselectLastObject();
            currentInteractable = null;
            UIManager.instance.ShowInteractPanel(false);

            if (hightlight != null)
                hightlight.gameObject.SetActive(false);

            return;
        }

        foreach (Transform source in raySources)
        {
            if (DetectFromInteractable(source))
            {
                hitDetected = true;
                break;
            }
        }

        if (!hitDetected)
        {
            DeselectLastObject();
            currentInteractable = null;
        }
    }

    // -------------------------
    // Detección
    // -------------------------
    private bool DetectFromInteractable(Transform source)
    {
        if (source == null) return false;

        Debug.DrawRay(source.position, source.forward * distance, Color.red, 0.1f);

        Ray ray = new Ray(source.position, source.forward);
        RaycastHit[] hits = Physics.RaycastAll(ray, distance, interactableMask);

        if (hits.Length > 0)
        {
            System.Array.Sort(hits, (a, b) => a.distance.CompareTo(b.distance));
            RaycastHit closestHit = hits[0]; // Corrección del error de 'hit'

            if (closestHit.collider.CompareTag(interactableTag))
            {
                GameObject hitObject = closestHit.collider.gameObject;

                if (hitObject != lastSelectedObject)
                {
                    DeselectLastObject();
                    SelectObject(hitObject);
                }

                currentInteractable = hitObject.GetComponent<InteractableOptions>();
                Debug.DrawRay(source.position, source.forward * distance, Color.green);
                return true;
            }
        }
        return false;
    }

    // -------------------------
    // Interacción (inputs)
    // -------------------------
    private void StartdHandleInteractInput()
    {
        if (currentInteractable != null)
        {
            currentInteractable.TryInteract();
            enableInteractable = currentInteractable;
        }
    }

    private void EndHandleInteractInput()
    {
        if (enableInteractable != null)
        {
            enableInteractable.EndInteract();
        }
    }

    // -------------------------
    // Selección y deselección
    // -------------------------
    private void SelectObject(GameObject obj)
    {
        hightlight = obj.transform.Find("Highlight");

        if (UIManager.instance != null)
            UIManager.instance.ShowInteractPanel(true);

        if (hightlight != null && !hightlight.gameObject.activeSelf)
            hightlight.gameObject.SetActive(true);
        else
            Debug.LogWarning("No se encontró el hijo del objeto " + obj.name);

        lastSelectedObject = obj;
    }

    private void DeselectLastObject()
    {
        if (lastSelectedObject != null)
        {
            if (UIManager.instance != null)
                UIManager.instance.ShowInteractPanel(false);

            if (hightlight != null)
                hightlight.gameObject.SetActive(false);

            lastSelectedObject = null;
        }
    }

    // -------------------------
    // API
    // -------------------------
    public InteractableOptions GetCurrentInteractable()
    {
        return currentInteractable;
    }
}
