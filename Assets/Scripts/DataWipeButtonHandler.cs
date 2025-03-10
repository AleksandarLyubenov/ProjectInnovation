using UnityEngine;

public class ResetButtonHandler : MonoBehaviour
{
    public void OnResetButtonClicked()
    {
        SaveManager.Instance.DeleteSaveData();
        Debug.Log("All progress reset - quitting application");

        // Quit the application
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
    }
}