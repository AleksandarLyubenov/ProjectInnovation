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

    [Header("Outfit UID Scriptable Objects")]
    [SerializeField] private NFCTagUID OutfitUIDs;

    [Header("Events")]
    public UnityEvent<string> onNFCSuccess = new UnityEvent<string>();
    public UnityEvent onNFCError = new UnityEvent();

    public UnityEvent<string> onNFCAlreadyUnlocked = new UnityEvent<string>();

    //[Header("Unlockables Tracking")]
    //[SerializeField] private UnlockablesTracker unlockablesTracker;

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

    void ProcessNfcTag(AndroidJavaObject intent)
    {
#if UNITY_ANDROID
        AndroidJavaObject tag = intent.Call<AndroidJavaObject>("getParcelableExtra", "android.nfc.extra.TAG");
        byte[] tagId = tag.Call<byte[]>("getId");

        StringBuilder sb = new StringBuilder();
        foreach (byte b in tagId)
        {
            sb.AppendFormat("{0:x2}", b);
        }
        string tagIdString = sb.ToString();

        if (tagIdString != mLastNfcId)
        {
            mLastNfcId = tagIdString;
            Debug.Log($"[NFC] New tag detected: {tagIdString}");
            SpawnCharacter(tagIdString);
        }
#endif
    }

    void SpawnCharacter(string tagIdString)
    {
        Debug.Log($"[NFC] Processing tag: {tagIdString}");

        string itemName = "";
        GameObject prefabToSpawn = null;
        bool isOutfit = false;

        // Check Character 1 tags
        foreach (var tag in Character1UIDs.tagData)
        {
            if (tag.uid == tagIdString)
            {
                itemName = tag.itemName;
                prefabToSpawn = char1Prefab;
                Debug.Log($"[NFC] Matched Character1 tag: {itemName}");
                break;
            }
        }

        // Check Character 2 tags
        if (prefabToSpawn == null)
        {
            foreach (var tag in Character2UIDs.tagData)
            {
                if (tag.uid == tagIdString)
                {
                    itemName = tag.itemName;
                    prefabToSpawn = char2Prefab;
                    Debug.Log($"[NFC] Matched Character2 tag: {itemName}");
                    break;
                }
            }
        }

        // Check Outfit tags
        if (prefabToSpawn == null && OutfitUIDs != null)
        {
            foreach (var tag in OutfitUIDs.tagData)
            {
                if (tag.uid == tagIdString)
                {
                    itemName = tag.itemName;
                    isOutfit = true;
                    Debug.Log($"[NFC] Matched Outfit tag: {itemName}");
                    HandleOutfitUnlock(itemName);
                    return;
                }
            }
        }

        if (prefabToSpawn != null)
        {
            HandleCharacterUnlock(itemName, prefabToSpawn);
        }
        else if (!isOutfit)
        {
            Debug.LogWarning($"[NFC] No matching configuration found for tag: {tagIdString}");
            onNFCError.Invoke();
        }
    }

    private void HandleCharacterUnlock(string itemName, GameObject prefab)
    {
        Debug.Log($"[NFC] Handling character unlock for: {itemName}");

        if (SaveManager.Instance.IsCharacterUnlocked(itemName))
        {
            Debug.Log($"[NFC] Character already unlocked: {itemName}");
            onNFCAlreadyUnlocked.Invoke(itemName);
        }
        else
        {
            Debug.Log($"[NFC] Unlocking new character: {itemName}");
            SaveManager.Instance.UnlockCharacter(itemName);
            Instantiate(prefab, spawnPoint.position, Quaternion.identity);
            onNFCSuccess.Invoke(itemName);
        }
    }

    private void HandleOutfitUnlock(string outfitName)
    {
        Debug.Log($"[NFC] Handling outfit unlock for: {outfitName}");

        if (SaveManager.Instance.IsCosmeticUnlocked(outfitName))
        {
            Debug.Log($"[NFC] Outfit already unlocked: {outfitName}");
            onNFCAlreadyUnlocked.Invoke(outfitName);
        }
        else
        {
            Debug.Log($"[NFC] Unlocking new outfit: {outfitName}");
            SaveManager.Instance.UnlockCosmetic(outfitName);

            // Update closet UI
            ClosetTrigger closet = FindObjectOfType<ClosetTrigger>();
            if (closet != null)
            {
                Debug.Log($"[NFC] Found closet trigger, updating UI");
                closet.RefreshCosmeticDisplay();
            }
            else
            {
                Debug.LogWarning($"[NFC] No ClosetTrigger found in scene");
            }

            onNFCSuccess.Invoke(outfitName);
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