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

    private Player playerScipt;

    private Vector2 targetPos;
    private Vector2 ghostPos;
    private Vector2 directionG2T;


    public bool isSpawned = true;


    public void Start()
    {
        ghostRenderer = GetComponent<SpriteRenderer>();
        playerScipt = FindAnyObjectByType<Player>();
    }

    // Update is called once per frame
    void Update()
    {
        Debug.Log(isSpawned);
        LockPosition();
        TrackingPlayer();

        RespawnAferCaught();

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
            transform.position -= (Vector3)directionG2T * 0.004f * 1;
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            
            playerScipt.PlayerTakesDmg(1);
            DisableGhost();
            StartCoroutine(RespawnGhost(1f));
        }
    }

    public void RespawnAferCaught()
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
    }

    private IEnumerator RespawnGhost(float Delay)
    {
        Debug.Log("Starting coroutine");
        yield return new WaitForSeconds(Delay);
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