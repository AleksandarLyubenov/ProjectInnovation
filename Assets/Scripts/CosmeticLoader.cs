using UnityEngine;

public class CosmeticLoader : MonoBehaviour
{
    [Header("Cosmetic Paths")]
    [SerializeField] private string hat1Path = "Canvas/Outfit 1/Hat_1";
    [SerializeField] private string hat2Path = "Canvas/Outfit 1/Hat_2";
    [SerializeField] private string hat3Path = "Canvas/Outfit 1/Hat_3";

    private GameObject hat1Object;
    private GameObject hat2Object;
    private GameObject hat3Object;

    void Start()
    {
        CacheHatReferences();
        ApplyEquippedCosmetic();
    }

    void CacheHatReferences()
    {
        hat1Object = transform.Find(hat1Path)?.gameObject;
        hat2Object = transform.Find(hat2Path)?.gameObject;
        hat3Object = transform.Find(hat3Path)?.gameObject;

        if (hat1Object == null || hat2Object == null || hat3Object == null)
        {
            Debug.LogError("Missing hat references in CosmeticLoader!");
        }
    }

    void ApplyEquippedCosmetic()
    {
        if (hat1Object != null) hat1Object.SetActive(false);
        if (hat2Object != null) hat2Object.SetActive(false);
        if (hat3Object != null) hat3Object.SetActive(false);

        string equippedId = SaveManager.Instance.GetEquippedCosmetic();

        GameObject hatToActivate = equippedId switch
        {
            "Hat_2" => hat2Object,
            "Hat_3" => hat3Object,
            _ => hat1Object
        };

        if (hatToActivate != null)
        {
            hatToActivate.SetActive(true);
            Debug.Log($"Equipped cosmetic: {equippedId}");
        }
    }
}