using System.Collections.Generic;

[System.Serializable]
public class PlayerData
{
    public List<string> unlockedCharacters = new List<string>();
    public List<string> unlockedCosmetics = new List<string>();
    public int sanity = 100;
    public int playerLevel = 1;
    public int cleanliness = 100;
    public int energy = 100;
    public int hunger = 100;
    public int minigame1HighScore = 0;
    public int minigame2HighScore = 0;

    public PlayerData()
    {
        // Initialize default values
        sanity = 50;
        playerLevel = 1;
        cleanliness = 50;
        energy = 6;
        hunger = 50;
        minigame1HighScore = 0;
        minigame2HighScore = 0;
        unlockedCharacters.Clear();
        unlockedCosmetics.Clear();
    }
}