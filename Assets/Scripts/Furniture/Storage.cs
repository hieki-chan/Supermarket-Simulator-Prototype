using Supermarket.Products;
using UnityEngine;
using Hieki.Utils;
using Supermarket.Player;

namespace Supermarket.Customers
{
    public class Storage : Interactable
    {
        static SimplePool<PriceTag> PriceTagPool;


        [SerializeField] private ProductInfo furnitureInfo;
        public ArrangementGrid[] grids;

        [SerializeField] private PriceTag priceTagPrefab;
        [SerializeField] private Vector3 priceTagPositionOffse;

        PriceTag[] activePriceTags;

        protected override void Awake()
        {
            base.Awake();
            PriceTagPool ??= new SimplePool<PriceTag>(priceTagPrefab, 9);
            activePriceTags = new PriceTag[grids.Length];
        }

        public override void OnInteract(PlayerController targetPlayer)
        {
            base.OnInteract(targetPlayer);
        }

        public bool TryArrangeProduct(ProductOnSale product, Transform tier)
        {
            for (int i = 0; i < grids.Length; i++)
            {
                var grid = grids[i];
                Transform gridTrans = grid.transform;
                if (gridTrans == tier || gridTrans == tier.parent)
                {
                    bool isSuccess = grid.Push(product, false);
                    if (activePriceTags[i] == null)
                    {
                        PriceTag tag = PriceTagPool.GetOrCreate(priceTagPrefab, Position.Offset(gridTrans, priceTagPositionOffse), transform.rotation);
                        //tag.transform.parent = gridTrans;
                        tag.transform.position += transform.forward;
                        tag.product = product;
                        activePriceTags[i] = tag;
                    }

                    if (isSuccess)
                        product.transform.parent = transform.parent;

                    return isSuccess;
                }
            }
            return false;
        }

        public ProductOnSale CheckProduct()
        {
            int count = grids.Length;
            int realIndex = Random.Range(0, count);
            int loopIndex = realIndex;

            for (int i = 0; i < count; i++)
            {
                var grid = grids[realIndex];

                if (grid.Count > 0)
                {
                    ProductOnSale product = grid.Peek();
                    return product;
                }

                loopIndex++;
                realIndex = (int)Mathf.Repeat(loopIndex, count - 1);
            }

            return null;
        }

        public ProductOnSale TakeProduct()
        {
            int count = grids.Length;
            int realIndex = Random.Range(0, count);
            int loopIndex = realIndex;

            for (int i = 0; i < count; i++)
            {
                var grid = grids[realIndex];

                if (grid.Count > 0)
                {
                    ProductOnSale product = grid.Pop();

                    if (grid.Count == 0 && activePriceTags[i] != null)
                    {
                        PriceTagPool.Return(activePriceTags[i]);
                        activePriceTags[i] = null;
                    }
                    return product;
                }

                loopIndex++;
                realIndex = (int)Mathf.Repeat(loopIndex, count - 1);
            }

            return null;
        }

        public void RotateLeft90()
        {
            transform.rotation = Quaternion.Euler(0, transform.eulerAngles.x + 90, 0);
        }

        public void RotateRight90()
        {
            transform.rotation = Quaternion.Euler(0, transform.eulerAngles.x - 90, 0);
        }

        private void OnDrawGizmosSelected()
        {
            foreach (var grid in grids)
            {
                grid.OnDrawGizmo();
            }
        }

#if UNITY_EDITOR
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        static void Init()  //domain reloading
        {
            PriceTagPool = new SimplePool<PriceTag>();
        }
#endif
    }
}