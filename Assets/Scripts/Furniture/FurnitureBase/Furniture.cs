using Supermarket.Player;
using System;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace Supermarket.Products
{
    public abstract class Furniture : Interactable, IInteractButton01, IInteractButton02
    {
        public static UnitPool<Type, Furniture> Pool = new UnitPool<Type, Furniture>(2);

        [SerializeField, NewProduct] protected ProductInfo furnitureInfo;

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


        protected PlayerController player 
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get; 
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            set; 
        }

        public override void OnHoverEnter(Transform playerTrans)
        {
            outline.enabled = true;
        }

        public virtual void OnGet(PlayerController player)
        {

        }

        //------------------------------------MOVE--------------------------------\\

        public void Move()
        {
            if (player.currentInteraction == this)
            {
                MoveDone();
                return;
            }
            player.currentInteraction = this;
            transform.parent = player.transform;
            Vector3 fwd = player.transform.position + player.transform.forward * 2;
            transform.position = new Vector3(fwd.x, .014f, fwd.z);
            state = FurnitureState.Moving;
        }

        private void MoveDone()
        {
            player.currentInteraction = null;
            transform.parent = null;
            state = FurnitureState.Normal;
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
            
        }

#if UNITY_EDITOR
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        static void Init()  //domain reloading
        {
            Pool = new UnitPool<Type, Furniture>(2);
        }
#endif
    }
}
