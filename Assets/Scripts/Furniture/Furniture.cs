using Supermarket.Player;
using System;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace Supermarket.Products
{
    public abstract class Furniture : Interactable, IInteractButton01, IInteractButton02, IInteractButton03
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

        public override void OnHoverEnter(PlayerController player)
        {
            outline.enabled = true;
            this.player = player;
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

        //-----------------------------------ROTATION-------------------------------\\

        void RotateLeft90()
        {
            transform.rotation = Quaternion.Euler(0, transform.eulerAngles.y + 90, 0);
        }

        void RotateRight90()
        {
            transform.rotation = Quaternion.Euler(0, transform.eulerAngles.y - 90, 0);
        }

        //----------------------------------------------------INTERACTING----------------------------------------------\\

        //----------------------------------- Button 01: Move/Ok -------------------------------\\
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

        //----------------------------------- Button 02: Rotate Left 90 -------------------------------\\

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

        //-----------------------------------Button 03: Rotate Right 90 -------------------------------\\

        public bool GetButtonState03()
        {
            return player.currentInteraction == this;
        }

        public string GetButtonTitle03()
        {
            return "Rotate Right 90";
        }

        public void OnClick_Button03()
        {
            RotateRight90();
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
