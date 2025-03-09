using UnityEngine;

public class ResetButtonHandler : MonoBehaviour
{
    public void OnResetButtonClicked()
    {
        SaveManager.Instance.DeleteSaveData();
        Debug.Log("Data reset via UI button");
    }
}