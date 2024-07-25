using UnityEngine;
using UnityEngine.EventSystems;

public class LookZone : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    public bool performed;

    public void OnPointerDown(PointerEventData eventData)
    {
        performed = true;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        performed = false;
    }
}
