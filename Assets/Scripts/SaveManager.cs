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
    public bool IsCosmeticUnlocked(string cosmeticId) =>
        playerData.unlockedCosmetics.Contains(cosmeticId);

    public void UnlockCosmetic(string cosmeticId)
    {
        if (!playerData.unlockedCosmetics.Contains(cosmeticId))
        {
            playerData.unlockedCosmetics.Add(cosmeticId);
            SaveData();
            Debug.Log($"Unlocked cosmetic: {cosmeticId}");
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

    public int GetMinigame1HighScore() => playerData.minigame1HighScore;
    public void SetMinigame1HighScore(int highScore)
    {
        playerData.minigame1HighScore = highScore;
        SaveData();
    }

    public int GetMinigame2HighScore() => playerData.minigame2HighScore;
    public void SetMinigame2HighScore(int highScore)
    {
        playerData.minigame2HighScore = highScore;
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
        int maxEnergy = SaveManager.Instance.CalculateMaxEnergy();
        playerData.energy = Mathf.Clamp(value, 0, maxEnergy);
        SaveData();
        OnEnergyChanged?.Invoke();
    }

    public int CalculateMaxEnergy()
    {
        float cleanlinessFactor = GetCleanliness() * 0.1f;
        float hungerFactor = GetHunger() * 0.1f;
        float sanityFactor = GetSanity() * 0.1f;

        return Mathf.RoundToInt(30 + cleanlinessFactor + hungerFactor + sanityFactor);
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

    public List<string> GetAllUnlockedCosmetics() =>
        new List<string>(playerData.unlockedCosmetics);
}