using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameOverHandler : MonoBehaviour
{
    public static GameOverHandler Instance;

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
    }

    [Header("UI References")]
    public GameObject panel;
    public TMP_Text contextText;
    public TMP_Text energyLostText;
    public TMP_Text sanityGainedText;
    public TMP_Text hungerLostText;
    public TMP_Text cleanlinessLostText;
    public TMP_Text scoreText;
    public GameObject newHighScoreAnnouncement;

    [Header("Component References")]
    public GameObject flashlight;
    public GameObject handPointer;

    [Header("Scene Settings")]
    public string hubSceneName = "Aleksaner's Scene";

    [Header("Gameplay Settings")]
    public float hungerModulo = 7f;
    public float cleanlinessModulo = 5f;
    public float sanityMultiplier = 1.5f;
    public int energyDeduction = 3;

    private float startTime;
    private bool gameActive = true;

    void Start()
    {
        startTime = Time.time;
        panel.SetActive(false);
    }

    public void ShowResults()
    {
        if (flashlight) flashlight.SetActive(false);
        if (handPointer) handPointer.SetActive(false);

        GhostSpawnHandler spawnHandler = FindObjectOfType<GhostSpawnHandler>();
        if (spawnHandler == null)
        {
            Debug.LogError("No GhostSpawnHandler found!");
            return;
        }

        int finalScore = spawnHandler.GetCurrentScore();
        float timeElapsed = spawnHandler.GetPlayTime();
        bool isNewHighScore = finalScore > SaveManager.Instance.GetMinigame2HighScore();

        int hungerLoss = Mathf.RoundToInt(timeElapsed % hungerModulo);
        int cleanlinessLoss = Mathf.RoundToInt(timeElapsed % cleanlinessModulo);
        int sanityGain = Mathf.RoundToInt(timeElapsed * sanityMultiplier);
        int energyLoss = energyDeduction;

        SaveManager.Instance.SetHunger(SaveManager.Instance.GetHunger() - hungerLoss);
        SaveManager.Instance.SetCleanliness(SaveManager.Instance.GetCleanliness() - cleanlinessLoss);
        SaveManager.Instance.SetSanity(SaveManager.Instance.GetSanity() + sanityGain);
        SaveManager.Instance.SetEnergy(SaveManager.Instance.GetEnergy() - energyLoss);

        energyLostText.text = $"-{energyLoss}";
        hungerLostText.text = $"-{hungerLoss}%";
        cleanlinessLostText.text = $"-{cleanlinessLoss}%";
        sanityGainedText.text = $"+{sanityGain}%";
        scoreText.text = finalScore.ToString();
        newHighScoreAnnouncement.SetActive(isNewHighScore);

        if (isNewHighScore)
        {
            SaveManager.Instance.SetMinigame2HighScore(finalScore);
        }

        panel.SetActive(true);
        Debug.Log("Results panel activated");
    }

    public void ReturnToHub()
    {
        SceneManager.LoadScene(hubSceneName);
    }
}