using UnityEngine;

public class NFCManager : MonoBehaviour
{
    public GameObject cubeToDestroy;  // Assign this in the Unity Editor

    private void Start()
    {
        AndroidJavaClass unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
        AndroidJavaObject activity = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity");
        AndroidJavaClass nfcReader = new AndroidJavaClass("com.yourgame.nfcplugin.NFCReader");

        nfcReader.CallStatic("Init");
    }

    // This function will be called from Java when an NFC tag is read
    public void OnNFCRead(string data)
    {
        Debug.Log("NFC Data: " + data);

        // Delete the cube if it exists
        if (cubeToDestroy != null)
        {
            Destroy(cubeToDestroy);
            Debug.Log("Cube destroyed!");
        }
        else
        {
            Debug.Log("Cube not found!");
        }
    }
}
