using System.Collections.Generic;

[System.Serializable]
public class PlayerData
{
    public List<string> unlockedCharacters = new List<string>();
    public List<string> unlockedOutfits = new List<string>();
    public int sanity = 100;
    public int playerLevel = 1;
    public int cleanliness = 100;
    public int energy = 100;
    public int hunger = 100;

    public PlayerData()
    {
        // Initialize default values
        sanity = 100;
        playerLevel = 1;
        cleanliness = 100;
        energy = 100;
        hunger = 100;
        unlockedCharacters.Clear();
        unlockedOutfits.Clear();
    }
}