using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Player : MonoBehaviour
{
    [SerializeField] private GameObject GameOverPanel;
    [SerializeField] private string switchSceneTo;

    [HideInInspector] public bool gameIsOver;

    private int health;

    private GhostMoveToPlayer[] ghosts;
    private TouchDrawer touchDrawer;

    // Start is called before the first frame update
    void Start()
    {
        gameIsOver = false;
        GameOverPanel.SetActive(false);
        health = 5;
        ghosts = FindObjectsOfType<GhostMoveToPlayer>();
        touchDrawer = FindObjectOfType<TouchDrawer>();
    }

    // Update is called once per frame
    void Update()
    {
        GameOver();
    }

    public void PlayerTakesDmg(int dmgAmount)
    {
        health -= dmgAmount;
    }

    private void GameOver()
    {
        if (health <= 0)
        {
            GameOverPanel.SetActive(true);
            DisableGhosts();
            DisableDrawing();
            gameIsOver = true;
        }
    }

    private void DisableGhosts()
    {
        foreach (var ghost in ghosts)
        {
            ghost.DisableGhost();
            ghost.StopMovement();
        }
    }

    private void DisableDrawing()
    {
        if (touchDrawer != null)
        {
            touchDrawer.DisableDrawing();
        }
    }

    private void SwitchScene(string directedScene)
    {
        SceneManager.LoadScene(directedScene);
    }
}