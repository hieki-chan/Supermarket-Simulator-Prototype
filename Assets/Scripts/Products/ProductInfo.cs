using Supermarket.Pricing;
using UnityEngine;

namespace Supermarket.Products
{
    [CreateAssetMenu(fileName = "New Product", menuName = "Products")]
    public class ProductInfo : ScriptableObject
    {
        public Sprite Icon => m_Icon;
        [SerializeField] private Sprite m_Icon;

        //public string Name => m_Name;
        //[SerializeField] private string m_Name;

        public ProductType ProductType => m_ProductType;
        [SerializeField] private ProductType m_ProductType;

        public StandardCurrency UnitCost => m_UnitCost;
        [SerializeField] private StandardCurrency m_UnitCost;

        public StandardCurrency MarketPrice => m_MarketPrice;
        [SerializeField] private StandardCurrency m_MarketPrice;

        public int UnitPerPack => m_UnitPerPack;
        [SerializeField] private int m_UnitPerPack;

        public Company Company => m_Company;
        [SerializeField] private Company m_Company;

        public ProductOnSale ProductOnSale => m_ProductOnSale;
        [SerializeField] ProductOnSale m_ProductOnSale;

#if UNITY_EDITOR
        private void OnValidate()
        {
            m_UnitCost.Rounded();
        }
#endif
    }

    public enum ProductType
    {
        Products,
        Furniture,
    }
}
