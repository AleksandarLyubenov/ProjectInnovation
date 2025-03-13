using UnityEngine;
using TMPro;

public class LevelUpHandler : MonoBehaviour
{
    public TextMeshProUGUI levelText;

    private void Start()
    {
        UpdateLevelDisplay();
        SaveManager.Instance.OnLevelUp += HandleLevelUp;
    }

    private void OnDestroy()
    {
        if (SaveManager.Instance != null)
        {
            SaveManager.Instance.OnLevelUp -= HandleLevelUp;
        }
    }

    private void UpdateLevelDisplay()
    {
        if (levelText != null)
        {
            levelText.text = "Level: " + SaveManager.Instance.GetPlayerLevel();
        }
    }

    private void HandleLevelUp()
    {
        UpdateLevelDisplay();
    }
}