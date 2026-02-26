using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

// =====================================================
// HoldButton.cs (CORRIGIDO para MOBILE)
// - Hold (PointerDown / PointerUp)  
// - Double click no PC (clickCount) 
// - Double tap no MOBILE por tempo 
// =====================================================

public class HoldButton : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IPointerClickHandler
{
    [Header("Hold")]
    public UnityEvent onHoldStart; // quando começa a segurar
    public UnityEvent onHoldEnd;   // quando solta

    [Header("Double Click")]
    public UnityEvent onDoubleClick;

    [Header("Mobile Double Tap")]
    [Tooltip("Tempo máximo entre dois taps para contar como duplo toque (mobile)")]
    public float maxDelay = 0.30f;

    private float lastTapTime = -10f;
    private int tapCount = 0;

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
        // 1) PC / Mouse: se o Unity reportar clickCount >= 2, usa isso
        if (eventData != null && eventData.clickCount >= 2)
        {
            onDoubleClick?.Invoke();
            ResetTap();
            return;
        }

        // 2) Mobile / Touch: detecta double tap por tempo
        float now = Time.unscaledTime;

        if (now - lastTapTime <= maxDelay)
            tapCount++;
        else
            tapCount = 1;

        lastTapTime = now;

        if (tapCount >= 2)
        {
            onDoubleClick?.Invoke();
            ResetTap();
        }
    }

    private void ResetTap()
    {
        tapCount = 0;
        lastTapTime = -10f;
    }
}