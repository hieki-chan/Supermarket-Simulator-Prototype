using UnityEngine;
using Supermarket.Customers;
using Hieki.AI.State;
using System.Runtime.CompilerServices;

public class CustomerStateBase : IState<CustomerSM_Model>
{
    public CustomerSM_Model SM
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get;
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        set;
    }
    public Customer customer
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => SM.customer;
    }

    public CustomerStateBase()
    {

    }

    public CustomerStateBase(CustomerSM_Model SM_model)
    {
        //Debug.Log(SM_model);
        SM = SM_model;
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

    protected void print(object message)
    {
#if UNITY_EDITOR
        Debug.Log(message);
#endif
    }

    public static implicit operator bool(CustomerStateBase state)
    {
        return state != null;
    }
}