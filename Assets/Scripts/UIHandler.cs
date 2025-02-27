using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class NFCUIHandler : MonoBehaviour
{
    [SerializeField] private Button addCharacterButton;
    [SerializeField] private GameObject nfcScanPromptPanel;
    [SerializeField] private Button cancelButton;
    [SerializeField] private TMP_Text contextText;
    [SerializeField] private TMP_Text cancelButtonText;
    [SerializeField] private NfcReader nfcReader;
    [SerializeField] private GameObject nfcTagReaderHandler;

    private string originalContextText;


    private void Awake()
    {
        originalContextText = contextText.text;
        addCharacterButton.onClick.AddListener(ShowScanPanel);
        cancelButton.onClick.AddListener(HideScanPanel);
        nfcReader.onNFCSuccess.AddListener(OnScanSuccess);
        nfcReader.onNFCError.AddListener(OnScanError);
        nfcScanPromptPanel.SetActive(false);
        nfcTagReaderHandler.SetActive(false);
        nfcReader.onNFCAlreadyUnlocked.AddListener(OnScanAlreadyUnlocked);
    }

    private void OnScanAlreadyUnlocked(string itemName)
    {
        contextText.text = $"{itemName} already unlocked!";
        StartCoroutine(RetryScan());
    }

    private void ShowScanPanel()
    {
        nfcScanPromptPanel.SetActive(true);
        nfcTagReaderHandler.SetActive(true);
        contextText.text = originalContextText;
        cancelButtonText.text = "Cancel";
    }

    private void HideScanPanel()
    {
        nfcScanPromptPanel.SetActive(false);
        nfcTagReaderHandler.SetActive(false);
        contextText.text = originalContextText;
        cancelButtonText.text = "Cancel";
    }

    private void OnScanSuccess(string itemName)
    {
        contextText.text = $"Success! Unlocked {itemName}!";
        cancelButtonText.text = "Close";
    }

    private void OnScanError()
    {
        contextText.text = "Failed! Could not read NFC tag!";
        StartCoroutine(RetryScan());
    }

    private IEnumerator RetryScan()
    {
        yield return new WaitForSeconds(1f);
        contextText.text = originalContextText;
    }

    private void OnDestroy()
    {
        addCharacterButton.onClick.RemoveListener(ShowScanPanel);
        cancelButton.onClick.RemoveListener(HideScanPanel);
        nfcReader.onNFCSuccess.RemoveListener(OnScanSuccess);
        nfcReader.onNFCError.RemoveListener(OnScanError);
        nfcReader.onNFCAlreadyUnlocked.RemoveListener(OnScanAlreadyUnlocked);
    }
}