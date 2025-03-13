using UnityEngine;

public class FollowTouchscreen : MonoBehaviour
{
    [SerializeField] private Camera mainCamera;

    void Update()
    {
        if (!Application.isMobilePlatform && Input.GetMouseButton(0))
        {
            RotateTowardsPosition(Input.mousePosition);
        }
        else if (Input.touchCount > 0)
        {
            RotateTowardsPosition(Input.GetTouch(0).position);
        }
    }

    private void RotateTowardsPosition(Vector3 screenPosition)
    {
        Vector3 worldPosition = mainCamera.ScreenToWorldPoint(screenPosition);
        worldPosition.z = transform.position.z; // Maintain Z position for 2D

        Vector2 direction = (worldPosition - transform.position).normalized;

        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg - 90f;

        transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
    }
}