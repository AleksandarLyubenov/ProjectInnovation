using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class ClosetTrigger : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private GameObject mainUI;
    [SerializeField] private RectTransform closetPanel;
    [SerializeField] private Button closetButton;
    [SerializeField] private Button closeButton;
    [SerializeField] private Button outfit1Button;
    [SerializeField] private Button outfit2Button;

    [Header("Player Settings")]
    [SerializeField] private string playerTag = "Player";
    [SerializeField] private string outfit1Path = "Canvas/Outfit 1";
    [SerializeField] private string outfit2Path = "Canvas/Outfit 2";

    private GameObject player;
    private GameObject outfit1Object;
    private GameObject outfit2Object;

    private Vector2 onScreenPos;
    private Vector2 offScreenPos;
    private bool isAnimating;
    private bool isOpen;

    private void Start()
    {
        FindPlayer();
        InitializePositions();
        SetupButtonListeners();
        UpdateOutfitDisplay();
    }

    void InitializePositions()
    {
        onScreenPos = closetPanel.anchoredPosition;
        offScreenPos = new Vector2(Screen.width, onScreenPos.y);
        closetPanel.anchoredPosition = offScreenPos;
        closetPanel.gameObject.SetActive(false);
    }

    void SetupButtonListeners()
    {
        closetButton.onClick.AddListener(ToggleCloset);
        closeButton.onClick.AddListener(ToggleCloset);
        outfit1Button.onClick.AddListener(() => SelectOutfit(1));
        outfit2Button.onClick.AddListener(() => SelectOutfit(2));
    }

    public void ToggleCloset()
    {
        if (isAnimating) return;

        if (!isOpen) OpenCloset();
        else CloseCloset();
    }

    void OpenCloset()
    {
        isOpen = true;
        mainUI.SetActive(false);
        closetPanel.gameObject.SetActive(true);
        StartCoroutine(AnimatePanel(offScreenPos, onScreenPos));
    }

    void CloseCloset()
    {
        isOpen = false;
        StartCoroutine(AnimatePanel(onScreenPos, offScreenPos, () => {
            closetPanel.gameObject.SetActive(false);
            mainUI.SetActive(true);
        }));
    }

    public void RefreshOutfitDisplay()
    {
        Debug.Log("[CLOSET] Refreshing outfit display");
        bool isUnlocked = SaveManager.Instance.IsOutfitUnlocked("Outfit 2");
        Debug.Log($"[CLOSET] Outfit 2 unlocked status: {isUnlocked}");

        // Update UI immediately
        outfit2Button.gameObject.SetActive(isUnlocked);

        // Force UI layout rebuild
        LayoutRebuilder.ForceRebuildLayoutImmediate(
            closetPanel.GetComponent<RectTransform>()
        );
    }

    void UpdateOutfitDisplay()
    {
        outfit2Button.gameObject.SetActive(SaveManager.Instance.IsOutfitUnlocked("Outfit 2"));
    }

    void FindPlayer()
    {
        player = GameObject.FindGameObjectWithTag(playerTag);

        if (player != null)
        {
            outfit1Object = player.transform.Find(outfit1Path)?.gameObject;
            outfit2Object = player.transform.Find(outfit2Path)?.gameObject;

            if (outfit1Object == null || outfit2Object == null)
            {
                Debug.LogError("Could not find outfit objects on player!");
                Debug.Log($"Player found: {player != null}");
                Debug.Log($"Outfit1 exists: {outfit1Object != null}");
                Debug.Log($"Outfit2 exists: {outfit2Object != null}");
            }
        }
        else
        {
            Debug.Log("Player not found with tag: " + playerTag);
            StartCoroutine(RetryFindPlayer());
        }
    }

    IEnumerator RetryFindPlayer()
    {
        while (player == null)
        {
            yield return new WaitForSeconds(0.5f);
            player = GameObject.FindGameObjectWithTag(playerTag);
        }
        FindPlayer(); // Recursively setup outfits once player is found
    }

    void SelectOutfit(int outfitNumber)
    {
        if (outfit1Object == null || outfit2Object == null)
        {
            Debug.LogWarning("Outfit objects not initialized!");
            return;
        }

        outfit1Object.SetActive(false);
        outfit2Object.SetActive(false);

        switch (outfitNumber)
        {
            case 1:
                outfit1Object.SetActive(true);
                Debug.Log("Activated Outfit 1");
                break;
            case 2:
                if (SaveManager.Instance.IsOutfitUnlocked("Outfit 2"))
                {
                    outfit2Object.SetActive(true);
                    Debug.Log("Activated Outfit 2");
                }
                break;
        }
    }

    IEnumerator AnimatePanel(Vector2 startPos, Vector2 endPos, System.Action onComplete = null)
    {
        isAnimating = true;
        float duration = 0.5f;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            closetPanel.anchoredPosition = Vector2.Lerp(startPos, endPos, elapsed / duration);
            yield return null;
        }

        closetPanel.anchoredPosition = endPos;
        isAnimating = false;
        onComplete?.Invoke();

        // Force UI update after animation
        RefreshOutfitDisplay();
    }
}