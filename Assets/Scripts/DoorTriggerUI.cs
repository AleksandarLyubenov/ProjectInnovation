using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections;

public class DoorTrigger : MonoBehaviour
{
    [Header("UI Elements")]
    public GameObject popupPanel;
    public RectTransform popupTransform; // Must be assigned in Inspector
    public Button game1Button;
    public Button game2Button;
    public Button cancelButton;
    public GameObject game1LockImage;
    public GameObject game2LockImage;

    [Header("Minigame Settings")]
    public string game1SceneName;
    public string game2SceneName;

    [Header("Energy Settings")]
    [SerializeField] private int energyCost = 3;

    private Vector2 offScreenPos;
    private Vector2 onScreenPos;

    private AudioManager audioManager;

    private void Start()
    {
        audioManager = FindAnyObjectByType<AudioManager>();

        if (popupTransform == null)
        {
            Debug.LogError("Popup Transform not assigned in DoorTrigger!");
            enabled = false;
            return;
        }

        // Initialize positions
        offScreenPos = new Vector2(popupTransform.anchoredPosition.x, -Screen.height);
        onScreenPos = popupTransform.anchoredPosition;

        // Set initial state
        popupTransform.anchoredPosition = offScreenPos;
        if (popupPanel != null) popupPanel.SetActive(false);

        // Setup button listeners
        game1Button.onClick.AddListener(() => StartMinigame(game1SceneName));
        game2Button.onClick.AddListener(() => StartMinigame(game2SceneName));
        cancelButton.onClick.AddListener(ClosePopup);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerMovement playerMovement = other.GetComponent<PlayerMovement>();
            if (playerMovement != null)
            {
                playerMovement.Unselect();
            }
            ShowPopup();
        }
    }

    void ShowPopup()
    {
        if (popupPanel == null || popupTransform == null) return;

        popupPanel.SetActive(true);
        StartCoroutine(AnimatePanel(onScreenPos));

        audioManager.PlaySound("Popup");

        UpdateButtonStates();
    }

    void UpdateButtonStates()
    {
        bool hasEnergy = SaveManager.Instance.GetEnergy() >= energyCost;
        bool hasSanity = SaveManager.Instance.GetSanity() > 0;
        bool canPlay = hasEnergy && hasSanity;

        UpdateButtonState(game1Button, game1LockImage, canPlay);
        UpdateButtonState(game2Button, game2LockImage, canPlay);
    }

    void ClosePopup()
    {
        if (popupPanel == null || popupTransform == null) return;

        StartCoroutine(AnimatePanel(offScreenPos, () => {
            popupPanel.SetActive(false);
        }));
    }

    void StartMinigame(string sceneName)
    {
        if (SaveManager.Instance.GetEnergy() >= energyCost &&
            SaveManager.Instance.GetSanity() > 0)
        {
            SaveManager.Instance.SetEnergy(SaveManager.Instance.GetEnergy() - energyCost);
            SceneManager.LoadScene(sceneName);
        }
    }

    private void UpdateButtonState(Button button, GameObject lockOverlay, bool enabled)
    {
        if (button == null || lockOverlay == null) return;

        button.interactable = enabled;
        lockOverlay.SetActive(!enabled);
    }

    IEnumerator AnimatePanel(Vector2 targetPos, System.Action onComplete = null)
    {
        if (popupTransform == null) yield break;

        float duration = 0.3f;
        float elapsed = 0f;
        Vector2 startPos = popupTransform.anchoredPosition;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            popupTransform.anchoredPosition = Vector2.Lerp(startPos, targetPos, elapsed / duration);
            yield return null;
        }

        popupTransform.anchoredPosition = targetPos;
        onComplete?.Invoke();
    }
}