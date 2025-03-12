using TMPro;
using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEditor;

public class CirclesManager : MonoBehaviour
{

    [Header("Game Settings")]
    [SerializeField] private GameObject circlePrefab;
    [SerializeField] private int circlesTotal = 3;

    private int circlesRemaining;
    private int successfulHits;

    [Header("UI Assets")]
    [SerializeField] private GameObject mainUI;
    [SerializeField] private GameObject resultPanel;
    [SerializeField] private TextMeshProUGUI contextText;
    [SerializeField] private Button closeButton;

    [Header("Pizza Visualization")]
    [SerializeField] private GameObject[] pizzaStates; // 0-Full, 1-1/3, 2-2/3, 3-Empty
    [SerializeField] private Transform pizzaDisplayParent;

    private bool isMinigameActive;
    public bool IsMinigameActive => isMinigameActive;
    private Vector2 lastSpawnPosition;
    private int hungerIncrease;

    [SerializeField] private float circleZPosition = -5f;

    [Header("Debugging Fields (Remove [SerializeField] before publish!")]
    [SerializeField] private GameObject player;
    [SerializeField] private bool hasPlayerReference;

    [SerializeField] private AudioManager audioManager;

    void Start()
    {
        audioManager = FindAnyObjectByType<AudioManager>();
        resultPanel.SetActive(false);
        closeButton.onClick.AddListener(CloseResultPanel);
    }
    void OnEnable()
    {
        PlayerPresenceNotifier.Instance.OnPlayerFound.AddListener(StorePlayerReference);
        PlayerPresenceNotifier.Instance.OnPlayerLost.AddListener(ClearPlayerReference);
        StartCoroutine(InitializeWhenReady());
    }

    private IEnumerator InitializeWhenReady()
    {
        while (PlayerPresenceNotifier.Instance == null)
        {
            yield return null;
        }

        PlayerPresenceNotifier.Instance.OnPlayerFound.AddListener(StorePlayerReference);
        PlayerPresenceNotifier.Instance.OnPlayerLost.AddListener(ClearPlayerReference);
    }

    void OnDisable()
    {
        PlayerPresenceNotifier.Instance.OnPlayerFound.RemoveListener(StorePlayerReference);
        PlayerPresenceNotifier.Instance.OnPlayerLost.RemoveListener(ClearPlayerReference);
    }

    void StorePlayerReference()
    {
        player = PlayerPresenceNotifier.Instance.GetPlayer();
        hasPlayerReference = true;
    }

    void ClearPlayerReference()
    {
        player = null;
        hasPlayerReference = false;
    }

    public void StartMinigame()
    {
        if (isMinigameActive) return;

        player = FindPlayerClone();

        if (player != null)
        {
            player.SetActive(false);
        }

        // Reset pizza display
        foreach (Transform child in pizzaDisplayParent)
        {
            child.gameObject.SetActive(false);
        }
        pizzaDisplayParent.gameObject.SetActive(true);
        pizzaStates[0].SetActive(true);

        // Existing start code...
        successfulHits = 0;
        circlesRemaining = circlesTotal;

        mainUI.SetActive(false);
        isMinigameActive = true;
        SpawnNewCircle();
    }

    public void ReportCircleResult(bool success)
    {
        if (success)
        {
            successfulHits++;
            audioManager.PlaySound("Eat");
        }
        else
        {
            audioManager.PlaySound("EatFail");
        }

        circlesRemaining--;

        UpdatePizzaDisplay(); // Update visualization after each circle

        if (circlesRemaining > 0)
        {
            SpawnNewCircle();
        }
        else
        {
            EndMinigame();
        }
    }

    void UpdatePizzaDisplay()
    {
        // Deactivate all pizza states
        foreach (GameObject state in pizzaStates)
        {
            state.SetActive(false);
        }

        // Activate appropriate state based on success count
        int stateIndex = Mathf.Clamp(successfulHits, 0, pizzaStates.Length - 1);
        pizzaStates[stateIndex].SetActive(true);
    }

    void HandleCircleTap()
    {
        // Get mouse position at circle depth
        Vector3 screenPos = new Vector3(
            Input.mousePosition.x,
            Input.mousePosition.y,
            Mathf.Abs(Camera.main.transform.position.z) - Mathf.Abs(circleZPosition)
        );

        Vector3 worldPos = Camera.main.ScreenToWorldPoint(screenPos);

        RaycastHit2D hit = Physics2D.Raycast(worldPos, Vector2.zero);

        if (hit.collider != null && hit.collider.TryGetComponent<CircleInstance>(out var circle))
        {
            circle.HandleTap();
        }
    }

    void Update()
    {
        if (!isMinigameActive) return;

        if (Input.GetMouseButtonDown(0))
        {
            RaycastHit2D hit = Physics2D.Raycast(
                Camera.main.ScreenToWorldPoint(Input.mousePosition),
                Vector2.zero
            );

            if (hit.collider != null && hit.collider.TryGetComponent<CircleInstance>(out var circle))
            {
                circle.HandleTap();
            }
        }
    }

    Vector3 GetValidSpawnPosition()
    {
        Vector3 spawnPos;
        int attempts = 0;
        const float minDistance = 2f;

        do
        {
            Vector2 viewportPos = new Vector2(
                Random.Range(0.1f, 0.9f),
                Random.Range(0.1f, 0.9f)
            );

            // Convert to world point and set Z position
            spawnPos = Camera.main.ViewportToWorldPoint(
                new Vector3(viewportPos.x, viewportPos.y, -circleZPosition)
            );
            spawnPos.z = circleZPosition;

            attempts++;
        }
        while (Vector3.Distance(spawnPos, lastSpawnPosition) < minDistance && attempts < 50);

        return spawnPos;
    }

    void SpawnNewCircle()
    {
        Vector3 spawnPos = GetValidSpawnPosition();
        Instantiate(circlePrefab, spawnPos, Quaternion.identity);
        lastSpawnPosition = spawnPos;
    }

    void EndMinigame()
    {
        isMinigameActive = false;
        hungerIncrease = Mathf.RoundToInt(successfulHits * 0.1f * 100);
        SaveManager.Instance.SetHunger(Mathf.Clamp(
            SaveManager.Instance.GetHunger() + hungerIncrease, 0, 100
        ));

        ShowResultPanel();
    }

    void ShowResultPanel()
    {
        contextText.text = $"You restored {hungerIncrease}% hunger!";
        audioManager.PlaySound("Popup");
        resultPanel.SetActive(true);
        resultPanel.GetComponent<PanelAnimator>().ShowPanel();
    }

    private GameObject FindPlayerClone()
    {
        foreach (GameObject go in Resources.FindObjectsOfTypeAll<GameObject>())
        {
            if (go.name == "Player(Clone)" && go.scene.isLoaded)
            {
                return go;
            }
        }
        return null;
    }

    void CloseResultPanel()
    {
        resultPanel.GetComponent<PanelAnimator>().HidePanel();
        mainUI.SetActive(true);
        pizzaDisplayParent.gameObject.SetActive(false); // Hide pizza when closing results

        player = FindPlayerClone();

        if (player != null)
        {
            player.SetActive(true);
        }
    }
}