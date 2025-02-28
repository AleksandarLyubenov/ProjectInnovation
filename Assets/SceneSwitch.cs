using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneSwitch : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        StartCoroutine(SceneSwitchDelay(5f));
    }

    private IEnumerator SceneSwitchDelay(float delay)
    {
        Debug.Log("scenes are switching in: " + delay);
        yield return new WaitForSeconds(delay);
        SceneManager.LoadScene("Lahon's minigame Scene (before first merge)");
    }
}
