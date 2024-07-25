using Supermarket.Pricing;
using Supermarket.Products;
using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PriceSettingView : MonoBehaviour
{
    public Image icon;
    public TextMeshProUGUI productName;
    public TextMeshProUGUI productCost;
    public TextMeshProUGUI productPrice;
    public TextMeshProUGUI marketPrice;
    public TextMeshProUGUI profit;

    [Space]
    public Button PlusButton;
    public Button MinusButton;
    public Slider slider;

    [SerializeField, Range(0.05f, 0.1f)] 
    private float deltaValue = 0.01f;
    [NonEditable, SerializeField] 
    private float currentPrice;
    [Space]
    public Button OkButton;


    ItemPricing item;
    Action<float> OnOk;

    private void Awake()
    {
        PlusButton.onClick.AddListener(Plus);
        MinusButton.onClick.AddListener(Minus);

        slider.onValueChanged.AddListener(SetValue);

        OkButton.onClick.AddListener(OnOK);
    }

    public void Set(ProductInfo productInfo, Action<float> OnOk)
    {
        SupermarketManager supermarket = SupermarketManager.Mine;

        item =  supermarket.GetItemPricing(productInfo);
        currentPrice = item.price;

        icon.sprite = productInfo.Icon;
        productName.text = productInfo.name;
        productCost.text = productInfo.UnitCost.ToString();
        productPrice.text = item.price.ToString();
        
        marketPrice.text = $"MARKET PRICE: {productInfo.MarketPrice}";
        profit.text = (currentPrice - productInfo.UnitCost).ToString();

        slider.minValue = 0;
        slider.maxValue = productInfo.UnitCost + 5;
        slider.value = currentPrice;

        this.OnOk = OnOk;

        gameObject.SetActive(true);
    }

    void SetValue(float val)
    {
        currentPrice = Mathf.Clamp(val, slider.minValue, slider.maxValue);
        currentPrice = Mathf.Round(currentPrice * 100f) / 100f;
        productPrice.text = currentPrice.ToString();
    }

    void Plus()
    {
        SetValue(currentPrice += deltaValue);
    }

    void Minus()
    {
        SetValue(currentPrice -= deltaValue);
    }

    void OnOK()
    {
        OnOk?.Invoke(currentPrice);
        OnOk = null;
        gameObject.SetActive(false);
    }
}
