using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public GameObject outline;
    public float moveSpeed = 5f;
    public LayerMask wallLayer;

    private Rigidbody rb;
    private Vector3 targetPosition;
    private bool isSelected = false;
    private bool moving = false;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        targetPosition = transform.position;
        outline.SetActive(false);
    }

    void Update()
    {
        // Handle selection
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit))
            {
                if (hit.collider.gameObject == gameObject)
                {
                    isSelected = !isSelected;
                    outline.SetActive(isSelected);
                }
                else if (isSelected)
                {
                    Vector3 destination = GetGroundPosition(ray);
                    if (!Physics.Raycast(transform.position, destination - transform.position, Vector3.Distance(transform.position, destination), wallLayer))
                    {
                        targetPosition = destination;
                        moving = true;
                    }
                }
            }
        }
 
        if (moving)
        {
            Vector3 direction = (targetPosition - transform.position).normalized;
            rb.velocity = direction * moveSpeed;

            if (Vector3.Distance(transform.position, targetPosition) < 0.2f)
            {
                rb.velocity = Vector3.zero;
                moving = false;
            }
        }
    }

    Vector3 GetGroundPosition(Ray ray)
    {
        RaycastHit groundHit;
        if (Physics.Raycast(ray, out groundHit, Mathf.Infinity, LayerMask.GetMask("Ground")))
        {
            return groundHit.point;
        }
        return transform.position;
    }
}
