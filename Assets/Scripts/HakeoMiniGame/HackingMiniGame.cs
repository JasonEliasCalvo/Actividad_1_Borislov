using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class HackingMiniGame : MonoBehaviour
{
    [Header("UI Elements")]
    [SerializeField] private RectTransform panel;
    [SerializeField] private Transform centerPoint;
    [SerializeField] private GameObject trianglePrefab;
    [SerializeField] private Transform pointer;

    [Header("Settings")]
    [SerializeField] private int triangleCount = 6;
    [SerializeField] private float radius = 100f;
    [SerializeField] private float rotationSpeed = 100f;
    [SerializeField] private float hackCompletedDelay = 0.5f;

    private List<TriangleHacking> triangles = new();
    private float angleStep;
    public bool isActive = false;
    public bool isInverted = false;

    [SerializeField] private TriangleHacking currentTarget;
    public TriangleHacking CurrentTarget => currentTarget;

    [SerializeField]  public UnityEvent onHackCompleted;

    public static HackingMiniGame Active;

    private void Update()
    {
        if (!isActive) return;

        Debug.Log("Rotando");

        RotatePointer();

        if (Input.GetMouseButtonDown(0) && currentTarget == null)
        {
            Debug.Log("No hay triángulo seleccionado");
            return;
        }

        if (Input.GetMouseButtonDown(0) && currentTarget != null)
        {
            Debug.Log("Conectando triángulo");

            currentTarget.ToggleConnection();

            if(isInverted)
            {
                if (rotationSpeed > 0)
                    rotationSpeed = Mathf.Abs(rotationSpeed) * -1f; // Invert rotation direction
                else
                    rotationSpeed = Mathf.Abs(rotationSpeed) * 1f; // Invert rotation direction
            }

            CheckHackCompletion();
        }
    }
    public void SetCurrentTarget(TriangleHacking triangle)
    {
        currentTarget = triangle;
    }

    public void ClearCurrentTarget()
    {
        if (currentTarget != null)
            currentTarget = null;
    }

    public void StartAndShowGame()
    {
        ResetGameData();
        GenerateTriangles();
    }

    private void GenerateTriangles()
    {
        Debug.Log("Generating triangles for the hacking mini-game.");

        foreach (Transform child in centerPoint)
        {
            Destroy(child.gameObject);
        }

        Debug.Log(centerPoint.childCount);

        triangles.Clear();
        ClearCurrentTarget();

        angleStep = 360f / triangleCount;
        for (int i = 0; i < triangleCount; i++)
        {
            float angle = i * angleStep * Mathf.Deg2Rad;

            GameObject triangleObj = Instantiate(trianglePrefab, centerPoint.position, Quaternion.identity, centerPoint);
            
            TriangleHacking triangle = triangleObj.GetComponentInChildren<TriangleHacking>();
            triangle.Initialize(angle, radius);

            Vector2 dir = (centerPoint.position - triangle.transform.position).normalized;
            float rot = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg - 90f;
            triangle.triangleImage.transform.rotation = Quaternion.Euler(0, 0, rot);

            triangles.Add(triangle);
        }

        ShowMiniGame();
    }

    void RotatePointer()
    {
        pointer.Rotate(0, 0, rotationSpeed * Time.deltaTime);
    }

    private void CheckHackCompletion()
    {
        foreach (var triangle in triangles)
        {
            if (!triangle.IsConnected)
                return;
        }

        StartCoroutine(HackCompletedRoutine());
    }

    private IEnumerator HackCompletedRoutine()
    {
        isActive = false;

        yield return new WaitForSeconds(hackCompletedDelay);

        Debug.Log("¡Hackeo exitoso!");

        onHackCompleted?.Invoke();
        HideMiniGame();
    }

    private void ResetGameData()
    {
        foreach (var triangle in triangles)
        {
            if (triangle != null) Destroy(triangle.gameObject);
        }
        triangles.Clear();
        ClearCurrentTarget();
    }

    public void ShowMiniGame()
    {
        Active = this;

        isActive = true;
        panel.localScale = Vector3.zero;
        panel.gameObject.SetActive(true);
        panel.DOScale(1, 0.5f).SetEase(Ease.OutBack);
    }

    public void HideMiniGame()
    {
        if (Active == this)
            Active = null;

        isActive = false;

        panel.DOScale(0, 0.4f)
            .SetEase(Ease.InBack)
            .OnComplete(() =>
            {
                panel.gameObject.SetActive(false);

                foreach (Transform child in centerPoint)
                {
                    Destroy(child.gameObject);
                }

                Debug.Log(centerPoint.childCount);
            });
    }
}
