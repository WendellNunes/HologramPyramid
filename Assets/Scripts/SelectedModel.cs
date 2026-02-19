// =====================================================
// SelectedModel.cs
// Guarda qual modelo foi escolhido no MENU, para a cena principal.
// =====================================================

public static class SelectedModel
{
    // =====================================================
    // Choice
    // (Adicionamos Trachea e Heart)
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
    // Selected (default)
    // =====================================================
    public static Choice Selected = Choice.Lung;
}