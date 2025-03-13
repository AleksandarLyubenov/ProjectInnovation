using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GhostMoveToPlayer : MonoBehaviour
{
    [Header("Entities")]
    [SerializeField] private GameObject target;
    [SerializeField] private Transform spawner;

    [Header("Components")]
    [SerializeField] private SpriteRenderer ghostRenderer; // Reference to the Renderer component
    [SerializeField] private Collider2D ghostCollider; // Reference to the Collider2D component

    [Header("Enemy Status")]
    private Player playerScript;
    private EnemyColor enemyColorScript;

    private Vector2 targetPos;
    private Vector2 ghostPos;
    private Vector2 directionG2T;

    public bool isSpawned = true;
    private bool isGameOver = false;
    private float speed = 1.5f;

    public void Start()
    {
        ghostRenderer = GetComponent<SpriteRenderer>();
        playerScript = FindAnyObjectByType<Player>();
        enemyColorScript = GetComponent<EnemyColor>();
    }

    // Update is called once per frame
    void Update()
    {
        if (isGameOver) return;

        Debug.Log(isSpawned);
        LockPosition();
        TrackingPlayer();

        RespawnAfterCaught();
    }

    void TrackingPlayer()
    {
        if (isSpawned && target != null)
        {
            // get the position of the ghost and the target
            ghostPos = transform.position;
            targetPos = target.transform.position;

            // ghost pos - target pos (B - A)
            directionG2T = ghostPos - targetPos;
            // normalize for consistent speed
            directionG2T.Normalize();

            // apply to position
            transform.position -= (Vector3)directionG2T * speed * Time.deltaTime;
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerScript.PlayerTakesDmg(1);
            DisableGhost();
            StartCoroutine(RespawnGhost(1f));
        }
    }

    public void RespawnAfterCaught()
    {
        if (!isSpawned)
        {
            DisableGhost();
            StartCoroutine(RespawnGhost(1f));
        }
    }

    public void DisableGhost()
    {
        this.ghostRenderer.enabled = false; // Disable the Renderer component
        this.ghostCollider.enabled = false; // Disable the Collider2D component
        transform.position = spawner.position;
        isSpawned = false; // Set isSpawned to false

        // Randomize the enemy color
        if (enemyColorScript != null)
        {
            enemyColorScript.RandomizeColor();
        }
    }

    public void StopMovement()
    {
        isGameOver = true;
    }

    public void SetSpeed(float newSpeed)
    {
        speed = newSpeed;
    }

    private IEnumerator RespawnGhost(float delay)
    {
        Debug.Log("Starting coroutine");
        yield return new WaitForSeconds(delay);
        Debug.Log("Re-enabling ghost");

        if (!isSpawned)
        {
            Debug.Log("Ghost is Respawned");
            isSpawned = true;
        }

        this.ghostRenderer.enabled = true; // Enable the Renderer component
        this.ghostCollider.enabled = true; // Enable the Collider2D component
    }

    void LockPosition()
    {
        // Locking ghost z position
        Vector3 lockedPosition = transform.position;
        lockedPosition.z = 0;
        transform.position = lockedPosition;
    }
}