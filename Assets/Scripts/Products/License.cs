using Supermarket.Pricing;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace Supermarket.Products
{
    [CreateAssetMenu(fileName = "License", menuName = "Licenses")]
    public class License : ScriptableObject
    {
        public string LicenseName
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => m_LicenseName;
        }
        [SerializeField, TextArea] private string m_LicenseName;

        public string Description
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => m_Description;
        }
        [SerializeField, TextArea] private string m_Description;

        public unit Cost
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => m_Cost;
        }
        [SerializeField] private unit m_Cost;

        public int StoreLeveRequired
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => m_RequireStoreLevel;
        }
        [SerializeField] private int m_RequireStoreLevel;



        public List<ProductInfo> AllowedPurchaseAndSellProducts => m_AllowedPurchaseAndSellProducts;
        [SerializeField] private List<ProductInfo> m_AllowedPurchaseAndSellProducts = new List<ProductInfo>();
    }
}