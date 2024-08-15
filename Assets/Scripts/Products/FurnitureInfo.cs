using UnityEngine;

namespace Supermarket.Products
{
    public class FurnitureInfo : ProductInfo
    {
        public GameObject PlaceEffect => m_PlaceEffect;
        [SerializeField] private GameObject m_PlaceEffect;

        public FurnitureShape Shape => m_Shape;
        [SerializeField] private FurnitureShape m_Shape;

        public Vector3 Size => m_Size;
        [SerializeField] private Vector3 m_Size;

        public float Radius => m_Radius;
        [SerializeField] private float m_Radius;

        public enum FurnitureShape
        {
            Box,
            Circle,
        }
    }
}
