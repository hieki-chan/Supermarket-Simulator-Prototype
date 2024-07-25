using UnityEngine;

namespace Supermarket
{
    [CreateAssetMenu(fileName = "Interaction Info", menuName = "Interactions/Info")]
    public class InteractionInfo : ScriptableObject
    {
        public string Tooltips => tooltips;
        [TextArea]
        [SerializeField] private string tooltips;
    }
}
