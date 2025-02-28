using UnityEngine;
using System.Collections;
using System.Text;
using UnityEditor;
using UnityEngine.Events;



#if UNITY_ANDROID
using UnityEngine.Android;
#endif

public class NfcReader : MonoBehaviour
{
    private AndroidJavaObject mActivity;
    private AndroidJavaObject mIntent;
    private string mLastNfcId = "";

    //[SerializeField]
    //private ReaderBehaviour readerBehaviour;

    [Header("Character UID Scriptable Objects")]
    [SerializeField] private NFCTagUID Character1UIDs;
    [SerializeField] private NFCTagUID Character2UIDs;

    [Header("Character Materials")]
    [SerializeField] private Material Character1Material;
    [SerializeField] private Material Character2Material;

    [Header("Character Spawning Prerequisites")]
    [SerializeField] private Transform spawnPoint;
    [SerializeField] private GameObject char1Prefab;
    [SerializeField] private GameObject char2Prefab;

    [Header("Events")]
    public UnityEvent<string> onNFCSuccess = new UnityEvent<string>();
    public UnityEvent onNFCError = new UnityEvent();

    public UnityEvent<string> onNFCAlreadyUnlocked = new UnityEvent<string>();

    [Header("Unlockables Tracking")]
    [SerializeField] private UnlockablesTracker unlockablesTracker;

    void Start()
    {
#if UNITY_ANDROID
        if (!Permission.HasUserAuthorizedPermission("android.permission.NFC"))
        {
            Permission.RequestUserPermission("android.permission.NFC");
        }

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

            // Change the cube color based on the detected tag
            SpawnCharacter(tagIdString);
        }
#endif
    }

    void SetCubeMaterial(Material newMaterial)
    {
        GameObject cube = GameObject.Find("Cube");
        if (cube != null)
        {
            Renderer cubeRenderer = cube.GetComponent<Renderer>();
            if (cubeRenderer != null)
            {
                cubeRenderer.material = newMaterial;
            }
        }
    }

    void SpawnCharacter(string tagIdString)
    {
        string itemName = "";
        GameObject prefabToSpawn = null;

        foreach (var tag in Character1UIDs.tagData)
        {
            if (tag.uid == tagIdString)
            {
                itemName = tag.itemName;
                prefabToSpawn = char1Prefab;
                break;
            }
        }

        if (prefabToSpawn == null)
        {
            foreach (var tag in Character2UIDs.tagData)
            {
                if (tag.uid == tagIdString)
                {
                    itemName = tag.itemName;
                    prefabToSpawn = char2Prefab;
                    break;
                }
            }
        }

        if (prefabToSpawn != null)
        {
            if (unlockablesTracker.IsUnlocked(itemName))
            {
                onNFCAlreadyUnlocked.Invoke(itemName);
            }
            else
            {
                unlockablesTracker.Unlock(itemName);
                Instantiate(prefabToSpawn, spawnPoint.position, Quaternion.identity);
                onNFCSuccess.Invoke(itemName);
            }
        }
        else
        {
            onNFCError.Invoke();
        }
    }

    //void ChangeColor(string tagIdString)
    //{
    //    if (Character1UIDs.tagUIDs.Contains(tagIdString))
    //    {
    //        SetCubeMaterial(Character1Material);
    //        Debug.Log("Cube color changed to Character 1!");
    //    }
    //    else if (Character2UIDs.tagUIDs.Contains(tagIdString))
    //    {
    //        SetCubeMaterial(Character2Material);
    //        Debug.Log("Cube color changed to Character 2!");
    //    }
    //    else
    //    {
    //        Debug.LogWarning("Tag ID not recognized!");
    //    }
    //}


    //void DestroyCube()
    //{
    //    GameObject cube = GameObject.Find("Cube");
    //    if (cube != null)
    //    {
    //        Destroy(cube);
    //        Debug.Log("Cube destroyed!");
    //    }
    //    else
    //    {
    //        Debug.LogError("No cube found in scene!");
    //    }
    //}
}