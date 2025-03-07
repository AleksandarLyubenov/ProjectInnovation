using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public GameObject outline;
    public float moveSpeed = 5f;
    public LayerMask wallLayer;

    private Rigidbody2D rb;
    private Vector3 targetPosition;
    public bool isSelected = false;
    private bool moving = false;
    private Camera mainCamera;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        mainCamera = Camera.main;
        targetPosition = transform.position;
        outline.SetActive(false);
    }

    void Update()
    {
        // Handle selection
        if (Input.GetMouseButtonDown(0))
        {
            Vector2 mousePos = mainCamera.ScreenToWorldPoint(Input.mousePosition);
            Collider2D hitCollider = Physics2D.OverlapPoint(mousePos);

            if (hitCollider != null && hitCollider.gameObject == gameObject)
            {
                isSelected = !isSelected;
                outline.SetActive(isSelected);
            }
            else if (isSelected)
            {
                Vector3 destination = mainCamera.ScreenToWorldPoint(Input.mousePosition);
                destination.z = 0; // Ensure z-position is zero for 2D

                // Check for walls using 2D linecast
                if (!Physics2D.Linecast(transform.position, destination, wallLayer))
                {
                    targetPosition = destination;
                    moving = true;
                }
            }
        }

        if (moving)
        {
            Vector2 direction = (targetPosition - transform.position).normalized;
            rb.velocity = direction * moveSpeed;

            if (Vector2.Distance(transform.position, targetPosition) < 0.1f)
            {
                rb.velocity = Vector2.zero;
                moving = false;
            }
        }
    }
}