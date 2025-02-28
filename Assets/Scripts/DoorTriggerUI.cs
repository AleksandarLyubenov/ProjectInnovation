using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections;

public class DoorTrigger : MonoBehaviour
{
    public GameObject popupPanel;
    public RectTransform popupTransform;
    public Button yesButton;
    public Button noButton;
    public string minigameSceneName;

    private Vector2 offScreenPos;
    private Vector2 onScreenPos;

    private void Start()
    {
        offScreenPos = new Vector2(popupTransform.anchoredPosition.x, -Screen.height);
        onScreenPos = new Vector2(popupTransform.anchoredPosition.x, 0);

        popupTransform.anchoredPosition = offScreenPos;
        popupPanel.SetActive(false);

        yesButton.onClick.AddListener(StartMinigame);
        noButton.onClick.AddListener(ClosePopup);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player")) // Make sure your player has the "Player" tag
        {
            ShowPopup();
        }
    }

    void ShowPopup()
    {
        popupPanel.SetActive(true);
        StartCoroutine(AnimatePanel(onScreenPos));
    }

    void ClosePopup()
    {
        StartCoroutine(AnimatePanel(offScreenPos, () => popupPanel.SetActive(false)));
    }

    void StartMinigame()
    {
        SceneManager.LoadScene(minigameSceneName);
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
