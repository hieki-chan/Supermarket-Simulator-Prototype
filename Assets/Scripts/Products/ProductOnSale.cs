using UnityEngine;

namespace Supermarket.Products
{
    public sealed class ProductOnSale : Interactable
    {
        public static Pool<string, ProductOnSale> Pool = new Pool<string, ProductOnSale>(9);
        public ProductInfo ProductInfo => productInfo;
        [SerializeField]
        private ProductInfo productInfo;

        [SerializeField] private Vector2 worldSize;

        Collider cols;

        protected override void Awake()
        {
            base.Awake();
            cols = GetComponent<Collider>();
        }

        private void OnEnable()
        {
            outline.enabled = false;
            cols.enabled = false;
        }

        public Vector2 GetWorldSize()
        {
            return worldSize;
        }

        public override void OnHoverEnter()
        {
            return;
        }

        public override void OnHoverExit()
        {
            return;
        }

        public void EnableInteracttion()
        {
            outline.enabled = true;
            cols.enabled = true;
        }

#if UNITY_EDITOR
        private void OnDrawGizmosSelected()
        {
            Gizmos.DrawWireCube(transform.position, new Vector3(worldSize.x, (worldSize.y + worldSize.x) / 2, worldSize.y));
        }

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        static void Init()  //domain reloading
        {
            Pool = new Pool<string, ProductOnSale>(9);
        }
#endif
    }
}