using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class MinigameTrigger : MonoBehaviour
{
    [SerializeField] private Button minigameButton;
    [SerializeField] private GameObject buttonImage;
    [SerializeField] private CirclesManager circlesManager;

    private void OnEnable()
    {
        PlayerPresenceNotifier.Instance.OnPlayerFound.AddListener(OnPlayerFound);
        PlayerPresenceNotifier.Instance.OnPlayerLost.AddListener(OnPlayerLost);
        StartCoroutine(InitializeWhenReady());
        UpdateButtonState();
    }

    private IEnumerator InitializeWhenReady()
    {
        while (PlayerPresenceNotifier.Instance == null)
        {
            yield return null;
        }

        PlayerPresenceNotifier.Instance.OnPlayerFound.AddListener(OnPlayerFound);
        PlayerPresenceNotifier.Instance.OnPlayerLost.AddListener(OnPlayerLost);
        UpdateButtonState();
    }

    private void OnDisable()
    {
        PlayerPresenceNotifier.Instance.OnPlayerFound.RemoveListener(OnPlayerFound);
        PlayerPresenceNotifier.Instance.OnPlayerLost.RemoveListener(OnPlayerLost);
    }

    private void OnPlayerFound() => UpdateButtonState();
    private void OnPlayerLost() => UpdateButtonState();


    void UpdateButtonState()
    {
        bool hasPlayer = PlayerPresenceNotifier.Instance.HasPlayer();

        // Control both interactability and visibility
        buttonImage.SetActive(hasPlayer);
        minigameButton.interactable = ShouldButtonBeInteractable();
    }

    bool ShouldButtonBeInteractable()
    {
        return PlayerPresenceNotifier.Instance.HasPlayer() &&
               !circlesManager.IsMinigameActive &&
               SaveManager.Instance.GetEnergy() >= 3 &&
               SaveManager.Instance.GetSanity() > 0;
    }

    void Start()
    {
        minigameButton.onClick.AddListener(StartMinigame);
        // Initial state update
        buttonImage.SetActive(false);
    }

    void Update()
    {
        if (PlayerPresenceNotifier.Instance.HasPlayer())
            UpdateButtonState();
    }

    void StartMinigame()
    {
        if (ShouldButtonBeInteractable())
        {
            SaveManager.Instance.SetEnergy(SaveManager.Instance.GetEnergy() - 3);

            PlayerMovement playerMovement = PlayerPresenceNotifier.Instance.GetPlayer()
                ?.GetComponent<PlayerMovement>();
            playerMovement?.Unselect();

            circlesManager.StartMinigame();
        }
    }
}