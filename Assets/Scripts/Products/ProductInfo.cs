using Supermarket.Pricing;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace Supermarket.Products
{
    [CreateAssetMenu(fileName = "New Product", menuName = "Products")]
    public class ProductInfo : ScriptableObject
    {
        public Sprite Icon { get => m_Icon; }
        [SerializeField] private Sprite m_Icon;

        //public string Name => m_Name;
        //[SerializeField] private string m_Name;

        public ProductType ProductType
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => m_ProductType;
        }
        [SerializeField] private ProductType m_ProductType;

        public unit UnitCost
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => m_UnitCost;
        }
        [SerializeField] private unit m_UnitCost;

        public unit MarketPrice
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => m_MarketPrice;
        }
        [SerializeField] private unit m_MarketPrice;

        public int UnitPerPack
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => m_UnitPerPack;
        }
        [SerializeField] private int m_UnitPerPack;

        public Company Company
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => m_Company;
        }
        [SerializeField] private Company m_Company;

        public ProductOnSale ProductOnSale
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => m_ProductOnSale;
        }
        [SerializeField] ProductOnSale m_ProductOnSale;

        public Furniture Furniture
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => m_Furniture;
        }
        [SerializeField] Furniture m_Furniture;

#if UNITY_EDITOR
        private void OnValidate()
        {
            m_UnitCost.Rounded();
            m_MarketPrice.Rounded();

            switch (m_ProductType)
            {
                case ProductType.Products:
                    m_UnitPerPack = Mathf.Clamp(m_UnitPerPack, 0, 99);
                    break;
                case ProductType.Furniture:
                    m_UnitPerPack = 1;
                    break;
                default:
                    break;
            }
        }
#endif
    }

    public enum ProductType
    {
        Products,
        Furniture,
    }
}
