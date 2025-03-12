using UnityEngine;
using UnityEngine.UI;

public class ButtonSelectPlayer : MonoBehaviour
{
    private PlayerMovement player;

    public void ReselectPlayer()
    {
        player = FindAnyObjectByType<PlayerMovement>();
        player.Select();
    }
}