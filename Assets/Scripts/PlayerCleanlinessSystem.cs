using UnityEngine;
using UnityEngine.UI;

public class PlayerDirtSystem : MonoBehaviour
{
    [Header("Dirt Settings")]
    [SerializeField] private SpriteRenderer dirtImage;
    [SerializeField] private float maxDirtAlpha = 0.8f;

    [Header("Microphone Settings")]
    [SerializeField] private float blowThreshold = 0.1f;
    [SerializeField] private float blowDuration = 3f;
    [SerializeField] private float cleanlinessPerSecond = 10f;

    //[Header("UI")]
    //[SerializeField] private Image cleaningProgressBar;

    private AudioClip microphoneInput;
    private bool isMicrophoneConnected;
    private float cleaningTimer;
    private int sampleWindow = 128;
    private Color originalDirtColor;

    private void Start()
    {
        Debug.Log("[DirtSystem] Initializing dirt system...");
        InitializeDirt();
        InitializeMicrophone();
    }

    void InitializeDirt()
    {
        if (dirtImage == null)
        {
            Debug.LogError("[DirtSystem] Missing dirt image reference!");
            return;
        }

        originalDirtColor = dirtImage.color;
        UpdateDirtAlpha();
        Debug.Log($"[DirtSystem] Initial dirt alpha: {originalDirtColor.a}");
    }

    void InitializeMicrophone()
    {
        if (Microphone.devices.Length > 0)
        {
            isMicrophoneConnected = true;
            microphoneInput = Microphone.Start(null, true, 1, AudioSettings.outputSampleRate);
            Debug.Log("[DirtSystem] Microphone initialized successfully");
        }
        else
        {
            Debug.LogWarning("[DirtSystem] No microphone detected - cleaning disabled");
        }
    }

    bool IsBlowingDetected()
    {
        return GetMicrophoneLoudness() > blowThreshold;
    }

    void CleanPlayer()
    {
        int currentCleanliness = SaveManager.Instance.GetCleanliness();
        if (cleaningTimer >= blowDuration)
        {
            currentCleanliness += Mathf.RoundToInt(cleanlinessPerSecond * Time.deltaTime);
            currentCleanliness = Mathf.Clamp(currentCleanliness, 0, 100);
            SaveManager.Instance.SetCleanliness(currentCleanliness);

            Debug.Log($"[DirtSystem] Cleaning progress: {currentCleanliness}%");

            if (currentCleanliness >= 100)
            {
                Debug.Log("[DirtSystem] Player fully cleaned!");
            }
        }
    }


    void UpdateDirtAlpha()
    {
        float cleanliness = SaveManager.Instance.GetCleanliness();
        float alpha = Mathf.Lerp(maxDirtAlpha, 0, cleanliness / 100f);
        dirtImage.color = new Color(originalDirtColor.r, originalDirtColor.g, originalDirtColor.b, alpha);
    }

    float GetMicrophoneLoudness()
    {
        float[] waveData = new float[sampleWindow];
        int micPosition = Microphone.GetPosition(null) - (sampleWindow + 1);

        if (micPosition < 0 || microphoneInput == null) return 0;

        microphoneInput.GetData(waveData, micPosition);
        float peak = 0;
        foreach (var sample in waveData) peak = Mathf.Max(peak, Mathf.Abs(sample));
        return peak;
    }

    private void Update()
    {
        if (isMicrophoneConnected)
        {
            float loudness = GetMicrophoneLoudness();
            Debug.Log($"[DirtSystem] Current mic loudness: {loudness.ToString("F2")}");

            if (IsBlowingDetected())
            {
                cleaningTimer += Time.deltaTime;
                CleanPlayer();
            }
            else
            {
                cleaningTimer = Mathf.Max(0, cleaningTimer - Time.deltaTime);
            }
        }

        UpdateDirtAlpha();
    }

    //void UpdateProgressUI()
    //{
    //    if (cleaningProgressBar != null)
    //    {
    //        cleaningProgressBar.fillAmount = cleaningTimer / blowDuration;
    //        cleaningProgressBar.color = Color.Lerp(Color.red, Color.green, cleaningTimer / blowDuration);
    //    }
    //}

    private void OnDestroy()
    {
        if (isMicrophoneConnected) Microphone.End(null);
    }

    private void OnApplicationPause(bool pauseStatus)
    {
        if (isMicrophoneConnected)
        {
            if (pauseStatus) Microphone.End(null);
            else microphoneInput = Microphone.Start(null, true, 1, AudioSettings.outputSampleRate);
        }
    }
}