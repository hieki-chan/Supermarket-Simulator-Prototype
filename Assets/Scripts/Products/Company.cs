using UnityEngine;

namespace Supermarket.Products
{
    [CreateAssetMenu(fileName = "Company", menuName = "Company")]
    public class Company : ScriptableObject
    {
        public string CompanyName => m_CompanyName;
        [SerializeField] private string m_CompanyName;
    }
}