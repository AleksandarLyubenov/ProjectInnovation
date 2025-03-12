using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.SceneManagement;

public class StatsUIHandler : MonoBehaviour
{
    [SerializeField] private GameObject statsPanel;
    [SerializeField] private GameObject sidePanel;

    [SerializeField] private TMP_Text levelText;
    [SerializeField] private TMP_Text energyText;
    [SerializeField] private TMP_Text cleanlinessText;
    [SerializeField] private TMP_Text hungerText;
    [SerializeField] private TMP_Text sanityText;

    [SerializeField] private Image cleanlinessBar;
    [SerializeField] private Image hungerBar;
    [SerializeField] private Image sanityBar;

    private int maxEnergy;

    private void Start()
    {
        statsPanel.SetActive(false);
        sidePanel.SetActive(false);
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        UpdateAllStats();
    }

    private void OnEnable()
    {
        if (PlayerPresenceNotifier.Instance != null)
        {
            PlayerPresenceNotifier.Instance.OnPlayerFound.AddListener(OnPlayerFound);
            PlayerPresenceNotifier.Instance.OnPlayerLost.AddListener(OnPlayerLost);
        }
        StartCoroutine(WaitForNotifier());

        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private IEnumerator WaitForNotifier()
    {
        while (PlayerPresenceNotifier.Instance == null)
        {
            yield return null;
        }

        PlayerPresenceNotifier.Instance.OnPlayerFound.AddListener(OnPlayerFound);
        PlayerPresenceNotifier.Instance.OnPlayerLost.AddListener(OnPlayerLost);
        UpdateUIState();
    }

    private void OnDisable()
    {
        PlayerPresenceNotifier.Instance.OnPlayerFound.RemoveListener(OnPlayerFound);
        PlayerPresenceNotifier.Instance.OnPlayerLost.RemoveListener(OnPlayerLost);

        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnPlayerFound()
    {
        UpdateUIState();
        SubscribeToEvents();
        UpdateAllStats();
    }

    private void OnPlayerLost()
    {
        UpdateUIState();
        UnsubscribeFromEvents();
    }

    private void UpdateUIState()
    {
        statsPanel.SetActive(PlayerPresenceNotifier.Instance.HasPlayer());
        sidePanel.SetActive(PlayerPresenceNotifier.Instance.HasPlayer());
    }

    private void SubscribeToEvents()
    {
        SaveManager.Instance.OnSanityChanged += UpdateSanity;
        SaveManager.Instance.OnEnergyChanged += UpdateEnergy;
        SaveManager.Instance.OnCleanlinessChanged += UpdateCleanliness;
        SaveManager.Instance.OnHungerChanged += UpdateHunger;
    }

    private void UpdateAllStats()
    {
        UpdateLevel();
        UpdateEnergy();
        UpdateCleanliness();
        UpdateHunger();
        UpdateSanity();
    }

    private void UpdateLevel()
    {
        int level = SaveManager.Instance.GetPlayerLevel();
        levelText.text = $"Level {level}";
    }

    private void UpdateEnergy()
    {
        maxEnergy = CalculateMaxEnergy();
        int currentEnergy = SaveManager.Instance.GetEnergy();
        energyText.text = $"{currentEnergy}/{maxEnergy}";
    }

    private void UpdateCleanliness()
    {
        int cleanliness = SaveManager.Instance.GetCleanliness();
        if (cleanlinessText != null) cleanlinessText.text = $"{cleanliness}%";
        if (cleanlinessBar != null) cleanlinessBar.fillAmount = cleanliness / 100f;
    }

    private void UpdateHunger()
    {
        int hunger = SaveManager.Instance.GetHunger();
        if (hungerText != null) hungerText.text = $"{hunger}%";
        if (hungerBar != null) hungerBar.fillAmount = hunger / 100f;
    }

    private void UpdateSanity()
    {
        int sanity = SaveManager.Instance.GetSanity();
        if (sanityText != null) sanityText.text = $"{sanity}%";
        if (sanityBar != null) sanityBar.fillAmount = sanity / 100f;
    }

    private int CalculateMaxEnergy()
    {
        float cleanlinessFactor = SaveManager.Instance.GetCleanliness() * 0.1f;
        float hungerFactor = SaveManager.Instance.GetHunger() * 0.1f;
        float sanityFactor = SaveManager.Instance.GetSanity() * 0.1f;

        return Mathf.RoundToInt(30 + cleanlinessFactor + hungerFactor + sanityFactor);
    }

    private void OnDestroy()
    {
        // Safely unsubscribe even if PlayerPresenceNotifier is destroyed
        if (PlayerPresenceNotifier.Instance != null)
        {
            PlayerPresenceNotifier.Instance.OnPlayerFound.RemoveListener(OnPlayerFound);
            PlayerPresenceNotifier.Instance.OnPlayerLost.RemoveListener(OnPlayerLost);
        }
        UnsubscribeFromEvents();
    }

    private void UnsubscribeFromEvents()
    {
        if (SaveManager.Instance != null)
        {
            SaveManager.Instance.OnSanityChanged -= UpdateSanity;
            SaveManager.Instance.OnEnergyChanged -= UpdateEnergy;
            SaveManager.Instance.OnCleanlinessChanged -= UpdateCleanliness;
            SaveManager.Instance.OnHungerChanged -= UpdateHunger;
        }
    }
}
