using System;
using System.Runtime.CompilerServices;
using Hieki.Pubsub;
using Hieki.Utils;
using UnityEngine;

namespace Supermarket.Products
{
    public abstract class Furniture : Interactable, IInteractButton01, IInteractButton02
    {
        public static UnitPool<Type, Furniture> Pool = new UnitPool<Type, Furniture>(2);

        public FurnitureInfo FurnitureInfo => furnitureInfo;
        [SerializeField, NewProduct] protected FurnitureInfo furnitureInfo;

        public FurnitureState state 
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get;
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            private set; 
        }

        public enum FurnitureState
        {
            Normal,
            Moving,
        }


        public Transform playerTrans 
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get; 
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            set; 
        }

        public override void OnHoverEnter(Transform playerTrans)
        {
            this.playerTrans = playerTrans;
            outline.enabled = true;
        }

        public virtual void OnGet(Transform playerTransform)
        {

        }

        public virtual void OnMoveBegin() { }

        public virtual void OnMoveDone() { }

        //------------------------------------MOVE--------------------------------\\
        public void Move()
        {
            this.Publish(moveTopic, new MoveMessage(this));
        }

        private void TrySell()
        {
            this.Publish(trySellTopic, new TrySellNotify(Sell));
        }

        private void Sell()
        {
            this.Publish(sellTopic, new SellMessage(this));
        }


        //----------------------------------- Button 01: Move -------------------------------\\
        public bool GetButtonState01()
        {
            return true;
        }

        public string GetButtonTitle01()
        {
            return "Move";
        }

        public void OnClick_Button01()
        {
            Move();
        }

        //----------------------------------- Button 02: Sell -------------------------------\\

        public bool GetButtonState02()
        {
            return true;
        }

        public string GetButtonTitle02()
        {
            return "Sell";
        }

        public void OnClick_Button02()
        {
            TrySell();
        }

        public static readonly Topic trySellTopic = Topic.FromMessage<TrySellNotify>();
        public static readonly Topic sellTopic = Topic.FromMessage<SellMessage>();
        public static readonly Topic moveTopic = Topic.FromMessage<MoveMessage>();

        public readonly struct TrySellNotify : IMessage
        {
            public readonly Action OnConfirm;
            public TrySellNotify(Action OnConfirm)
            {
                this.OnConfirm = OnConfirm;
            }
        }

        public readonly struct SellMessage : IMessage
        {
            public readonly Furniture furniture;
            public SellMessage(Furniture furniture)
            {
                this.furniture = furniture;
            }
        }

        public readonly struct MoveMessage : IMessage
        {
            public readonly Furniture furniture;
            public MoveMessage(Furniture furniture)
            {
                this.furniture = furniture;
            }
        }

#if UNITY_EDITOR
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        static void Init()  //domain reloading
        {
            Pool = new UnitPool<Type, Furniture>(2);
        }

        protected virtual void OnDrawGizmosSelected()
        {
            switch(FurnitureInfo.Shape)
            {
                case FurnitureInfo.FurnitureShape.Box:
                    SceneDrawer.DrawWireCubeGizmo(transform.position, FurnitureInfo.Size, transform.rotation, Color.blue);
                    break;
                case FurnitureInfo.FurnitureShape.Circle:
                    UnityEditor.Handles.DrawWireArc(transform.position, Vector3.up, Vector3.forward, 360, FurnitureInfo.Radius);
                    break;
            }
        }
#endif
    }
}
