using UnityEngine;
using UnityEngine.UI;

public class PointerRotator : MonoBehaviour
{
    public Transform target;
    public float rotationSpeed = 10f;

    private RectTransform rectTransform;
    private Camera mainCamera;
    private Vector3 viewportCenter = new Vector3(0.5f, 0.5f, 0);

    void Start()
    {
        rectTransform = GetComponent<RectTransform>();
        mainCamera = Camera.main;

        if (rectTransform.pivot != new Vector2(0.35f, 0.5f))
        {
            rectTransform.pivot = new Vector2(0.35f, 0.5f);
        }
    }

    void Update()
    {
        if (target == null) return;

        Vector3 targetViewport = mainCamera.WorldToViewportPoint(target.position);
        Vector3 pointerViewport = mainCamera.WorldToViewportPoint(rectTransform.position);

        Vector2 direction = targetViewport - pointerViewport;

        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

        float perspectiveFactor = Mathf.Clamp(targetViewport.x * 2 - 1, -1f, 1f);
        angle += perspectiveFactor * 15f;

        Quaternion targetRotation = Quaternion.Euler(0, 0, angle - 90);
        rectTransform.rotation = Quaternion.Lerp(
            rectTransform.rotation,
            targetRotation,
            Time.deltaTime * rotationSpeed
        );
    }
}