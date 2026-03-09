// =====================================================
// SELECTED MODEL
// Guarda qual modelo foi escolhido no MENU.
// A cena principal lê essa informação para spawnar.
// =====================================================

public static class SelectedModel
{

    // =====================================================
    // TIPOS DE MODELO
    // =====================================================

    public enum Choice
    {
        Lung,
        Bronchus,
        Alveolus,
        Trachea,
        Heart
    }

    // =====================================================
    // MODELO SELECIONADO
    // =====================================================

    public static Choice Selected = Choice.Lung;
}