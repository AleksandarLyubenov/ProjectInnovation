using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using UnityEngine.UI;
using System.Collections;

public class DoorTrigger : MonoBehaviour
{
    public GameObject popupPanel;
    public RectTransform popupTransform;
    public Button yesButton;
    public TextMeshProUGUI yesButtonText; // Reference to the TMP text component
    public Button noButton;
    public string minigameSceneName;

    [Header("Energy Settings")]
    [SerializeField] private int energyCost = 3;
    [SerializeField] private Color disabledColor = Color.gray;

    private Vector2 offScreenPos;
    private Vector2 onScreenPos;
    private Color normalColor;

    private void Start()
    {
        offScreenPos = new Vector2(popupTransform.anchoredPosition.x, -Screen.height);
        onScreenPos = new Vector2(popupTransform.anchoredPosition.x, 0);

        popupTransform.anchoredPosition = offScreenPos;
        popupPanel.SetActive(false);

        // Store original color
        normalColor = yesButtonText.color;

        yesButton.onClick.AddListener(StartMinigame);
        noButton.onClick.AddListener(ClosePopup);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            ShowPopup();
        }
    }

    void ShowPopup()
    {
        popupPanel.SetActive(true);
        StartCoroutine(AnimatePanel(onScreenPos));

        // Check conditions
        bool hasEnergy = SaveManager.Instance.GetEnergy() >= energyCost;
        bool hasSanity = SaveManager.Instance.GetSanity() > 0;

        // Update button state
        yesButton.interactable = hasEnergy && hasSanity;
        yesButtonText.color = yesButton.interactable ? normalColor : disabledColor;
    }

    void ClosePopup()
    {
        StartCoroutine(AnimatePanel(offScreenPos, () => {
            popupPanel.SetActive(false);
            // Reset button appearance
            yesButton.interactable = true;
            yesButtonText.color = normalColor;
        }));
    }

    void StartMinigame()
    {
        // Final check in case button was clicked while disabled
        if (SaveManager.Instance.GetEnergy() >= energyCost &&
            SaveManager.Instance.GetSanity() > 0)
        {
            SaveManager.Instance.SetEnergy(SaveManager.Instance.GetEnergy() - energyCost);
            SceneManager.LoadScene(minigameSceneName);
        }
    }

    IEnumerator AnimatePanel(Vector2 targetPos, System.Action onComplete = null)
    {
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