using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GhostMoveToPlayer : MonoBehaviour
{
    public static GhostMoveToPlayer Instance;

    [SerializeField] private GameObject target;
    [SerializeField] private string targetPlayer;
    [SerializeField] private Transform spawner;
    [SerializeField] private Renderer ghostRenderer; // Reference to the Renderer component
    [SerializeField] private Collider2D ghostCollider; // Reference to the Collider2D component

    private Vector2 targetPos;
    private Vector2 ghostPos;
    private Vector2 directionG2T;

    private bool isDamaged = false;
    public bool isSpawned = true;

    private int lives;

    private void Awake()
    {
            Instance = this;
    }

    private void Start()
    {
        lives = 1;
    }
    // Update is called once per frame
    void Update()
    {
        Debug.Log(isSpawned);
        LockPosition();
        TrackingPlayer();
 
        RespawnAferCaught();

        Debug.Log(lives);
    }

    void TrackingPlayer() // for this to work you need both player and ghost to be on the same z position, otherwise the ghost slows down
    {
        if (target != null)
        {
            // get the position of the ghost and the target
            ghostPos = transform.position;
            targetPos = target.transform.position;

            // ghost pos - target pos (B - A)
            directionG2T = ghostPos - targetPos;
            // normalize for consistent speed
            directionG2T.Normalize();

            // apply to position
            transform.position -= (Vector3)directionG2T * 0.004f * 10;
        }
    }

    private void OnTriggerEnter2D()
    {
        if (!isDamaged)
        {
            Debug.Log("health -1");
            lives -= 1;

            if (lives <= 0)
                SwitchScene();

            isDamaged = true;
            DisableGhost();
            StartCoroutine(GhostRespawnAfterDMG(1f));
        }
    }

    public void DisableGhost()
    {
        Debug.Log("Disabling ghost");
        Debug.Log(isDamaged);
        ghostRenderer.enabled = false; // Disable the Renderer component
        ghostCollider.enabled = false; // Disable the Collider2D component
        transform.position = spawner.position;
    }

    private void RespawnAferCaught()
    {
        if (!isSpawned)
        {
            DisableAfterCaught();
            StartCoroutine(GhostRespawnAfterCapture(1f));
        }
    }

    private void DisableAfterCaught()
    {
        ghostRenderer.enabled = false;
        ghostCollider.enabled = false;
        transform.position = spawner.position;
    }

    private IEnumerator GhostRespawnAfterDMG(float Delay)
    {
        Debug.Log("Starting coroutine");
        yield return new WaitForSeconds(Delay);
        Debug.Log("Re-enabling ghost");
        isDamaged = false;
        ghostRenderer.enabled = true; // Enable the Renderer component
        ghostCollider.enabled = true; // Enable the Collider2D component
    }

    private IEnumerator GhostRespawnAfterCapture(float Delay)
    {
        yield return new WaitForSeconds(Delay);
        ghostRenderer.enabled = true;
        ghostCollider.enabled = true;
        isSpawned = true;
    }

    private void SwitchScene()
    {
        SceneManager.LoadScene("GameOverScene");
    }

    void LockPosition()
    {
        // Locking ghost z position
        Vector3 lockedPosition = transform.position;
        lockedPosition.z = 0;
        transform.position = lockedPosition;
    }
}