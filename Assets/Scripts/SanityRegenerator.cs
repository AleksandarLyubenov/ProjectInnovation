using UnityEngine;
using System.Collections;

public class SanityRegenerator : MonoBehaviour
{
    [Header("Regeneration Settings")]
    [SerializeField] private float baseRegenInterval = 15f;
    [SerializeField] private int baseSanityGain = 1;
    [SerializeField] private float statWeightMultiplier = 0.03f;

    private void Start()
    {
        StartCoroutine(SanityRegenerationRoutine());
    }

    IEnumerator SanityRegenerationRoutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(baseRegenInterval);
            RegenerateSanity();
        }
    }

    private void RegenerateSanity()
    {
        float cleanlinessFactor = SaveManager.Instance.GetCleanliness() * statWeightMultiplier;
        float hungerFactor = SaveManager.Instance.GetHunger() * statWeightMultiplier;
        float sanityFactor = SaveManager.Instance.GetSanity() * statWeightMultiplier;

        int totalGain = Mathf.RoundToInt(baseSanityGain + cleanlinessFactor + hungerFactor + sanityFactor);
        SaveManager.Instance.SetEnergy(SaveManager.Instance.GetEnergy() + totalGain);
    }
}