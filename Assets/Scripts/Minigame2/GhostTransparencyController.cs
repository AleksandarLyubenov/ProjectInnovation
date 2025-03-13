using UnityEngine;

public class GhostTransparencyController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Renderer objectRenderer;
    public bool isPassive;

    [Header("Vibration Settings")]
    [SerializeField] private float vibrationDistanceDivisor = 1.5f;

    [HideInInspector] public float currentAlpha = 0f;
    private Vector3 originalPosition;
    private bool isRegistered = false;

    private GhostSpawnHandler spawnHandler;
    private GameOverHandler gameOverHandler;

    void Update()
    {
        UpdateTransparency();
        UpdateVibrationRegistration();
    }

    public void SetHandlers(GhostSpawnHandler spawnHandler, GameOverHandler gameOverHandler)
    {
        this.spawnHandler = spawnHandler;
        this.gameOverHandler = gameOverHandler;
    }

    void Start()
    {
        originalPosition = transform.position;
        if (!objectRenderer) objectRenderer = GetComponent<Renderer>();
    }

    void UpdateVibrationRegistration()
    {
        // Always register hostile ghosts, but only when visible
        bool shouldRegister = currentAlpha >= 0.1f && !isPassive;

        if (shouldRegister && !isRegistered)
        {
            VibrationManager.Instance.RegisterActiveGhost(this);
            isRegistered = true;
        }
        else if (!shouldRegister && isRegistered)
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
        // Reduce divisor from 6f to 4f to shorten detection range
        currentAlpha = Mathf.Clamp01(1 - distance / vibrationDistanceDivisor);

        if (objectRenderer.material.HasProperty("_Color"))
        {
            Color color = objectRenderer.material.color;
            color.a = currentAlpha;
            objectRenderer.material.color = color;
        }
    }

    // Add cleanup when destroyed
    private void OnDestroy()
    {
        if (isRegistered)
        {
            VibrationManager.Instance.UnregisterActiveGhost(this);
        }
    }

    private float lastAlpha = 0f;
    private const float ALPHA_CHANGE_THRESHOLD = 0.05f;
    private bool isBeingDestroyed = false;

    public void DestroyGhost()
    {
        if (isBeingDestroyed) return;
        isBeingDestroyed = true;

        if (isPassive)
        {
            var collider = GetComponent<Collider>();
            if (collider) collider.enabled = false;

            if (spawnHandler != null)
            {
                spawnHandler.ClearExistingGhosts(); // Destroys all ghosts, including this one
            }

            if (gameOverHandler != null)
            {
                gameOverHandler.ShowResults(); // Show game over panel
            }
        }
        else
        {
            if (spawnHandler != null)
            {
                spawnHandler.OnCorrectGuess();
            }
            Destroy(gameObject);
        }
    }
}