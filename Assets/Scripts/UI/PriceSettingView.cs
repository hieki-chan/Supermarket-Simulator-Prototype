using Supermarket.Pricing;
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

    [SerializeField, Range(0.01f, 0.1f)] 
    private float deltaValue = 0.01f;
    [NonEditable, SerializeField] 
    private unit currentPrice;
    [Space]
    public Button OkButton;

    ItemPricing itemPricing;

    Action<unit> OnOk;

    private void Awake()
    {
        PlusButton.onClick.AddListener(Plus);
        MinusButton.onClick.AddListener(Minus);

        slider.onValueChanged.AddListener(SetValue);

        OkButton.onClick.AddListener(OnOK);
    }

    public void Set(ItemPricing itemPricing, Action<unit> OnOk)
    {
        this.itemPricing = itemPricing;
        currentPrice = itemPricing.price;

        icon.sprite = itemPricing.product.Icon;
        productName.text = itemPricing.product.name;
        productCost.text = itemPricing.product.UnitCost.ToString();
        productPrice.text = itemPricing.price.ToString();
        
        marketPrice.text = $"MARKET PRICE: {itemPricing.product.MarketPrice}";
        profit.text = (currentPrice - itemPricing.product.UnitCost).ToString();

        slider.minValue = 0;
        slider.maxValue = (itemPricing.product.MarketPrice * 2.15f).Rounded();
        slider.value = currentPrice;

        this.OnOk = OnOk;

        gameObject.SetActive(true);
    }

    void SetValue(float val)
    {
        currentPrice = Mathf.Clamp(val, slider.minValue, slider.maxValue);
        currentPrice = Mathf.Round(currentPrice * 100f) / 100f;
        productPrice.text = currentPrice.ToString();
        profit.text = (currentPrice - itemPricing.product.UnitCost).ToString();
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
