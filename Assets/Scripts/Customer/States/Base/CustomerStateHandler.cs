using System;
using System.Reflection;
using System.Collections.Generic;
using UnityEngine;

namespace Supermarket.Customers
{
    [Serializable]
    public class CustomerStateHandler
    {
        protected Dictionary<Type, CustomerStateBase> stateDict = new Dictionary<Type, CustomerStateBase>();

        public void CreateStates(Customer customer)
        {
            Type[] stateTypes = Assembly.GetAssembly(typeof(CustomerStateBase)).GetTypes();

            for (int i = 0; i < stateTypes.Length; i++)
            {
                Type type = stateTypes[i];
                if (!type.IsSubclassOf(typeof(CustomerStateBase)) || type.IsAbstract)
                    continue;
                var state = Activator.CreateInstance(type, (object)customer) as CustomerStateBase;
                stateDict.Add(type, state);

                //Debug.Log(state);
            }
        }

        public T GetState<T>() where T : CustomerStateBase
        {
            if (stateDict.TryGetValue(typeof(T), out var state))
            {
                return (T)state;
            }

            //T newState = new T();
            //stateDict.Add(typeof(T), newState);

            Debug.Log($"State {typeof(T)} not found");

            return null;
        }

        public CustomerStateBase GetState(Type type)
        {
            if (stateDict.TryGetValue(type, out var state))
            {
                return state;
            }

            //T newState = new T();
            //stateDict.Add(typeof(T), newState);

            Debug.Log($"State {type} not found");

            return null;
        }

        public T SwitchState<T>() where T : CustomerStateBase
        {
            currentState?.OnStateExit();
            T state = GetState<T>();
            state.OnStateEnter();
            currentState = state;
            return state;
        }

        public void SwitchState(Type type)
        {
            currentState?.OnStateExit();
            var state = GetState(type);
            state.OnStateEnter();
            currentState = state;
        }

        private CustomerStateBase currentState;

        public CustomerStateHandler()
        {
            currentState = null;
        }

        public void UpdateState()
        {
            currentState?.OnStateUpdate();

            if (currentState.transitions == null)
                return;
            foreach (CustomerTransition trans in currentState.transitions)
            {
                if(trans.Condition())
                {
                    SwitchState(trans.To);
                }
            }
        }

#if UNITY_EDITOR
        public CustomerStateBase editorCurrentState => currentState;
#endif
    }
}