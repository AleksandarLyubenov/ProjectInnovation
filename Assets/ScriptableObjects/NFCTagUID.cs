using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObjects/NFC Tag UIDs", fileName = "NFCTagUID")]
public class NFCTagUID : ScriptableObject
{
    [Tooltip("NFC tag UID (Hex)")]
    public List<string> tagUIDs = new List<string>();
}
