using Supermarket.Player;
using System;
using UnityEngine;

namespace Supermarket.Products
{
    public  abstract class Furniture : Interactable
    {
        public static Pool<Type, Furniture> Pool = new Pool<Type, Furniture>(2);

        public virtual void OnGet(PlayerController player)
        {

        }

#if UNITY_EDITOR
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        static void Init()  //domain reloading
        {
            Pool = new Pool<Type, Furniture>(2);
        }
#endif
    }
}
