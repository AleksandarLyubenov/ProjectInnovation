using UnityEngine;
using UnityEngine.UI;

public class MinigameTrigger : MonoBehaviour
{
    [SerializeField] private Button minigameButton;
    [SerializeField] private CirclesManager circlesManager;

    void Start()
    {
        // Add the listener properly
        minigameButton.onClick.AddListener(StartMinigame);
    }

    void Update()
    {
        minigameButton.interactable = ShouldButtonBeInteractable();
    }

    bool ShouldButtonBeInteractable()
    {
        return !circlesManager.IsMinigameActive &&
               SaveManager.Instance.GetEnergy() >= 3 &&
               SaveManager.Instance.GetSanity() > 0;
    }

    void StartMinigame()
    {
        if (ShouldButtonBeInteractable())
        {
            SaveManager.Instance.SetEnergy(SaveManager.Instance.GetEnergy() - 3);
            circlesManager.StartMinigame();
        }
    }
}