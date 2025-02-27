using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class ClosetTrigger : MonoBehaviour
{
    // Add this field
    [Header("Unlockables Tracking")]
    [SerializeField] private UnlockablesTracker unlockablesTracker;

    [Header("UI Elements")]
    public GameObject closetPanel; // Assign in Inspector
    public RectTransform closetTransform; // UI Animation
    public Button outfit1Button, outfit2Button, closeButton;

    private Vector2 offScreenPos, onScreenPos;
    private GameObject player, outfit1, outfit2;

    private void Start()
    {
        offScreenPos = new Vector2(closetTransform.anchoredPosition.x, -Screen.height);
        onScreenPos = new Vector2(closetTransform.anchoredPosition.x, 0);
        closetTransform.anchoredPosition = offScreenPos;
        closetPanel.SetActive(false);

        outfit1Button.onClick.AddListener(SelectOutfit1);
        outfit2Button.onClick.AddListener(SelectOutfit2);
        closeButton.onClick.AddListener(CloseCloset);

        // Initialize outfit 2 button state
        UpdateOutfitButtonStates();
    }

    //private void Update() { if (player == null) FindPlayer(); }

    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent<PlayerMovement>(out PlayerMovement playerMovement))
        {
            if (playerMovement.isSelected) // Ensure it's the selected player
            {
                Debug.Log("Closet triggered by selected player!");

                // Deselect the player to allow UI interaction
                playerMovement.isSelected = false;
                playerMovement.outline.SetActive(false);

                // Assign player and outfits
                player = other.gameObject;
                outfit1 = player.transform.Find("Canvas/Outfit 1")?.gameObject;
                outfit2 = player.transform.Find("Canvas/Outfit 2")?.gameObject;

                foreach (Transform child in player.GetComponentsInChildren<Transform>())
                {
                    if (child.name.Contains("Canvas/Outfit 1")) outfit1 = child.gameObject;
                    if (child.name.Contains("Canvas/Outfit 2")) outfit2 = child.gameObject;
                }

                if (outfit1 == null) Debug.LogError("Outfit 1 NOT FOUND!");
                if (outfit2 == null) Debug.LogError("Outfit 2 NOT FOUND!");

                Debug.Log($"Outfit 1: {outfit1}, Outfit 2: {outfit2}");

                ShowCloset();
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject == player)
        {
            Debug.Log("Player exited closet trigger. Closing UI.");
            CloseCloset();
            player = null; // Reset player reference
        }
    }

    private void ShowCloset()
    {
        // Check unlock status when opening closet
        UpdateOutfitButtonStates();
        closetPanel.SetActive(true);
        StartCoroutine(AnimatePanel(onScreenPos));
    }

    private void UpdateOutfitButtonStates()
    {
        if (unlockablesTracker != null)
        {
            // Assuming "Outfit 2" is the name used in your UnlockablesTracker
            outfit2Button.interactable = unlockablesTracker.IsUnlocked("Outfit 2");
        }
        else
        {
            Debug.LogError("UnlockablesTracker reference not set in ClosetTrigger!");
            outfit2Button.interactable = false; // Fail-safe
        }
    }

    void CloseCloset()
    {
        StartCoroutine(AnimatePanel(offScreenPos, () => closetPanel.SetActive(false)));
    }

    void FindPlayer()
    {
        PlayerMovement[] players = FindObjectsOfType<PlayerMovement>();

        foreach (var p in players)
        {
            Debug.Log($"Checking Player: {p.gameObject.name}, isSelected: {p.isSelected}");

            if (p.isSelected)
            {
                player = p.gameObject;
                outfit1 = player.transform.Find("Outfit 1")?.gameObject;
                outfit2 = player.transform.Find("Outfit 2")?.gameObject;
                Debug.Log("Player found and assigned!");
                return;
            }
        }

        Debug.LogError("No selected player found!");
    }

    void SelectOutfit1()
    {
        if (outfit1 && outfit2)
        {
            outfit1.SetActive(false); // Reset both
            outfit2.SetActive(false);

            outfit1.SetActive(true); // Activate the selected one
            Debug.Log("Outfit 1 selected");
        }
    }

    void SelectOutfit2()
    {
        if (outfit1 && outfit2)
        {
            outfit1.SetActive(false); // Reset both
            outfit2.SetActive(false);

            outfit2.SetActive(true); // Activate the selected one
            Debug.Log("Outfit 2 selected");
        }
    }


    IEnumerator AnimatePanel(Vector2 targetPos, System.Action onComplete = null)
    {
        float duration = 0.3f, elapsed = 0f;
        Vector2 startPos = closetTransform.anchoredPosition;
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            closetTransform.anchoredPosition = Vector2.Lerp(startPos, targetPos, elapsed / duration);
            yield return null;
        }
        closetTransform.anchoredPosition = targetPos;
        onComplete?.Invoke();
    }
}
