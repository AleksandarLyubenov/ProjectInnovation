using UnityEngine;
using System.IO;
using System;
using System.Collections.Generic;

public class SaveManager : MonoBehaviour
{
    public static SaveManager Instance;

    private PlayerData playerData;
    private string savePath;

    public event Action OnSanityChanged;
    public event Action OnEnergyChanged;
    public event Action OnCleanlinessChanged;
    public event Action OnHungerChanged;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            savePath = Application.persistentDataPath + "/playerData.json";
            LoadData();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void DeleteSaveData()
    {
        if (File.Exists(savePath))
        {
            File.Delete(savePath);
            Debug.Log("Deleted save file at: " + savePath);
        }

        // Reset in-memory data
        playerData = new PlayerData();
        Debug.Log("Reset all player data to defaults");
    }

    private void LoadData()
    {
        if (File.Exists(savePath))
        {
            string json = File.ReadAllText(savePath);
            playerData = JsonUtility.FromJson<PlayerData>(json);
        }
        else
        {
            playerData = new PlayerData();
            SaveData();
        }
    }

    public void SaveData()
    {
        string json = JsonUtility.ToJson(playerData);
        File.WriteAllText(savePath, json);
    }

    // Character methods
    public bool IsCharacterUnlocked(string characterName) =>
        playerData.unlockedCharacters.Contains(characterName);

    public void UnlockCharacter(string characterName)
    {
        if (!playerData.unlockedCharacters.Contains(characterName))
        {
            playerData.unlockedCharacters.Add(characterName);
            SaveData();
        }
    }

    // Outfit methods
    public bool IsOutfitUnlocked(string outfitName) =>
        playerData.unlockedOutfits.Contains(outfitName);

    public void UnlockOutfit(string outfitName)
    {
        if (!playerData.unlockedOutfits.Contains(outfitName))
        {
            playerData.unlockedOutfits.Add(outfitName);
            SaveData();
        }
    }

    // Stat methods
    public int GetSanity() => playerData.sanity;
    public void SetSanity(int value)
    {
        playerData.sanity = Mathf.Clamp(value, 0, 100);
        SaveData();
        OnSanityChanged?.Invoke();
    }

    public int GetPlayerLevel() => playerData.playerLevel;
    public void SetPlayerLevel(int level)
    {
        playerData.playerLevel = level;
        SaveData();
    }

    public int GetCleanliness() => playerData.cleanliness;
    public void SetCleanliness(int value)
    {
        playerData.cleanliness = Mathf.Clamp(value, 0, 100);
        SaveData();
        OnCleanlinessChanged?.Invoke();
    }

    public int GetEnergy() => playerData.energy;
    public void SetEnergy(int value)
    {
        playerData.energy = Mathf.Max(value, 0);
        SaveData();
        OnEnergyChanged?.Invoke();
    }

    public int GetHunger() => playerData.hunger;
    public void SetHunger(int value)
    {
        playerData.hunger = Mathf.Clamp(value, 0, 100);
        SaveData();
        OnHungerChanged?.Invoke();
    }

    public List<string> GetAllUnlockedCharacters() =>
    new List<string>(playerData.unlockedCharacters);

    public List<string> GetAllUnlockedOutfits() =>
        new List<string>(playerData.unlockedOutfits);
}