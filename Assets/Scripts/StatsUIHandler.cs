using TMPro;
using UnityEngine;

public class StatsUIHandler : MonoBehaviour
{
    [SerializeField] private TMP_Text energyText;
    [SerializeField] private TMP_Text cleanlinessText;
    [SerializeField] private TMP_Text hungerText;
    [SerializeField] private TMP_Text sanityText;

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
        UpdateEnergy();
        UpdateCleanliness();
        UpdateHunger();
        UpdateSanity();
    }

    private void UpdateEnergy() =>
        energyText.text = $"Energy: {SaveManager.Instance.GetEnergy()}";

    private void UpdateCleanliness() =>
        cleanlinessText.text = $"Cleanliness: {SaveManager.Instance.GetCleanliness()}";

    private void UpdateHunger() =>
        hungerText.text = $"Hunger: {SaveManager.Instance.GetHunger()}";

    private void UpdateSanity() =>
        sanityText.text = $"Sanity: {SaveManager.Instance.GetSanity()}";

    private void OnDestroy()
    {
        SaveManager.Instance.OnSanityChanged -= UpdateSanity;
        SaveManager.Instance.OnEnergyChanged -= UpdateEnergy;
        SaveManager.Instance.OnCleanlinessChanged -= UpdateCleanliness;
        SaveManager.Instance.OnHungerChanged -= UpdateHunger;
    }
}