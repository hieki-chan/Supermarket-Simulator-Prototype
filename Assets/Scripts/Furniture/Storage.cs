using Supermarket.Products;
using UnityEngine;
using Hieki.Utils;

namespace Supermarket.Customers
{
    public sealed class Storage : Furniture
    {
        static MonoPool<PriceTag> PriceTagPool;

        public ArrangementGrid[] grids;

        [SerializeField] private PriceTag priceTagPrefab;
        [SerializeField] private Vector3 priceTagPositionOffset;

        PriceTag[] activePriceTagsSelf;


        protected sealed override void Awake()
        {
            base.Awake();
            PriceTagPool ??= new MonoPool<PriceTag>(priceTagPrefab, 9);
            activePriceTagsSelf = new PriceTag[grids.Length];
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
                        PriceTag tag = PriceTagPool.GetOrCreate(priceTagPrefab);
                        //tag.transform.parent = gridTrans;
                        tag.transform.SetPositionAndRotation(Position.Offset(gridTrans, priceTagPositionOffset), transform.rotation);
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

        public bool IsAvailable() => CheckProduct() != null;

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
                realIndex = loopIndex >= count ? loopIndex - count : loopIndex;
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

                    if (grid.Count == 0 && activePriceTagsSelf[realIndex] != null)
                    {
                        PriceTagPool.Return(activePriceTagsSelf[realIndex]);
                        activePriceTagsSelf[realIndex] = null;
                    }

                    return product;
                }

                loopIndex++;
                realIndex = loopIndex >= count ? loopIndex - count : loopIndex;
            }

            return null;
        }

        public override void OnMoveBegin()
        {
            foreach (var tag in activePriceTagsSelf)
            {
                if(tag == null)
                    continue;
                tag.gameObject.SetActive(false);
            }
        }

        public override void OnMoveDone()
        {
            for (int i = 0; i < activePriceTagsSelf.Length; i++)
            {
                PriceTag tag = activePriceTagsSelf[i];
                if (tag == null)
                    continue;
                tag.gameObject.SetActive(true);
                tag.transform.SetPositionAndRotation(Position.Offset(grids[i].transform, priceTagPositionOffset), transform.rotation);
            }
        }


#if UNITY_EDITOR
        protected override void OnDrawGizmosSelected()
        {
            base.OnDrawGizmosSelected();
            foreach (var grid in grids)
            {
                grid.OnDrawGizmo();
            }
        }

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        static void Init()  //domain reloading
        {
            PriceTagPool = null;
        }
#endif
    }
}