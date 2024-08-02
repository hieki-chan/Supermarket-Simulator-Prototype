using Supermarket.Products;
using UnityEngine;
using Hieki.Utils;
using Supermarket.Player;

namespace Supermarket.Customers
{
    public class Storage : Furniture, IInteractButton01, IInteractButton02
    {
        static SimplePool<PriceTag> PriceTagPool;

        [SerializeField] private ProductInfo furnitureInfo;
        public ArrangementGrid[] grids;

        [SerializeField] private PriceTag priceTagPrefab;
        [SerializeField] private Vector3 priceTagPositionOffse;

        PriceTag[] activePriceTagsSelf;

        public State state { get; private set; }

        [Header("Rotation")]
        [SerializeField] private float range;
        PlayerController player;

        public enum State
        {
            Normal,
            Moving,
        }

        protected override void Awake()
        {
            base.Awake();
            PriceTagPool ??= new SimplePool<PriceTag>(priceTagPrefab, 9);
            activePriceTagsSelf = new PriceTag[grids.Length];
        }

        public override void OnHoverEnter(PlayerController player)
        {
            base.OnHoverEnter(player);
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

        public void Move()
        {
            if (player.currentInteraction == this)
            {
                MoveDone();
                return;
            }
            player.currentInteraction = this;
            transform.parent = player.transform;
            Vector3 fwd = player.transform.position + player.transform.forward * range;
            transform.position = new Vector3(fwd.x, transform.position.y, fwd.z);
            state = State.Moving;
        }

        //ROTATION

        public bool GetButtonState01()
        {
            return true;
        }

        public string GetButtonTitle01()
        {
            return player.currentInteraction != this ? "Move" : "Ok";
        }

        public void OnClick_Button01()
        {
            Move();
        }

        public bool GetButtonState02()
        {
            return player.currentInteraction == this;
        }

        public string GetButtonTitle02()
        {
            return "Rotate Left 90";
        }

        public void OnClick_Button02()
        {
            RotateLeft90();
        }

        void MoveDone()
        {
            player.currentInteraction = null;
            transform.parent = null;
            state = State.Normal;
        }

        void RotateLeft90()
        {
            transform.rotation = Quaternion.Euler(0, transform.eulerAngles.y + 90, 0);
        }

        void RotateRight90()
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