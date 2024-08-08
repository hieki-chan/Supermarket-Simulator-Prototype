using Supermarket.Products;
using UnityEngine;
using Hieki.Utils;
using Supermarket.Player;

namespace Supermarket.Customers
{
    public sealed class Storage : Furniture
    {
        static MonoPool<PriceTag> PriceTagPool;

        public ArrangementGrid[] grids;

        [SerializeField] private PriceTag priceTagPrefab;
        [SerializeField] private Vector3 priceTagPositionOffse;

        PriceTag[] activePriceTagsSelf;


        protected sealed override void Awake()
        {
            base.Awake();
            PriceTagPool ??= new MonoPool<PriceTag>(priceTagPrefab, 9);
            activePriceTagsSelf = new PriceTag[grids.Length];
        }

        public sealed override void OnHoverEnter(Transform playerTrans)
        {
            base.OnHoverEnter(playerTrans);
            this.player = player;
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
                    if (!isSuccess)
                        continue;

                    if (activePriceTagsSelf[i] == null)
                    {
                        PriceTag tag = PriceTagPool.GetOrCreate(priceTagPrefab, Position.Offset(gridTrans, priceTagPositionOffse), transform.rotation);
                        //tag.transform.parent = gridTrans;
                        tag.transform.position += transform.forward;
                        tag.productInfo = product.ProductInfo;
                        activePriceTagsSelf[i] = tag;
                        PriceTag.ActivePriceTags.Add(tag);
                    }

                    product.transform.parent = transform;
                    product.transform.localRotation = Quaternion.identity;
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

                    if (grid.Count == 0 && activePriceTagsSelf[i] != null)
                    {
                        PriceTagPool.Return(activePriceTagsSelf[i]);
                        activePriceTagsSelf[i] = null;
                    }

                    return product;
                }

                loopIndex++;
                realIndex = (int)Mathf.Repeat(loopIndex, count - 1);
            }

            return null;
        }

        public override void OnGet(PlayerController player)
        {
            this.player = player;
            Move();
        }


#if UNITY_EDITOR
        private void OnDrawGizmosSelected()
        {
            foreach (var grid in grids)
            {
                grid.OnDrawGizmo();
            }
        }

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        static void Init()  //domain reloading
        {
            PriceTagPool = new MonoPool<PriceTag>();
        }
#endif
    }
}