using UnityEngine;

public class LadderTrigger : MonoBehaviour
{
    public Transform kitchenCenter;
    public Transform basementCenter;
    public bool isKitchenLadder;
    public bool isBasementLadder;

    private Camera mainCamera;

    private void Start()
    {
        mainCamera = Camera.main;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerMovement player = other.GetComponent<PlayerMovement>();

            if (player != null && player.isSelected)
            {
                if (isKitchenLadder)
                {
                    MovePlayerAndCamera(player, basementCenter.position, -6.75f);
                }
                else if (isBasementLadder)
                {
                    MovePlayerAndCamera(player, kitchenCenter.position, 6.75f);
                }
            }
        }
    }

    private void MovePlayerAndCamera(PlayerMovement player, Vector3 newPosition, float cameraShift)
    {
        player.Unselect();
        player.transform.position = new Vector3(newPosition.x, newPosition.y, -5);
        mainCamera.transform.position += new Vector3(0, cameraShift, 0);
    }

}
