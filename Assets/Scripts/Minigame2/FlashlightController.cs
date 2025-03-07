using UnityEngine;
using System.Collections;

public class FlashlightController : MonoBehaviour
{
    public static FlashlightController Instance;
    public Light spotlight;
    public float flashRange = 12f;
    [SerializeField] private int killedEnemies = 0;
    [SerializeField] private float deadzoneRadius = 1f;

    [Header("Controls")]
    public bool useGyro = false;

    [Header("Flash Effect")]
    [SerializeField] private float flashIntensityMultiplier = 3f;
    [SerializeField] private float flashSpotAngle = 170f;
    [SerializeField] private float flashDuration = 1f;

    private Gyroscope gyro;
    private bool gyroEnabled;
    private float originalIntensity;
    private float originalSpotAngle;
    private Coroutine flashRoutine;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }

        // Store original light properties
        originalIntensity = spotlight.intensity;
        originalSpotAngle = spotlight.spotAngle;

        if (useGyro)
        {
            EnableGyro();
        }
    }

    void EnableGyro()
    {
        if (SystemInfo.supportsGyroscope)
        {
            gyro = Input.gyro;
            gyro.enabled = true;
            gyroEnabled = true;
        }
        else
        {
            Debug.LogWarning("Gyroscope not supported on this device!");
            gyroEnabled = false;
        }
    }

    void Update()
    {
        if (useGyro && gyroEnabled)
        {
            Vector3 gravity = Input.gyro.gravity; // Get gravity vector

            float moveX = Mathf.Lerp(transform.position.x, gravity.x * 32f, Time.deltaTime * 25f); // Left/Right
            float moveY = Mathf.Lerp(transform.position.y, -gravity.z * 18f, Time.deltaTime * 25f); // Forward/Backward (away/toward)

            transform.position = new Vector3(moveX, moveY, -10f);
        }
        else
        {
            Vector3 mousePos = Input.mousePosition;
            mousePos.z = 10f;
            Vector3 worldPos = Camera.main.ScreenToWorldPoint(mousePos);
            transform.position = new Vector3(worldPos.x, worldPos.y, -10f);
        }

        Debug.DrawRay(transform.position, transform.forward * flashRange, Color.yellow);

        if (Input.GetMouseButtonDown(0))
        {
            Flash();
        }
    }

    void Flash()
    {
        // Existing flash logic
        RaycastHit hit;
        if (Physics.Raycast(transform.position, transform.forward, out hit, flashRange))
        {
            Collider[] closeHits = Physics.OverlapSphere(hit.point, deadzoneRadius);
            foreach (Collider closeHit in closeHits)
            {
                if (closeHit.CompareTag("Ghost"))
                {
                    GhostTransparencyController ghost = closeHit.GetComponent<GhostTransparencyController>();

                    if (ghost != null)
                    {
                        Debug.Log($"Ghost Detected: {ghost.name}, Alpha: {ghost.currentAlpha}");

                        if (ghost.currentAlpha > 0.5f)
                        {
                            if (!ghost.isPassive)
                            {
                                killedEnemies++;
                                Debug.Log($"Enemy Eliminated! Total: {killedEnemies}");
                                ghost.DestroyGhost();
                            }
                            else
                            {
                                Debug.Log("Game Over - Passive Ghost Eliminated!");
                                ghost.DestroyGhost();
                            }
                        }
                    }
                }
            }
        }

        // Trigger flash effect
        if (flashRoutine != null)
        {
            StopCoroutine(flashRoutine);
        }
        flashRoutine = StartCoroutine(FlashEffect());
    }

    private IEnumerator FlashEffect()
    {
        // Apply flash effect
        spotlight.intensity = originalIntensity * flashIntensityMultiplier;
        spotlight.spotAngle = flashSpotAngle;

        // Wait for duration
        yield return new WaitForSeconds(flashDuration);

        // Revert to original values
        spotlight.intensity = originalIntensity;
        spotlight.spotAngle = originalSpotAngle;
        flashRoutine = null;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawLine(transform.position, transform.position + transform.forward * flashRange);

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, deadzoneRadius);
    }
}
