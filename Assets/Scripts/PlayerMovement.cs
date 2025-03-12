using UnityEngine;
using System.Collections.Generic;

public class PlayerMovement : MonoBehaviour
{
    // Inspector Assignments
    [Header("Movement")]
    public float moveSpeed = 5f;
    public LayerMask wallLayer;
    public LayerMask groundLayer;

    [Header("Visuals")]
    public GameObject outline;
    public Animator bodyAnimator;
    public Animator outlineAnimator;

    // Private Variables
    private Queue<Vector3> waypoints = new Queue<Vector3>();
    private Camera mainCamera;
    public bool isSelected = false;
    private Vector3 currentTarget;
    private bool moving = false;
    private Vector2 lastMoveDirection;

    void Start()
    {
        mainCamera = Camera.main;
        outline.SetActive(false);

        if (!bodyAnimator || !outlineAnimator)
            Debug.LogError("Assign both body and outline animators!");
    }

    void Update()
    {
        HandleSelectionInput();
        HandleMovementInput();
    }

    void FixedUpdate()
    {
        if (moving)
        {
            MoveStepByStep();
            UpdateAnimation();
            CheckForObstacles();
        }
    }

    private void HandleSelectionInput()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Vector2 mousePos = mainCamera.ScreenToWorldPoint(Input.mousePosition);
            Collider2D hit = Physics2D.OverlapPoint(mousePos);

            if (hit && hit.gameObject == gameObject)
            {
                ToggleSelection();
            }
        }
    }

    private void ToggleSelection()
    {
        isSelected = !isSelected;
        outline.SetActive(isSelected);
    }

    private void HandleMovementInput()
    {
        if (isSelected && Input.GetMouseButtonDown(0))
        {
            Vector3 destination = mainCamera.ScreenToWorldPoint(Input.mousePosition);
            destination.z = 0;

            if (IsValidDestination(destination))
            {
                CalculatePath(destination);
                moving = true;
            }
        }
    }

    private bool IsValidDestination(Vector3 destination)
    {
        return Physics2D.OverlapPoint(destination, groundLayer) &&
               !Physics2D.Linecast(transform.position, destination, wallLayer);
    }

    private void CalculatePath(Vector3 destination)
    {
        waypoints.Clear();
        Vector3 currentPos = transform.position;

        // Grid-based path calculation
        Vector3 horizontalMove = new Vector3(destination.x, currentPos.y, 0);
        Vector3 verticalMove = new Vector3(destination.x, destination.y, 0);

        if (horizontalMove != currentPos) waypoints.Enqueue(horizontalMove);
        if (verticalMove != horizontalMove) waypoints.Enqueue(verticalMove);
    }

    private void MoveStepByStep()
    {
        if (waypoints.Count == 0)
        {
            moving = false;
            return;
        }

        currentTarget = waypoints.Peek();
        transform.position = Vector3.MoveTowards(
            transform.position,
            currentTarget,
            moveSpeed * Time.fixedDeltaTime
        );

        // Store movement direction for animation
        lastMoveDirection = (currentTarget - transform.position).normalized;

        if (Vector3.Distance(transform.position, currentTarget) < 0.01f)
        {
            transform.position = currentTarget;
            waypoints.Dequeue();
        }
    }

    private void CheckForObstacles()
    {
        if (waypoints.Count > 0 && Physics2D.Linecast(transform.position, currentTarget, wallLayer))
        {
            StopMovement();
            Debug.Log("Path blocked - stopping movement");
        }
    }

    private void UpdateAnimation()
    {
        if (waypoints.Count == 0 || lastMoveDirection.magnitude < 0.1f)
        {
            SafePlayAnimation("Idle");
            return;
        }

        if (lastMoveDirection.x > 0.1f) SafePlayAnimation("Right");
        else if (lastMoveDirection.x < -0.1f) SafePlayAnimation("Left");
        else if (lastMoveDirection.y > 0.1f) SafePlayAnimation("Up");
        else if (lastMoveDirection.y < -0.1f) SafePlayAnimation("Down");
    }

    private void SafePlayAnimation(string stateName)
    {
        PlayAnimationState(bodyAnimator, stateName);
        PlayAnimationState(outlineAnimator, stateName);
    }

    private void PlayAnimationState(Animator animator, string stateName)
    {
        if (animator && animator.HasState(0, Animator.StringToHash(stateName)))
        {
            animator.Play(stateName);
        }
    }

    public void Unselect()
    {
        isSelected = false;
        outline.SetActive(false);
        StopMovement();
    }

    public void Select()
    {
        isSelected = true;
        outline.SetActive(true);
    }

    private void StopMovement()
    {
        moving = false;
        waypoints.Clear();
        lastMoveDirection = Vector2.zero;
        SafePlayAnimation("Idle");
    }
}