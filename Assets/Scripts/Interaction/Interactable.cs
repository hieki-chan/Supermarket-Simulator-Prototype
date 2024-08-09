using QuickOutline;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace Supermarket
{
    [RequireComponent(typeof(Outline))]
    public abstract class Interactable : MonoBehaviour
    {
        public InteractionInfo interactionInfo => m_interactionInfo;
        [SerializeField] private InteractionInfo m_interactionInfo;

        //public UnityAction<Interactable> OnNoInteraction;

        protected Outline outline;

        protected virtual void Awake()
        {
            outline = GetComponent<Outline>();
            outline.enabled = false;
        }

        #region Hovering

        public virtual void OnHoverEnter(Transform playerTrans)
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

        public virtual void OnInteract(Transform playerTrans, Transform cameraTrans)
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
        #endregion
    }
}
