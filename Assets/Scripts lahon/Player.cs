using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Player : MonoBehaviour
{
    [SerializeField] private string switchSceneTo;

    private int health;

    // Start is called before the first frame update
    void Start()
    {
        health = 5;
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
            SwitchScene(switchSceneTo);
        }
    }
    private void SwitchScene(string directedScene)
    {
        SceneManager.LoadScene(directedScene);
    }
}
