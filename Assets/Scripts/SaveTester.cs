using UnityEngine;

public class SaveTester : MonoBehaviour
{
    public static SaveTester Instance;

    [SerializeField] private int incrementAmount = 5;
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Update()
    {
        HandleSanityInput();
        HandleCleanlinessInput();
        HandleEnergyInput();
        HandleHungerInput();
        HandleLevelInput();
        HandleCharacterUnlock();
        DumpLog();
        HandleResetInput();
        HandleOutfitUnlock();
    }

    void HandleSanityInput()
    {
        if (Input.GetKeyUp(KeyCode.Alpha1))
        {
            SaveManager.Instance.SetSanity(
                SaveManager.Instance.GetSanity() + incrementAmount
            );
            PrintAllStats();
        }
        else if (Input.GetKeyUp(KeyCode.Alpha2))
        {
            SaveManager.Instance.SetSanity(
                SaveManager.Instance.GetSanity() - incrementAmount
            );
            PrintAllStats();
        }
    }

    void HandleCleanlinessInput()
    {
        if (Input.GetKeyUp(KeyCode.Alpha3))
        {
            SaveManager.Instance.SetCleanliness(
                SaveManager.Instance.GetCleanliness() + incrementAmount
            );
            PrintAllStats();
        }
        else if (Input.GetKeyUp(KeyCode.Alpha4))
        {
            SaveManager.Instance.SetCleanliness(
                SaveManager.Instance.GetCleanliness() - incrementAmount
            );
            PrintAllStats();
        }
    }

    void HandleResetInput()
    {
        if (Input.GetKeyUp(KeyCode.R))
        {
            SaveManager.Instance.DeleteSaveData();
            Debug.Log("FORCED DATA RESET!");
            PrintAllStats();
        }
    }

    void HandleEnergyInput()
    {
        if (Input.GetKeyUp(KeyCode.Alpha5))
        {
            SaveManager.Instance.SetEnergy(
                SaveManager.Instance.GetEnergy() + incrementAmount
            );
            PrintAllStats();
        }
        else if (Input.GetKeyUp(KeyCode.Alpha6))
        {
            SaveManager.Instance.SetEnergy(
                SaveManager.Instance.GetEnergy() - incrementAmount
            );
            PrintAllStats();
        }
    }

    void HandleHungerInput()
    {
        if (Input.GetKeyUp(KeyCode.Alpha7))
        {
            SaveManager.Instance.SetHunger(
                SaveManager.Instance.GetHunger() + incrementAmount
            );
            PrintAllStats();
        }
        else if (Input.GetKeyUp(KeyCode.Alpha8))
        {
            SaveManager.Instance.SetHunger(
                SaveManager.Instance.GetHunger() - incrementAmount
            );
            PrintAllStats();
        }
    }

    void HandleLevelInput()
    {
        if (Input.GetKeyUp(KeyCode.Alpha9))
        {
            SaveManager.Instance.SetPlayerLevel(
                SaveManager.Instance.GetPlayerLevel() + 1
            );
            PrintAllStats();
        }
        else if (Input.GetKeyUp(KeyCode.Alpha0))
        {
            SaveManager.Instance.SetPlayerLevel(
                Mathf.Max(SaveManager.Instance.GetPlayerLevel() - 1, 1)
            );
            PrintAllStats();
        }
    }

    void HandleCharacterUnlock()
    {
        if (Input.GetKeyUp(KeyCode.U))
        {
            string charName = "Player";
            SaveManager.Instance.UnlockCharacter(charName);
            Debug.Log("Unlocked " + charName);
            PrintAllStats();
        }
    }
    void HandleOutfitUnlock()
    {
        if (Input.GetKeyUp(KeyCode.O))
        {
            string charName = "Outfit 2";
            SaveManager.Instance.UnlockOutfit(charName);
            Debug.Log("Unlocked outfit: " + charName);
            PrintAllStats();
        }
    }

    void DumpLog()
    {
        if (Input.GetKeyUp(KeyCode.Tilde))
        {
            PrintAllStats();
        }
    }

    void PrintAllStats()
    {
        Debug.Log(
            $"SAVE UPDATED:\n" +
            $"Sanity: {SaveManager.Instance.GetSanity()}\n" +
            $"Cleanliness: {SaveManager.Instance.GetCleanliness()}\n" +
            $"Energy: {SaveManager.Instance.GetEnergy()}\n" +
            $"Hunger: {SaveManager.Instance.GetHunger()}\n" +
            $"Player Level: {SaveManager.Instance.GetPlayerLevel()}\n" +
            $"Unlocked Characters: {string.Join(", ", SaveManager.Instance.GetAllUnlockedCharacters())}\n" +
            $"Unlocked Outfits: {string.Join(", ", SaveManager.Instance.GetAllUnlockedOutfits())}\n" +
            "-----------------------------"
        );
    }
}