using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(menuName = "ScriptableObjects/Unlockables Tracker", fileName = "UnlockablesTracker")]
public class UnlockablesTracker : ScriptableObject
{
    [SerializeField] private List<string> unlockedItems = new List<string>();

    public bool IsUnlocked(string itemName)
    {
        return unlockedItems.Contains(itemName);
    }

    public void Unlock(string itemName)
    {
        if (!unlockedItems.Contains(itemName))
        {
            unlockedItems.Add(itemName);
        }
    }
}