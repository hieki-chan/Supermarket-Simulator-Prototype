using Supermarket.Products;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class ShopProductItem : MonoBehaviour
{
    public const int MAX_AMOUNT = 99;
    public const int MIN_AMOUNT = 0;

    public Image icon;
    public TextMeshProUGUI productName;
    public TextMeshProUGUI info;
    public TextMeshProUGUI companyName;

    public TextMeshProUGUI amountText;
    public TextMeshProUGUI totalText;


    [SerializeField, NonEditable]
    private int amount;

    ProductInfo product;

    public void Load(ProductInfo productInfo)
    {
        this.product = productInfo;
        icon.sprite = productInfo.Icon;
        productName.text = productInfo.name;
        companyName.text = productInfo.Company.CompanyName;
        info.text = $"Unit: {productInfo.UnitPerPack}\nUnit Price: {productInfo.UnitCost}\nStorage:";
        amount = 1;
        amountText.text = amount.ToString();
        CalculateTotalCost();
    }

    public void OnAddAmout()
    {
        amount = Mathf.Clamp(amount + 1, amount, MAX_AMOUNT);
        amountText.text = amount.ToString();
        CalculateTotalCost();
    }

    public void OnSubtractAmount()
    {
        amount = Mathf.Clamp(amount - 1, MIN_AMOUNT, amount);
        amountText.text = amount.ToString();
        CalculateTotalCost();
    }

    private void CalculateTotalCost()
    {
        totalText.text = (product.UnitCost * product.UnitPerPack * amount).ToString();
    }

    public UnityEvent<CartItem> OnAddToCart;

    public void AddToCart()
    {
        CartItem cartItem = new CartItem()
        {
            product = product,
            amount = amount,
        };

        OnAddToCart?.Invoke(cartItem);
    }
}