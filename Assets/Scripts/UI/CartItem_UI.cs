using Supermarket;
using Supermarket.Products;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class CartItem_UI : MonoBehaviour
{
    public Image icon;
    public TextMeshProUGUI productName;
    public TextMeshProUGUI info;

    public TextMeshProUGUI amountText;
    public TextMeshProUGUI totalText;


    public UnityAction AmoutChanged { get; set; }

    [NonEditable]
    public int amount;

    public Company company;

    public void Load(CartItem cartItem)
    {
        ProductInfo productInfo = cartItem.product;
        company = productInfo.Company;
        icon.sprite = productInfo.Icon;
        productName.text = productInfo.name;
        amount = cartItem.amount;
        amountText.text = amount.ToString();
        totalText.text = (productInfo.UnitCost * cartItem.amount).ToString();
        info.text = $"Unit: {productInfo.UnitPerPack}\nUnit Price: {productInfo.UnitCost}\nStorage:";
    }


    public UnityEvent<CartItem_UI> OnRemove;

    public void Remove()
    {
        gameObject.SetActive(false);
        OnRemove?.Invoke(this);
    }
}