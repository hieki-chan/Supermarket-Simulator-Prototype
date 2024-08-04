using System.Runtime.CompilerServices;
using UnityEngine;

namespace Supermarket
{
    [CreateAssetMenu(fileName = "Interaction Info", menuName = "Interactions/Info")]
    public class InteractionInfo : ScriptableObject
    {
        public string Tooltips
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => tooltips;
        }
        [TextArea]
        [SerializeField] private string tooltips;
    }
}
