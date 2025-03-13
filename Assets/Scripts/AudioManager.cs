using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }

    [Header("Sound Effects")]
    [SerializeField] private AudioClip[] soundEffects;

    private AudioSource audioSource;

    private void Awake()
    {
        // Singleton pattern
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }
    }

    public void PlaySound(int soundIndex)
    {
        if (soundIndex < 0 || soundIndex >= soundEffects.Length)
        {
            Debug.LogWarning("Sound index out of range!");
            return;
        }

        audioSource.PlayOneShot(soundEffects[soundIndex]);
    }

    public void PlaySound(AudioClip clip)
    {
        if (clip == null)
        {
            Debug.LogWarning("No audio clip provided!");
            return;
        }

        audioSource.PlayOneShot(clip);
    }

    public void PlaySound(string soundName)
    {
        if (audioSource == null)
        {
            Debug.LogError("AudioSource not initialized!");
            return;
        }

        foreach (AudioClip clip in soundEffects)
        {
            if (clip.name == soundName)
            {
                audioSource.PlayOneShot(clip);
                return;
            }
        }
        Debug.LogWarning($"Sound '{soundName}' not found!");
    }

    private void OnDestroy()
    {
        if (Instance == this)
        {
            Instance = null;
        }
    }
}