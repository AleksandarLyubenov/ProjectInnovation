using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObjects/NFC Tag UIDs", fileName = "NFCTagUID")]
public class NFCTagUID : ScriptableObject
{
    [System.Serializable]
    public class TagData
    {
        public string uid;
        public string itemName;
    }

    public List<TagData> tagData = new List<TagData>();
}