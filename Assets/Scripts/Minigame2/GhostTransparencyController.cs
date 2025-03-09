using UnityEngine;

public class GhostTransparencyController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Renderer objectRenderer;
    public bool isPassive;

    [HideInInspector] public float currentAlpha = 0f;
    private Vector3 originalPosition;
    private bool isRegistered = false;

    void Update()
    {
        UpdateTransparency();
        UpdateVibrationRegistration();
    }

    void Start()
    {
        originalPosition = transform.position;
        if (!objectRenderer) objectRenderer = GetComponent<Renderer>();
    }

    void UpdateVibrationRegistration()
    {
        bool isActive = currentAlpha >= 0.5f;

        if (isActive && !isRegistered)
        {
            VibrationManager.Instance.RegisterActiveGhost(this);
            isRegistered = true;
        }
        else if (!isActive && isRegistered)
        {
            VibrationManager.Instance.UnregisterActiveGhost(this);
            isRegistered = false;
        }
    }

    void UpdateTransparency()
    {
        if (!FlashlightController.Instance || !objectRenderer) return;

        Vector3 ghostPosition = new Vector3(transform.position.x, transform.position.y, 0f);
        Vector3 flashlightPosition = new Vector3(FlashlightController.Instance.transform.position.x, FlashlightController.Instance.transform.position.y, 0f);

        float distance = Vector3.Distance(ghostPosition, flashlightPosition);
        currentAlpha = Mathf.Clamp01(1 - distance / 6f);

        if (objectRenderer.material.HasProperty("_Color"))
        {
            Color color = objectRenderer.material.color;
            color.a = currentAlpha;
            objectRenderer.material.color = color;
        }
    }

    private float lastAlpha = 0f;
    private const float ALPHA_CHANGE_THRESHOLD = 0.05f;

    public void DestroyGhost()
    {
        Debug.Log($"💀 {name} Destroyed.");
        Destroy(gameObject);
    }
}