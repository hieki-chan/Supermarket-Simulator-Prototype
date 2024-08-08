using System;
using System.Reflection;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace Hieki.AI.State
{
    public class StateMachine<T> : IStateMachine where T : IState
    {
        

        public T currentState 
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get; 
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            protected set; 
        }

        public virtual void Start()
        {

        }

        public virtual void UpdateState()
        {
            currentState?.OnStateUpdate();
        }
    }

    public interface IStateMachine
    {
        void Start();
        void UpdateState();
    }
}
