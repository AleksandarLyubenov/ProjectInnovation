using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class StatsUIHandler : MonoBehaviour
{
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
        UpdateAllStats();
        SubscribeToEvents();
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
        cleanlinessText.text = $"{cleanliness}%";
        cleanlinessBar.fillAmount = cleanliness / 100f;
    }

    private void UpdateHunger()
    {
        int hunger = SaveManager.Instance.GetHunger();
        hungerText.text = $"{hunger}%";
        hungerBar.fillAmount = hunger / 100f;
    }

    private void UpdateSanity()
    {
        int sanity = SaveManager.Instance.GetSanity();
        sanityText.text = $"{sanity}%";
        sanityBar.fillAmount = sanity / 100f;
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
        SaveManager.Instance.OnSanityChanged -= UpdateSanity;
        SaveManager.Instance.OnEnergyChanged -= UpdateEnergy;
        SaveManager.Instance.OnCleanlinessChanged -= UpdateCleanliness;
        SaveManager.Instance.OnHungerChanged -= UpdateHunger;
    }
}
