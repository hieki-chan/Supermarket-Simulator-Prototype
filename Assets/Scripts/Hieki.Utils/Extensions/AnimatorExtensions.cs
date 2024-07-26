using UnityEngine;

namespace Hieki.Utils
{
    public static class AnimatorExtensions
    {
        /// <summary>
        /// Play animation dynamically if there is no transition on the given layer.
        /// </summary>
        public static void DynamicPlay(this Animator animator, int stateHashName, float normalizedTransitionDuration, int layer = 0)
        {
            if (!animator.IsInTransition(0) && animator.gameObject.activeSelf)
                animator.CrossFade(stateHashName, normalizedTransitionDuration, layer);
        }

        /// <summary>
        /// Play animation immediately on the given layer.
        /// </summary>
        public static void ImmediatePlay(this Animator animator, int stateHashName, float normalizedTransitionDuration, int layer = 0)
        {
            if (animator.gameObject.activeSelf && !IsPlaying(animator, stateHashName, layer))
                animator.CrossFade(stateHashName, normalizedTransitionDuration, layer);
        }

        /// <summary>
        /// Returns true if the current animation with given hash name is playing.
        /// </summary>
        public static bool IsPlaying(this Animator animator, int stateHashName, int layer = 0)
        {
            return !animator.IsInTransition(0) && animator.GetCurrentAnimatorStateInfo(layer).shortNameHash == stateHashName && animator.gameObject.activeSelf;
        }
    }
}
