using UnityEngine;

namespace Supermarket.Customers
{
    public abstract class CustomerStateBase : IState
    {
        protected readonly Customer Customer;
        protected Transform Transform => Customer.transform;

        public /*readonly*/ CustomerTransition[] transitions;

        public CustomerStateBase(Customer Customer)
        {
            this.Customer = Customer;
        }


        public virtual void OnStateEnter() { }

        public virtual void OnStateUpdate() { }

        public virtual void OnStateExit() { }


        public T IsState<T>() where T : CustomerStateBase
        {
            return this as T;
        }

        public bool IsState<T>(out T state) where T : CustomerStateBase
        {
            return state = this as T;
        }

        protected void print(object message, Object context = null)
        {
#if UNITY_EDITOR
            Debug.Log(message, context);
#endif
        }

        protected void print(object message, CustomerStateBase context = null)
        {
#if UNITY_EDITOR
            Debug.Log(message, context?.Customer);
#endif
        }

        public static implicit operator bool(CustomerStateBase state)
        {
            return state != null;
        }
    }
}