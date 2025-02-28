using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneSwitch : MonoBehaviour
{
    [SerializeField] string sceneToSwitchTo = "Alex's Scene";

    void Update()
    {
        StartCoroutine(SceneSwitchDelay(5f));
    }

    private IEnumerator SceneSwitchDelay(float delay)
    {
        Debug.Log("scenes are switching in: " + delay);
        yield return new WaitForSeconds(delay);
        SceneManager.LoadScene(sceneToSwitchTo);
    }
}
