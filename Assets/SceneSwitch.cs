using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneSwitch : MonoBehaviour
{
    [SerializeField] private string DirectedScene;
    private ScoreManager scoreManager;

    private void Start()
    {
        scoreManager = FindObjectOfType<ScoreManager>();
    }

    public void Back2MenuButton()
    {
        int sanityGained = scoreManager.GetSanityGained();
        // Add sanityGained to another value in this script as needed
        Debug.Log($"Sanity Gained: {sanityGained}");
        SceneManager.LoadScene(DirectedScene);
    }
}