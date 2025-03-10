using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class OutfitCardUI : MonoBehaviour
{
    [SerializeField] private TMP_Text outfitNameText;
    [SerializeField] private Button selectButton;

    private string outfitId;
    private System.Action<string> selectCallback;

    public void Initialize(string outfitId, System.Action<string> callback)
    {
        this.outfitId = outfitId;
        this.selectCallback = callback;
        outfitNameText.text = outfitId;

        selectButton.onClick.AddListener(OnSelect);
    }

    private void OnSelect()
    {
        selectCallback?.Invoke(outfitId);
    }
}