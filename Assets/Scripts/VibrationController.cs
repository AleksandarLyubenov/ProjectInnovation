using UnityEngine;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using System.Linq;

public class VibrationManager : MonoBehaviour
{
    public static VibrationManager Instance;

    private List<GhostTransparencyController> activeGhosts = new List<GhostTransparencyController>();
    private AndroidJavaObject vibrator;
    private bool hasVibratePermission;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        SceneManager.sceneLoaded += OnSceneLoaded;
        InitializeVibrator();
    }
    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // Clear ghosts when new scene loads
        activeGhosts.Clear();
        StopVibration();
    }

    void InitializeVibrator()
    {
        // Add null check for Android initialization
        if (Application.platform != RuntimePlatform.Android) return;

        try
        {
            using (AndroidJavaClass unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer"))
            {
                AndroidJavaObject activity = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity");
                vibrator = activity.Call<AndroidJavaObject>("getSystemService", "vibrator");
                hasVibratePermission = CheckVibratePermission();
            }
        }
        catch (System.Exception e)
        {
            Debug.LogError("Vibrator initialization failed: " + e.Message);
        }
    }

    bool CheckVibratePermission()
    {
        using (AndroidJavaClass contextClass = new AndroidJavaClass("android.content.Context"))
        using (AndroidJavaObject activity = new AndroidJavaClass("com.unity3d.player.UnityPlayer").GetStatic<AndroidJavaObject>("currentActivity"))
        {
            int permissionResult = activity.Call<int>("checkSelfPermission", "android.permission.VIBRATE");
            return permissionResult == 0;
        }
    }

    public void RegisterActiveGhost(GhostTransparencyController ghost)
    {
        if (!activeGhosts.Contains(ghost))
            activeGhosts.Add(ghost);
    }

    public void UnregisterActiveGhost(GhostTransparencyController ghost)
    {
        if (activeGhosts.Contains(ghost))
            activeGhosts.Remove(ghost);
    }

    void Update()
    {
        if (!hasVibratePermission || vibrator == null) return;

        GhostTransparencyController dominantGhost = GetDominantGhost();

        if (dominantGhost != null)
        {
            float vibrationStrength = Mathf.Lerp(50, 255, dominantGhost.currentAlpha);
            if (dominantGhost.isPassive)
                StartPulsatingVibration(vibrationStrength);
            else
                StartConstantVibration((int)vibrationStrength);
        }
        else
        {
            StopVibration();
        }
    }

    GhostTransparencyController GetDominantGhost()
    {
        GhostTransparencyController dominant = null;
        float maxAlpha = 0.5f;
        bool hasEnemy = activeGhosts.Exists(g => !g.isPassive);

        // Debug: Log all active ghosts
        Debug.Log($"Active ghosts: {activeGhosts.Count} (Enemies: {activeGhosts.Count(g => !g.isPassive)})");

        foreach (var ghost in activeGhosts)
        {
            // Debug individual ghost status
            Debug.Log($"Ghost: {ghost.name} | Passive: {ghost.isPassive} | Alpha: {ghost.currentAlpha}");

            if (hasEnemy && ghost.isPassive) continue;

            if (ghost.currentAlpha > maxAlpha)
            {
                maxAlpha = ghost.currentAlpha;
                dominant = ghost;
            }
        }

        Debug.Log($"Dominant ghost: {(dominant != null ? dominant.name : "None")}");
        return dominant;
    }

    void StartPulsatingVibration(float strength)
    {
        long vibrateDuration = 100;
        long pauseDuration = (long)Mathf.Lerp(300, 50, (strength - 50) / 205f);
        long[] pattern = { 0, vibrateDuration, pauseDuration };

        if (GetAndroidSDKVersion() >= 26)
        {
            using (AndroidJavaClass vibrationEffectClass = new AndroidJavaClass("android.os.VibrationEffect"))
            {
                AndroidJavaObject effect = vibrationEffectClass.CallStatic<AndroidJavaObject>(
                    "createWaveform", pattern, -1);
                vibrator.Call("vibrate", effect);
            }
        }
        else
        {
            vibrator.Call("vibrate", pattern, -1);
        }
    }

    void StartConstantVibration(int amplitude)
    {
        const long DURATION = long.MaxValue;
        amplitude = Mathf.Clamp(amplitude, 1, 255);

        if (GetAndroidSDKVersion() >= 26)
        {
            try
            {
                using (AndroidJavaClass vibrationEffectClass = new AndroidJavaClass("android.os.VibrationEffect"))
                {
                    object[] parameters = new object[2];
                    parameters[0] = (long)DURATION;
                    parameters[1] = (int)amplitude;

                    AndroidJavaObject effect = vibrationEffectClass.CallStatic<AndroidJavaObject>(
                        "createOneShot",
                        parameters
                    );
                    vibrator.Call("vibrate", effect);
                }
            }
            catch (System.Exception e)
            {
                Debug.LogError($"Vibration failed: {e.Message}");
            }
        }
        else
        {
            vibrator.Call("vibrate", (long)DURATION);
        }
    }

    int GetAndroidSDKVersion()
    {
        using (AndroidJavaClass buildVersion = new AndroidJavaClass("android.os.Build$VERSION"))
        {
            return buildVersion.GetStatic<int>("SDK_INT");
        }
    }

    public void StopVibration()
    {
        if (vibrator != null)
            vibrator.Call("cancel");
    }
}