using System.Collections.Generic;
using UnityEngine;

public class GhostSpawnHandler : MonoBehaviour
{
    [Header("Ghost Prefabs")]
    [SerializeField] private List<GameObject> passiveGhosts;
    [SerializeField] private List<GameObject> enemyGhosts;

    [Header("Spawn Settings")]
    [SerializeField] private int passiveGhostsAmount;
    [SerializeField] private int enemyGhostsAmount;
    [SerializeField] private float spawnRadius = 10f;

    void Start()
    {
        SpawnGhostGroup(passiveGhosts, passiveGhostsAmount, true);
        SpawnGhostGroup(enemyGhosts, enemyGhostsAmount, false);
    }

    void SpawnGhostGroup(List<GameObject> prefabs, int amount, bool isPassive)
    {
        for (int i = 0; i < amount; i++)
        {
            if (prefabs.Count == 0) return;

            Vector2 spawnPos = Random.insideUnitCircle * spawnRadius;
            GameObject ghost = Instantiate(
                prefabs[Random.Range(0, prefabs.Count)],
                new Vector3(spawnPos.x, spawnPos.y, 0),
                Quaternion.identity
            );

            GhostTransparencyController controller = ghost.GetComponent<GhostTransparencyController>();
            if (controller) controller.isPassive = isPassive;
        }
    }
}