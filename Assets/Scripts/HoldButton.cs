using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class HoldButton : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IPointerClickHandler
{
    [Header("Hold")]
    public UnityEvent onHoldStart;
    public UnityEvent onHoldEnd;

    [Header("Double Click")]
    public UnityEvent onDoubleClick;

    public void OnPointerDown(PointerEventData eventData)
    {
        onHoldStart?.Invoke();
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        onHoldEnd?.Invoke();
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        // Unity já conta cliques pra UI
        if (eventData.clickCount >= 2)
            onDoubleClick?.Invoke();
    }
}