using UnityEngine;
using UnityEngine.SceneManagement;

// =====================================================
// MenuManager.cs
// Controla a cena de MENU (seleção de modelos + navegação).
// =====================================================

public class MenuManager : MonoBehaviour
{
    // =====================================================
    // Scenes (Build Settings Index)
    // =====================================================
    [Header("Build Settings Index")]
    [SerializeField] private int titleSceneIndex = 1;
    [SerializeField] private int mainSceneIndex = 4;

    // =====================================================
    // Beacon (Open URL)
    // Coloque aqui o link do seu “Beacon” (lista de endereços).
    // =====================================================
    [Header("Beacon URL")]
    [SerializeField] private string beaconUrl = "https://SEU_LINK_AQUI";

    // =====================================================
    // Navigation
    // =====================================================
    public void BackToTitle()
    {
        SceneManager.LoadScene(titleSceneIndex);
    }

    // =====================================================
    // Model Selection
    // =====================================================
    public void SelectLung()
    {
        SelectedModel.Selected = SelectedModel.Choice.Lung;
        SceneManager.LoadScene(mainSceneIndex);
    }

    public void SelectBronchus()
    {
        SelectedModel.Selected = SelectedModel.Choice.Bronchus;
        SceneManager.LoadScene(mainSceneIndex);
    }

    public void SelectAlveolus()
    {
        SelectedModel.Selected = SelectedModel.Choice.Alveolus;
        SceneManager.LoadScene(mainSceneIndex);
    }

    public void SelectTrachea()
    {
        SelectedModel.Selected = SelectedModel.Choice.Trachea;
        SceneManager.LoadScene(mainSceneIndex);
    }

    public void SelectHeart()
    {
        SelectedModel.Selected = SelectedModel.Choice.Heart;
        SceneManager.LoadScene(mainSceneIndex);
    }

    // =====================================================
    // Beacon Button
    // =====================================================
    public void OpenBeacon()
    {
        if (string.IsNullOrWhiteSpace(beaconUrl))
        {
            Debug.LogError("MenuManager: beaconUrl está vazio. Preencha no Inspector.");
            return;
        }

        Application.OpenURL(beaconUrl);
    }
}