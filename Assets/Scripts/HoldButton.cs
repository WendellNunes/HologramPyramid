using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

// =====================================================
// HoldButton.cs
// Script para botões da UI que detectam:
// - Segurar pressionado (hold)
// - Soltar (end hold)
// - Duplo clique
//
// Usado para:
// rotação contínua, zoom contínuo e auto-rotação.
// =====================================================

public class HoldButton : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IPointerClickHandler
{
    // =====================================================
    // HOLD EVENTS
    // Chamados quando segura e quando solta o botão
    // =====================================================
    [Header("Hold")]
    public UnityEvent onHoldStart; // quando começa a segurar
    public UnityEvent onHoldEnd;   // quando solta

    // =====================================================
    // DOUBLE CLICK EVENT
    // Chamado quando dá duplo clique no botão
    // =====================================================
    [Header("Double Click")]
    public UnityEvent onDoubleClick;

    // =====================================================
    // Pointer Down
    // Quando o dedo/mouse pressiona o botão
    // =====================================================
    public void OnPointerDown(PointerEventData eventData)
    {
        // dispara evento de segurar
        onHoldStart?.Invoke();
    }

    // =====================================================
    // Pointer Up
    // Quando solta o botão
    // =====================================================
    public void OnPointerUp(PointerEventData eventData)
    {
        // dispara evento de parar de segurar
        onHoldEnd?.Invoke();
    }

    // =====================================================
    // Pointer Click
    // Detecta clique e duplo clique
    // =====================================================
    public void OnPointerClick(PointerEventData eventData)
    {
        // Unity já conta número de cliques automaticamente
        // Se >= 2 → duplo clique
        if (eventData.clickCount >= 2)
            onDoubleClick?.Invoke();
    }
}