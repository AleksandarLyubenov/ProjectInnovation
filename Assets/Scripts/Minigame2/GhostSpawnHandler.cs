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
    [SerializeField] private int passiveGhostsAmount;
    [SerializeField] private int enemyGhostsAmount;
    [SerializeField] private float spawnRadius = 10f;

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

        SpawnGhostGroup(passiveGhosts, passiveCount, true);
        SpawnGhostGroup(enemyGhosts, enemyCount, false);

        currentRound++;
    }

    public void ClearExistingGhosts()
    {
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

    void SpawnGhostGroup(List<GameObject> prefabs, int amount, bool isPassive)
    {
        for (int i = 0; i < amount; i++)
        {
            Vector2 spawnPos = Random.insideUnitCircle * spawnRadius;
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