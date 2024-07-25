﻿using Supermarket.Player;
using UnityEngine;
using UnityEngine.Events;

namespace Supermarket
{
    [RequireComponent(typeof(Outline))]
    public abstract class Interactable : MonoBehaviour
    {
        [SerializeField] private InteractionInfo interaction;

        public UnityAction<Interactable> OnNoInteraction;

        protected Outline outline;

        protected virtual void Awake()
        {
            outline = GetComponent<Outline>();
            outline.enabled = false;
        }

        #region Hovering

        public virtual void OnHoverEnter()
        {
            outline.enabled = true;
        }

        public virtual void OnHover()
        {

        }

        public virtual void OnHoverExit()
        {
            outline.enabled = false;
        }

        public virtual void OnHoverOtherEnter()
        {

        }

        public virtual void OnHoverOther(Interactable other)
        {

        }

        public virtual void OnHoverOther(Collider collider)
        {

        }

        public virtual void OnHoverOtherExit()
        {

        }
        #endregion

        #region Interacting

        public virtual void OnInteractEnter()
        {

        }

        public virtual void OnInteract(PlayerController targetPlayer)
        {

        }

        public virtual void OnInteractExit()
        {

        }

        public virtual void OnInteractOtherEnter()
        {

        }

        public virtual void OnInteractOther(Interactable other)
        {

        }

        public virtual void OnInteractOtherExit()
        {

        }

        protected virtual void InteractExit()
        {
            OnNoInteraction?.Invoke(this);
        }
        #endregion
    }
}
