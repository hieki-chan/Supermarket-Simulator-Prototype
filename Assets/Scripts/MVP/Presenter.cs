using UnityEngine;

namespace Supermarket.MVP
{
    public abstract class Presenter<M, V>
    {
        [SerializeField] protected M model;
        [SerializeField] protected V view;

        public abstract void Initialize();

        /// <summary>
        /// Useful for cancel UniTask
        /// </summary>
        public virtual void OnDestroy() { }
    }
}