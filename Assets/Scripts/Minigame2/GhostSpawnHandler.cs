using System.Collections.Generic;
using UnityEngine;

public class GhostSpawnHandler : MonoBehaviour
{
    public static GhostSpawnHandler Instance;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private int currentRound = 0;
    private int currentScore = 0;
    [SerializeField] private float startTime;
    private List<GameObject> activeGhosts = new List<GameObject>();

    [Header("Ghost Prefabs")]
    [SerializeField] private List<GameObject> passiveGhosts;
    [SerializeField] private List<GameObject> enemyGhosts;

    [Header("Spawn Settings")]
    [SerializeField] private bool isRadialSpawnEnabled = false;
    [SerializeField] private int passiveGhostsAmount;
    [SerializeField] private int enemyGhostsAmount;
    [SerializeField] private float spawnRadialRadius = 10f;

    [Header("Rectangular Spawn Area")]
    [SerializeField] private float spawnWidth = 19f;
    [SerializeField] private float spawnHeight = 10f;
    [SerializeField] private float minSeparation = 1.5f;
    [SerializeField] private float minDistanceFromPlayer = 3.0f;

    [Header("Round Settings")]
    public int maxPassivePerRound = 15;

    void Start()
    {
        startTime = Time.time;
        StartNewRound();
    }

    public float GetPlayTime() => Time.time - startTime;
    public int GetCurrentScore() => currentScore;

    void StartNewRound()
    {
        ClearExistingGhosts();

        int passiveCount = Mathf.Min(currentRound + 1, maxPassivePerRound); // Start at 1
        int enemyCount = 1;

        if (isRadialSpawnEnabled)
        {
            SpawnRadialGhostGroup(passiveGhosts, passiveCount, true);
            SpawnRadialGhostGroup(enemyGhosts, enemyCount, false);
        }
        else
        {
            SpawnRectangularGhostGroup(passiveGhosts, passiveCount, true);
            SpawnRectangularGhostGroup(enemyGhosts, enemyCount, false);
        }
        

        currentRound++;
    }

    public void ClearExistingGhosts()
    {
        VibrationManager.Instance.StopVibration();

        int count = activeGhosts.Count;
        for (int i = count - 1; i >= 0; i--)
        {
            if (activeGhosts[i] != null)
            {
                Destroy(activeGhosts[i]);
            }
        }
        activeGhosts.Clear();
        Debug.Log($"Cleared {count} ghosts"); 
    }

    void SpawnRectangularGhostGroup(List<GameObject> prefabs, int amount, bool isPassive)
    {
        VibrationManager.Instance.StopVibration();
        if (!isPassive) amount = 1;

        Vector2 playerPosition = FlashlightController.Instance.transform.position;

        for (int i = 0; i < amount; i++)
        {
            Vector2 spawnPos;
            int attempts = 0;
            bool validPosition;

            do
            {
                // Generate position within rectangular area
                spawnPos = new Vector2(
                    Random.Range(-spawnWidth / 2, spawnWidth / 2),
                    Random.Range(-spawnHeight / 2, spawnHeight / 2)
                ) + playerPosition; // Center around player

                // Check constraints
                //bool farEnoughFromPlayer = Vector2.Distance(spawnPos, playerPosition) > minDistanceFromPlayer;
                bool farEnoughFromOthers = IsPositionValid(spawnPos);

                //validPosition = farEnoughFromPlayer && farEnoughFromOthers;
                validPosition = farEnoughFromOthers;
                attempts++;
            }
            while (!validPosition && attempts < 100);

            GameObject ghost = Instantiate(
                prefabs[Random.Range(0, prefabs.Count)],
                spawnPos,
                Quaternion.identity
            );

            var controller = ghost.GetComponent<GhostTransparencyController>();
            controller.isPassive = isPassive;
            controller.SetHandlers(this, FindObjectOfType<GameOverHandler>());
            activeGhosts.Add(ghost);
        }
    }

    bool IsPositionValid(Vector2 testPosition)
    {
        // Check against all existing ghosts
        foreach (GameObject existingGhost in activeGhosts)
        {
            if (existingGhost != null &&
                Vector2.Distance(testPosition, existingGhost.transform.position) < minSeparation)
            {
                return false;
            }
        }
        return true;
    }

    void SpawnRadialGhostGroup(List<GameObject> prefabs, int amount, bool isPassive)
    {
        // Clear previous vibration when spawning new ghosts
        VibrationManager.Instance.StopVibration();

        // For enemies, ensure only one is ever spawned
        if (!isPassive) amount = 1;

        for (int i = 0; i < amount; i++)
        {
            Vector2 spawnPos;
            int attempts = 0;
            float minDistanceFromPlayer = 3f; // Minimum spawn distance from flashlight

            // Find valid spawn position
            do
            {
                spawnPos = Random.insideUnitCircle * spawnRadialRadius;
                attempts++;
            }
            while (Vector2.Distance(spawnPos, FlashlightController.Instance.transform.position) < minDistanceFromPlayer
                   && attempts < 50);

            GameObject ghost = Instantiate(
                prefabs[Random.Range(0, prefabs.Count)],
                spawnPos,
                Quaternion.identity
            );

            var controller = ghost.GetComponent<GhostTransparencyController>();
            controller.isPassive = isPassive;
            controller.SetHandlers(this, FindObjectOfType<GameOverHandler>());
            activeGhosts.Add(ghost);
        }
    }

    public void OnCorrectGuess()
    {
        currentScore++;

        if (currentScore >= 5 && !SaveManager.Instance.IsCosmeticUnlocked("Hat_2"))
        {
            SaveManager.Instance.UnlockCosmetic("Hat_2");
        }

        StartNewRound();
    }
}