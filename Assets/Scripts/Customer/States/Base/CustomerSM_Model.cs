using System;
using System.Reflection;
using System.Collections.Generic;
using Supermarket.Customers;
using Supermarket.Products;
using Hieki.AI.State;
using UnityEngine;
using State = CustomerStateBase;
using System.Runtime.CompilerServices;

[Serializable]
public class CustomerSM_Model : StateMachine<State>
{
    public Customer customer
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => _customer;
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        set => _customer = value;
    }
    public Customer _customer;


    [NonEditable]
    public int currentNode;

    [NonEditable] public Vector3 targetPosition;


    public Storage storage
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => _storage;
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        set => _storage = value;
    }
    [SerializeField] private Storage _storage;

    public bool isShopping
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => _isShopping;
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        set => _isShopping = value;
    }
    [SerializeField] private bool _isShopping;

    public int chooseCount
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => _chooseCount;
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        set => _chooseCount = value;
    }
    [SerializeField] private int _chooseCount;

    public List<ProductOnSale> productsInBag
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => _productsInBag;
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        set => _productsInBag = value;
    }

    [SerializeField] private List<ProductOnSale> _productsInBag = new List<ProductOnSale>();


#if UNITY_EDITOR
    [NonEditable, SerializeField] string state = "";
#endif

    protected Dictionary<Type, State> stateDict = new Dictionary<Type, State>();

    public virtual void CreateStates(object data)
    {
        Type[] stateTypes = Assembly.GetAssembly(typeof(State)).GetTypes();

        for (int i = 0; i < stateTypes.Length; i++)
        {
            Type type = stateTypes[i];
            if (!type.IsSubclassOf(typeof(State)) || type.IsAbstract)
                continue;

            var state = Activator.CreateInstance(type, data);
            stateDict.Add(type, (State)state);

            // Debug.Log(state);
        }
    }

    public T GetState<T>() where T : State, new()
    {
        if (stateDict.TryGetValue(typeof(T), out var state))
        {
            return (T)state;
        }

        T newState = new T();
        newState.SM = this;

        stateDict.Add(typeof(T), newState);

        //Debug.Log($"State {typeof(S)} not found");

        return newState;
    }

    public T SwitchState<T>() where T : State, new()
    {
        currentState?.OnStateExit();
        T state = GetState<T>();
        currentState = state;
        state.OnStateEnter();
        return state;
    }

    public override void Start()
    {
        productsInBag = new List<ProductOnSale>(Customer.MAX_PRODUCTS);

        SwitchState<WalkingState>();
    }

#if UNITY_EDITOR
    public override void UpdateState()
    {
        base.UpdateState();
        state = currentState?.ToString();
    }
#endif
}