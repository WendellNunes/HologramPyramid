using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

// =====================================================
// HOLD BUTTON
// Detecta interações no botão:
//
// • Segurar (PointerDown / PointerUp)
// • Double Click no PC
// • Double Tap no celular
// =====================================================

public class HoldButton : MonoBehaviour,
    IPointerDownHandler,
    IPointerUpHandler,
    IPointerClickHandler
{

    // =====================================================
    // EVENTOS
    // =====================================================

    [Header("Hold")]
    public UnityEvent onHoldStart;
    public UnityEvent onHoldEnd;

    [Header("Double Click")]
    public UnityEvent onDoubleClick;

    // =====================================================
    // DOUBLE TAP MOBILE
    // =====================================================

    [Header("Mobile Double Tap")]
    public float maxDelay = 0.30f;

    private float lastTapTime = -10f;
    private int tapCount = 0;

    // =====================================================
    // SEGURAR BOTÃO
    // =====================================================

    public void OnPointerDown(PointerEventData eventData)
    {
        onHoldStart?.Invoke();
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        onHoldEnd?.Invoke();
    }

    // =====================================================
    // CLICK / DOUBLE CLICK
    // =====================================================

    public void OnPointerClick(PointerEventData eventData)
    {
        // PC double click
        if (eventData != null && eventData.clickCount >= 2)
        {
            onDoubleClick?.Invoke();
            ResetTap();
            return;
        }

        // mobile double tap
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

    // =====================================================
    // RESET DOUBLE TAP
    // =====================================================

    private void ResetTap()
    {
        tapCount = 0;
        lastTapTime = -10f;
    }
}