using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class ScenePlayerManager : MonoBehaviour
{
    [Header("Player Settings")]
    [SerializeField] private GameObject playerPrefab;
    [SerializeField] private Transform spawnPoint;
    [SerializeField] private string targetScene = "Aleksander's Scene";
    [SerializeField] private string playerCharacterName = "Player";

    private void Start()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
        // Handle initial scene if already loaded
        if (SceneManager.GetActiveScene().name == targetScene)
        {
            StartCoroutine(DelayedSpawnCheck());
        }
    }

    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name == targetScene)
        {
            StartCoroutine(DelayedSpawnCheck());
        }
    }

    IEnumerator DelayedSpawnCheck()
    {
        // Wait for 1 frame to ensure scene is fully loaded
        yield return null;
        HandlePlayerSpawning();
    }

    private void HandlePlayerSpawning()
    {
        // Use FindWithTag instead of Find for better performance
        GameObject existingPlayer = GameObject.FindWithTag("Player");

        if (existingPlayer == null)
        {
            if (SaveManager.Instance.IsCharacterUnlocked(playerCharacterName))
            {
                SpawnPlayer();
            }
        }
        else
        {
            Debug.Log("Player already exists in scene");
        }
    }

    private void SpawnPlayer()
    {
        if (playerPrefab == null)
        {
            Debug.LogError("Player prefab reference missing!");
            return;
        }

        if (spawnPoint == null)
        {
            Debug.LogError("Spawn point reference missing!");
            return;
        }

        // Ensure we're on main thread for instantiate
        StartCoroutine(SpawnPlayerRoutine());
    }

    IEnumerator SpawnPlayerRoutine()
    {
        yield return new WaitForEndOfFrame();
        Instantiate(playerPrefab, spawnPoint.position, Quaternion.identity);
        Debug.Log("Player spawned successfully");

        // Android-specific debug
#if UNITY_ANDROID && !UNITY_EDITOR
        Debug.Log("Spawn position: " + spawnPoint.position);
        WriteToPersistentData("Player spawned at: " + System.DateTime.Now);
#endif
    }

    // Android debug logging
    private void WriteToPersistentData(string message)
    {
        string path = Application.persistentDataPath + "/debug.log";
        System.IO.File.AppendAllText(path, message + "\n");
    }
}