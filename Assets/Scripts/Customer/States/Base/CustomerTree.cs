using Hieki.AI;
using UnityEngine;

namespace Supermarket.Customers
{
    public class CustomerTree : BehaviourTree<Customer>
    {
        public /*readonly*/ CustomerTransition[] transitions;


        public CustomerTree(Root root)
        {
            this.root = root;
        }

        public override void InitTree(Customer component) { }


        public virtual void OnStateEnter() { }

        public virtual void OnStateUpdate() { }

        public virtual void OnStateExit() { }


        public T IsState<T>() where T : CustomerTree
        {
            return this as T;
        }

        public bool IsState<T>(out T state) where T : CustomerTree
        {
            return state = this as T;
        }

        protected void print(object message, Object context = null)
        {
#if UNITY_EDITOR
            Debug.Log(message, context);
#endif
        }

        protected void print(object message, CustomerTree context = null)
        {
#if UNITY_EDITOR
            Debug.Log(message);
#endif
        }

        public static implicit operator bool(CustomerTree state)
        {
            return state != null;
        }
    }
}