using Supermarket.Pricing;
using Supermarket.Products;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "License", menuName = "Licenses")]
public class License : ScriptableObject
{
    public List<ProductInfo> AllowedPurchaseAndSellProducts => m_AllowedPurchaseAndSellProducts;
    [SerializeField] private List<ProductInfo> m_AllowedPurchaseAndSellProducts = new List<ProductInfo>();

    public StandardCurrency Price => m_Price;
    [SerializeField] private StandardCurrency m_Price;
}
