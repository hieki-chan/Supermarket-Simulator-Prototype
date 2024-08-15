using Supermarket.Pricing;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class License_UI : MonoBehaviour
{
    public TextMeshProUGUI header;
    public TextMeshProUGUI description;
    public TextMeshProUGUI allows;
    public TextMeshProUGUI requires;
    public TextMeshProUGUI cost;

    public Button purchaseBttn;
    public GameObject owned;

    [NonEditable] 
    public int licenseId;

    public UnityAction<int> OnPurchase { get; set; }

    private void Awake()
    {
        purchaseBttn.onClick.AddListener(OnPurchase_Button);
    }

    public void Load(int licenseId, string header, string description, int requireLevel, unit cost)
    {
        this.licenseId = licenseId;
        this.header.text = header;
        this.description.text = description;
        this.requires.text = $"Required store level: {requireLevel}";
        this.cost.text = cost;
    }

    private void OnPurchase_Button()
    {
        OnPurchase?.Invoke(licenseId);
    }

    public void OnPurchaseSuccess()
    {
        owned.SetActive(true);
        purchaseBttn.gameObject.SetActive(false);
    }
}