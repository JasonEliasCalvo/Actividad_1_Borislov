using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class TriangleHacking : MonoBehaviour
{

    public Image triangleImage;
    [SerializeField] private Transform visualTransform;
    [SerializeField] private Transform triggerTransform;
    [SerializeField] private float moveSpeed = 8f;

    private float angleThisTriangle;
    private float currentRadius;
    private bool _isConnected = false;
    private Vector3 targetLocalPosition;

    public bool IsConnected => _isConnected;

    public void Initialize(float angle, float radius)
    {
        angleThisTriangle = angle;
        currentRadius = radius;
        UpdateAppearance();
        visualTransform.localPosition = targetLocalPosition;

        triggerTransform.position = visualTransform.position;

    }

    private void Update()
    {
        if (!_isConnected)
            triggerTransform.position = visualTransform.position;

        visualTransform.localPosition = Vector3.Lerp(visualTransform.localPosition, targetLocalPosition, moveSpeed * Time.deltaTime);
    }

    public void ToggleConnection()
    {
        _isConnected = !_isConnected;
        UpdateAppearance();
    }

    private void UpdateAppearance()
    {
        triangleImage.color = _isConnected ? Color.green : Color.red;

        float distance = _isConnected ? currentRadius / 2f : currentRadius;
        targetLocalPosition = new Vector3(Mathf.Cos(angleThisTriangle), Mathf.Sin(angleThisTriangle), 0) * distance;
    }
}
