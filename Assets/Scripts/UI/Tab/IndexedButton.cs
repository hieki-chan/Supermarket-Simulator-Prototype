using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.Events;

public class IndexedButton : Selectable, IPointerClickHandler
{
    public SelectEvent OnSelected { get { return m_OnSelected; } set { m_OnSelected = value; } }
    [SerializeField] private SelectEvent m_OnSelected;
    [System.Serializable]
    public class SelectEvent : UnityEvent<int> { }

    [NonEditable] public int index;

    public virtual void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Right) { return; }
        m_OnSelected?.Invoke(index);
    }
}
