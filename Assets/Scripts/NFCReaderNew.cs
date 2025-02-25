using UnityEngine;
using System.Collections;
using System.Text;

#if UNITY_ANDROID
using UnityEngine.Android;
#endif

public class NfcReader : MonoBehaviour
{
    private AndroidJavaObject mActivity;
    private AndroidJavaObject mIntent;
    private string mLastNfcId = "";

    void Start()
    {
#if UNITY_ANDROID
        if (!Permission.HasUserAuthorizedPermission("android.permission.NFC"))
        {
            Permission.RequestUserPermission("android.permission.NFC");
        }

        // Get current Android activity
        AndroidJavaClass unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
        mActivity = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity");
#endif
    }

    void Update()
    {
#if UNITY_ANDROID
        // Check for NFC intents
        AndroidJavaObject intent = mActivity.Call<AndroidJavaObject>("getIntent");
        string action = intent.Call<string>("getAction");

        if (action == "android.nfc.action.NDEF_DISCOVERED" ||
            action == "android.nfc.action.TECH_DISCOVERED" ||
            action == "android.nfc.action.TAG_DISCOVERED")
        {
            ProcessNfcTag(intent);
        }
#endif
    }

    void ProcessNfcTag(AndroidJavaObject intent)
    {
#if UNITY_ANDROID
        // Get NFC tag ID
        AndroidJavaObject tag = intent.Call<AndroidJavaObject>("getParcelableExtra", "android.nfc.extra.TAG");
        byte[] tagId = tag.Call<byte[]>("getId");

        // Convert byte array to hex string
        StringBuilder sb = new StringBuilder();
        foreach (byte b in tagId)
        {
            sb.AppendFormat("{0:x2}", b);
        }
        string tagIdString = sb.ToString();

        if (tagIdString != mLastNfcId)
        {
            mLastNfcId = tagIdString;
            Debug.Log("NFC Tag Detected: " + tagIdString);

            // Delete the cube
            DestroyCube();
        }
#endif
    }

    void DestroyCube()
    {
        GameObject cube = GameObject.Find("Cube");
        if (cube != null)
        {
            Destroy(cube);
            Debug.Log("Cube destroyed!");
        }
        else
        {
            Debug.LogError("No cube found in scene!");
        }
    }
}