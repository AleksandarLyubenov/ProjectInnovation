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
        activeGhosts.Clear();
        StopVibration();
    }

    void InitializeVibrator()
    {
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

        activeGhosts.RemoveAll(g => g == null);

        var hostileGhosts = activeGhosts.Where(g => !g.isPassive).ToList();

        if (hostileGhosts.Count > 0)
        {
            var closestGhost = hostileGhosts.OrderByDescending(g => g.currentAlpha).First();
            float vibrationStrength = Mathf.Lerp(50, 255, closestGhost.currentAlpha);
            StartProximityVibration(vibrationStrength);
        }
        else
        {
            StopVibration();
        }
    }

    void StartProximityVibration(float strength)
    {
        int amplitude = Mathf.Clamp((int)strength, 1, 255);

        long vibrateDuration = 100;
        long pauseDuration = (long)Mathf.Lerp(500, 50, strength / 255f);
        long[] pattern = { 0, vibrateDuration, pauseDuration };

        if (GetAndroidSDKVersion() >= 26)
        {
            using (AndroidJavaClass vibrationEffectClass = new AndroidJavaClass("android.os.VibrationEffect"))
            {
                AndroidJavaObject effect = vibrationEffectClass.CallStatic<AndroidJavaObject>(
                    "createWaveform", pattern, new int[] { 0, amplitude, 0 }, -1
                );
                vibrator.Call("vibrate", effect);
            }
        }
        else
        {
            vibrator.Call("vibrate", pattern, -1);
        }
    }

    GhostTransparencyController GetDominantGhost()
    {
        return activeGhosts.FirstOrDefault(g => !g.isPassive);
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
                Debug.Log("Vibrating!");
            }
        }
        else
        {
            vibrator.Call("vibrate", pattern, -1);
            Debug.Log("Not vibrating!");
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
        try
        {
            if (vibrator != null)
                vibrator.Call("cancel");
        }
        catch (System.Exception e)
        {
            Debug.LogError("Vibration stop failed: " + e.Message);
        }
    }
}