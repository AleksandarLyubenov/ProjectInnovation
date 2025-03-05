using UnityEngine;

public class GhostTransparencyController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Renderer objectRenderer;
    public bool isPassive;

    [HideInInspector] public float currentAlpha = 0f;
    private Vector3 originalPosition;
    private AndroidJavaObject vibrator;
    private AndroidJavaObject vibrationEffect;
    [SerializeField] private bool isVibrating = false;

    void Start()
    {
        originalPosition = transform.position;
        if (!objectRenderer) objectRenderer = GetComponent<Renderer>();

        if (Application.platform == RuntimePlatform.Android)
        {
            // Check for VIBRATE permission
            if (!HasVibratePermission())
            {
                Debug.LogWarning("VIBRATE permission not granted!");
                RequestVibratePermission();
                return;
            }

            using (AndroidJavaClass unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer"))
            {
                AndroidJavaObject activity = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity");
                vibrator = activity.Call<AndroidJavaObject>("getSystemService", "vibrator");
            }
        }
    }

    bool HasVibratePermission()
    {
        using (AndroidJavaClass contextClass = new AndroidJavaClass("android.content.Context"))
        using (AndroidJavaObject activity = new AndroidJavaClass("com.unity3d.player.UnityPlayer").GetStatic<AndroidJavaObject>("currentActivity"))
        {
            int permissionResult = activity.Call<int>("checkSelfPermission", "android.permission.VIBRATE");
            return permissionResult == 0; // 0 means granted
        }
    }

    void RequestVibratePermission()
    {
        using (AndroidJavaObject activity = new AndroidJavaClass("com.unity3d.player.UnityPlayer").GetStatic<AndroidJavaObject>("currentActivity"))
        {
            string[] permissions = { "android.permission.VIBRATE" };
            activity.Call("requestPermissions", permissions, 1); // 1 is the request code
        }
    }

    void Update()
    {
        UpdateTransparency();
        HandleVibration();
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

    void HandleVibration()
    {
        if (!HasVibratePermission())
        {
            Debug.LogWarning("VIBRATE permission not granted. Vibration disabled.");
            return;
        }

        if (currentAlpha < 0.5f)
        {
            if (isVibrating)
            {
                StopVibration();
                isVibrating = false;
            }
            return;
        }

        float vibrationStrength = Mathf.Lerp(50, 255, currentAlpha);

        if (Application.platform == RuntimePlatform.Android && vibrator != null)
        {
            bool alphaChangedSignificantly = Mathf.Abs(currentAlpha - lastAlpha) > ALPHA_CHANGE_THRESHOLD;

            if (!isVibrating || alphaChangedSignificantly)
            {
                StopVibration();

                if (isPassive)
                {
                    StartVibrationPattern(currentAlpha);
                    Debug.Log($"[Passive] Pattern updated (Alpha: {currentAlpha:F2})");
                }
                else
                {
                    StartConstantVibration((int)vibrationStrength);
                    Debug.Log($"[Enemy] Strength updated: {vibrationStrength:F2}");
                }

                isVibrating = true;
                lastAlpha = currentAlpha;
            }
        }
        else
        {
            Debug.Log($"[DEBUG] Vibration: {(isPassive ? "Pattern" : "Constant")}");
        }
    }

    void StartVibrationPattern(float alpha)
    {
        if (vibrator != null)
        {
            long vibrateDuration = 100;
            float patternIntensity = (alpha - 0.5f) * 2f;
            long pauseDuration = (long)Mathf.Lerp(300, 50, patternIntensity);

            long[] pattern = { 0, vibrateDuration, pauseDuration };

            if (GetAndroidSDKVersion() >= 26)
            {
                using (AndroidJavaClass vibrationEffectClass = new AndroidJavaClass("android.os.VibrationEffect"))
                {
                    AndroidJavaObject effect = vibrationEffectClass.CallStatic<AndroidJavaObject>(
                        "createWaveform",
                        pattern,
                        -1
                    );
                    vibrator.Call("vibrate", effect);
                }
            }
            else
            {
                vibrator.Call("vibrate", pattern, -1);
            }
        }
    }

    int GetAndroidSDKVersion()
    {
        using (AndroidJavaClass buildVersion = new AndroidJavaClass("android.os.Build$VERSION"))
        {
            return buildVersion.GetStatic<int>("SDK_INT");
        }
    }

    void StartConstantVibration(int strength)
    {
        if (vibrator != null)
        {
            using (AndroidJavaClass vibrationEffectClass = new AndroidJavaClass("android.os.VibrationEffect"))
            {
                vibrationEffect = vibrationEffectClass.CallStatic<AndroidJavaObject>("createOneShot", strength, 255);
                vibrator.Call("vibrate", vibrationEffect);
            }
        }
    }

    void StopVibration()
    {
        if (vibrator != null)
        {
            vibrator.Call("cancel");
        }
    }

    public void DestroyGhost()
    {
        Debug.Log($"💀 {name} Destroyed.");
        StopVibration();
        Destroy(gameObject);
    }
}