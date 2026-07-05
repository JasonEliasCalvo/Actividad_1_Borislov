using DG.Tweening;
using System.Collections.Generic;
using UnityEngine;

public class HackingMiniGame : MonoBehaviour
{
    [Header("UI Elements")]
    [SerializeField] private RectTransform panel;
    [SerializeField] private CanvasGroup canvasGroup;
    [SerializeField] private Transform centerPoint;
    [SerializeField] private GameObject trianglePrefab;
    [SerializeField] private Transform pointer;

    [Header("Settings")]
    [SerializeField] private int triangleCount = 6;
    [SerializeField] private float radius = 100f;
    [SerializeField] private float rotationSpeed = 100f;


    private List<TriangleHacking> triangles = new();
    private float angleStep;
    private bool isActive = false;

    private TriangleHacking currentTarget;
    public TriangleHacking CurrentTarget => currentTarget;

    private void Start()
    {
        GenerateTriangles();
    }

    private void Update()
    {
        if (!isActive) return;

        RotatePointer();

        if (Input.GetMouseButtonDown(0) && currentTarget != null)
        {
            currentTarget.ToggleConnection();
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

    private void GenerateTriangles()
    {
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
        pointer.RotateAround(centerPoint.position, Vector3.forward, rotationSpeed * Time.deltaTime);
    }

    private void CheckHackCompletion()
    {
        foreach (var triangle in triangles)
        {
            if (!triangle.IsConnected)
                return;
        }

        Debug.Log("¡Hackeo exitoso!");
        HideMiniGame();
    }

    public void ShowMiniGame()
    {
        isActive = true;
        //canvasGroup.alpha = 0;
        //panel.localScale = Vector3.zero;

        //canvasGroup.DOFade(1, 0.5f);
        //panel.DOScale(1, 0.5f).SetEase(Ease.OutBack);
    }

    public void HideMiniGame()
    {
        isActive = false;
        canvasGroup.DOFade(0, 0.4f);
        panel.DOScale(0, 0.4f).SetEase(Ease.InBack);
    }
}
