using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneSwitch : MonoBehaviour
{
    [SerializeField] private string DirectedScene;

 /*   int currentEnergy = SaveManager.Instance.GetEnergy();
    int currentLevel = SaveManager.Instance.GetPlayerLevel();*/

    public void Back2MenuButton()
    {
        SceneManager.LoadScene(DirectedScene);
    }
}
