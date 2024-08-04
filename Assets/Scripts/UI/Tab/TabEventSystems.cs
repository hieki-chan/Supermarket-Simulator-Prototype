using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class TabEventSystems : MonoBehaviour, IPointerClickHandler
{
    [System.Serializable]
    public class TabEvents : UnityEvent { }
    [SerializeField]
    private TabEvents m_OnOpenTab = new TabEvents();
    [SerializeField]
    private TabEvents m_OnCloseTab = new TabEvents();
    [SerializeField]
    private TabEvents m_OnClickTab = new TabEvents();

    public TabEvents OnOpen { get { return m_OnOpenTab; } set { m_OnOpenTab = value; } }
    public TabEvents OnClose { get { return m_OnCloseTab; } set { m_OnCloseTab = value; } }
    public TabEvents OnClick { get { return m_OnClickTab; } set { m_OnClickTab = value; } }

    protected virtual void OnEnable()
    {
        OnOpen?.Invoke();
    }
    protected virtual void OnDisable()
    {
        OnClose?.Invoke();
    }

    public virtual void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Right) { return; }
        OnClick?.Invoke();
    }
}
