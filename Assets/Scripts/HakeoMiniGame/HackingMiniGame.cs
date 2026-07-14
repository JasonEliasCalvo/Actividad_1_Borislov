using DG.Tweening;
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

    private List<TriangleHacking> triangles = new();
    private float angleStep;
    public bool isActive = false;

    private TriangleHacking currentTarget;
    public TriangleHacking CurrentTarget => currentTarget;

    [SerializeField]  public UnityEvent onHackCompleted;

    private void Update()
    {
        if (!isActive) return;

        Debug.Log("Rotando");

        Debug.Log(pointer.position);
        Debug.Log(centerPoint.position);

        RotatePointer();

        if (Input.GetMouseButtonDown(0) && currentTarget != null)
        {
            Debug.Log("Conectando triángulo");

            currentTarget.ToggleConnection();

            if(rotationSpeed > 0)
                rotationSpeed = Mathf.Abs(rotationSpeed) * -1f; // Invert rotation direction
            else
                rotationSpeed = Mathf.Abs(rotationSpeed) * 1f; // Invert rotation direction

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

        angleStep = 360f / triangleCount;
        for (int i = 0; i < triangleCount; i++)
        {
            float angle = i * angleStep * Mathf.Deg2Rad;

            GameObject triangleObj = Instantiate(trianglePrefab, centerPoint.position, Quaternion.identity, centerPoint);
            TriangleHacking triangle = triangleObj.GetComponentInChildren<TriangleHacking>();
            triangle.Initialize(angle, radius);
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
        isActive = true;
        panel.localScale = Vector3.zero;
        panel.gameObject.SetActive(true);
        panel.DOScale(1, 0.5f).SetEase(Ease.OutBack);
    }

    public void HideMiniGame()
    {
        isActive = false;
        panel.gameObject.SetActive(false);
        panel.DOScale(0, 0.4f).SetEase(Ease.InBack);
    }
}
