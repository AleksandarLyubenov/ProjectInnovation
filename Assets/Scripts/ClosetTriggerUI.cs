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
    [SerializeField] private Button hat1Button;
    [SerializeField] private Button hat2Button;
    [SerializeField] private Button hat3Button;

    [Header("Object Paths (Player Hierarchy)")]
    [SerializeField] private string playerTag = "Player";
    [SerializeField] private string hat1Path = "Canvas/Hat_1";
    [SerializeField] private string hat2Path = "Canvas/Hat_2";
    [SerializeField] private string hat3Path = "Canvas/Hat_3";

    [Header("Cosmetic IDs (Save/Load)")]
    [SerializeField] private string hat1Id = "Hat_1";
    [SerializeField] private string hat2Id = "Hat_2";
    [SerializeField] private string hat3Id = "Hat_3";

    private GameObject player;

    private GameObject hat1Object;
    private GameObject hat2Object;
    private GameObject hat3Object;

    private Vector2 onScreenPos;
    private Vector2 offScreenPos;
    private bool isAnimating;
    private bool isOpen;

    private void Start()
    {
        FindPlayer();
        InitializePositions();
        SetupButtonListeners();
        UpdateCosmeticDisplay();
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
        hat1Button.onClick.AddListener(() => SelectHat(1));
        hat2Button.onClick.AddListener(() => SelectHat(2));
        hat3Button.onClick.AddListener(() => SelectHat(3));
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

    public void RefreshCosmeticDisplay()
    {
        hat2Button.gameObject.SetActive(SaveManager.Instance.IsCosmeticUnlocked(hat2Id));
        hat3Button.gameObject.SetActive(SaveManager.Instance.IsCosmeticUnlocked(hat3Id));
        LayoutRebuilder.ForceRebuildLayoutImmediate(closetPanel.GetComponent<RectTransform>());
    }

    void UpdateCosmeticDisplay()
    {
        Debug.Log($"Hat 2 Unlocked: {SaveManager.Instance.IsCosmeticUnlocked(hat2Id)}");
        Debug.Log($"Hat 2 Path: {hat2Path} | Object Found: {hat2Object != null}");

        Debug.Log($"Hat 3 Unlocked: {SaveManager.Instance.IsCosmeticUnlocked(hat3Id)}");
        Debug.Log($"Hat 3 Path: {hat3Path} | Object Found: {hat3Object != null}");

        hat2Button.gameObject.SetActive(SaveManager.Instance.IsCosmeticUnlocked(hat2Id));
        hat3Button.gameObject.SetActive(SaveManager.Instance.IsCosmeticUnlocked(hat3Id));
    }


    void FindPlayer()
    {
        player = GameObject.FindGameObjectWithTag(playerTag);

        if (player != null)
        {
            hat1Object = player.transform.Find(hat1Path)?.gameObject;
            hat2Object = player.transform.Find(hat2Path)?.gameObject;
            hat3Object = player.transform.Find(hat3Path)?.gameObject;


            if (hat1Object == null || hat2Object == null || hat3Object == null)
            {
                Debug.LogError("Could not find outfit objects on player!");
                Debug.Log($"Player found: {player != null}");
                Debug.Log($"Hat 1 exists: {hat1Object != null}");
                Debug.Log($"Hat 2 exists: {hat2Object != null}");
                Debug.Log($"Hat 3 exists: {hat3Object != null}");
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

    void SelectHat(int hatNumber)
    {
        DeactivateAllHats(); // First, disable all hats

        switch (hatNumber)
        {
            case 1:
                ActivateHat(hat1Object);
                break;
            case 2 when SaveManager.Instance.IsCosmeticUnlocked(hat2Id):
                ActivateHat(hat2Object);
                break;
            case 3 when SaveManager.Instance.IsCosmeticUnlocked(hat3Id):
                ActivateHat(hat3Object);
                break;
        }
    }

    void DeactivateAllHats()
    {
        // Disable all hat GameObjects
        if (hat1Object != null) hat1Object.SetActive(false);
        if (hat2Object != null) hat2Object.SetActive(false);
        if (hat3Object != null) hat3Object.SetActive(false);
    }

    void ActivateHat(GameObject hatObject)
    {
        if (hatObject != null)
        {
            hatObject.SetActive(true);
            Debug.Log($"Activated hat: {hatObject.name}");
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
        RefreshCosmeticDisplay();
    }
}