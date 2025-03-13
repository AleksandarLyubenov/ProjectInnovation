using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ScoreManager : MonoBehaviour
{
    [Header("Stat text")]
    [SerializeField] private TMP_Text scoreText;
    [SerializeField] private TMP_Text sanityText;
    [SerializeField] private TMP_Text hungerText;
    [SerializeField] private TMP_Text cleanlinessText;

    private int score = 0;
    private int sanityGained = -5;

    private int hungerLost = -1;
    private int cleanlineLost = -2;
    private float defaultSpeed = 1.5f;
    private float enemySpeedIncrement = 0.25f;
    private float survivalTime = 0f;

    private bool isGameOver = false;

    private GhostMoveToPlayer[] ghosts;
    private TouchDrawer touchDrawer;
    private Player player;

    private Coroutine sanityCoroutine;

    // Start is called before the first frame update
    void Start()
    {
        score = 0;
        ghosts = FindObjectsOfType<GhostMoveToPlayer>();
        touchDrawer = FindObjectOfType<TouchDrawer>();
        player = FindObjectOfType<Player>();
        sanityCoroutine = StartCoroutine(SanityTimer());
        UpdateScoreText();
    }

    // Update is called once per frame
    void Update()
    {
        if (player.gameIsOver == true)
        {
            GameOver();
        }
        if (!isGameOver)
        {
            survivalTime += Time.deltaTime;
        }
    }

    private IEnumerator SanityTimer()
    {
        while (!isGameOver)
        {
            yield return new WaitForSeconds(5f);
            if (!isGameOver)
            {
                sanityGained += 1;
                hungerLost -= 2;
                cleanlineLost -= 3;
                IncreaseEnemySpeed();
            }
        }
    }

    private void IncreaseEnemySpeed()
    {
        if (defaultSpeed < 5f)
        {
            defaultSpeed += enemySpeedIncrement;
            foreach (var ghost in ghosts)
            {
                ghost.SetSpeed(defaultSpeed);
            }
        }
    }

    public void GameOver()
    {
        isGameOver = true;
        if (sanityCoroutine != null)
        {
            StopCoroutine(sanityCoroutine);
        }
        CalculateSanityGained();
        DisplaySanityGained();
        DisplayHungerLost();
        DisplayCleanlinessLost();
        DisableGhosts();
        DisableDrawing();
        SaveManager.Instance.SetHunger(SaveManager.Instance.GetHunger() - hungerLost);
        SaveManager.Instance.SetCleanliness(SaveManager.Instance.GetCleanliness() - cleanlineLost);
        SaveManager.Instance.SetSanity(SaveManager.Instance.GetSanity() + sanityGained);
    }

    private void CalculateSanityGained()
    {
        sanityGained = (int)(survivalTime / 5) * 1;
    }

    public int GetSanityGained()
    {
        return sanityGained;
    }

    public void AddScore(int points)
    {
        score += points;
        UpdateScoreText();
    }

    private void UpdateScoreText()
    {
        if (scoreText != null)
        {
            scoreText.text = $"Score: {score}";
        }
    }

    private void DisplaySanityGained()
    {
        if (sanityText != null)
        {
            sanityText.text = $"+{sanityGained}%";
        }
    }
    private void DisplayHungerLost()
    {
        if (hungerText != null)
        {
            hungerText.text = $"+{hungerLost}%";
        }
    }

    private void DisplayCleanlinessLost()
    {
        if (cleanlinessText != null)
        {
            cleanlinessText.text = $"+{cleanlineLost}%";
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
}