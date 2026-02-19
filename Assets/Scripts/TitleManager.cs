using UnityEngine;
using UnityEngine.SceneManagement;

// =====================================================
// TitleMenu.cs
// Controla o menu principal (Title Screen):
// - Iniciar jogo
// - Abrir tutorial
// - Abrir redes sociais
// =====================================================

public class TitleMenu : MonoBehaviour
{
    // =====================================================
    // SCENES
    // Índices configurados no Build Settings
    // =====================================================
    [Header("Scenes")]
    [SerializeField] private int gameSceneIndex = 2;       // Cena principal do jogo
    [SerializeField] private int tutorialSceneIndex = 3;   // Cena tutorial

    // =====================================================
    // SOCIAL LINKS
    // Links configurados no Inspector
    // =====================================================
    [Header("Social Links")]
    [SerializeField] private string instagramURL;
    [SerializeField] private string tiktokURL;
    [SerializeField] private string facebookURL;
    [SerializeField] private string kwaiURL;

    // =====================================================
    // START GAME
    // Carrega cena principal
    // =====================================================
    public void StartGame()
    {
        SceneManager.LoadScene(gameSceneIndex);
    }

    // =====================================================
    // OPEN TUTORIAL
    // Carrega cena tutorial
    // =====================================================
    public void OpenTutorial()
    {
        SceneManager.LoadScene(tutorialSceneIndex);
    }

    // =====================================================
    // SOCIAL BUTTONS
    // Abrem links externos no navegador
    // =====================================================
    public void OpenInstagram()
    {
        OpenURL(instagramURL);
    }

    public void OpenTikTok()
    {
        OpenURL(tiktokURL);
    }

    public void OpenFacebook()
    {
        OpenURL(facebookURL);
    }

    public void OpenKwai()
    {
        OpenURL(kwaiURL);
    }

    // =====================================================
    // MÉTODO AUXILIAR
    // Verifica se o link está preenchido antes de abrir
    // =====================================================
    private void OpenURL(string url)
    {
        if (string.IsNullOrWhiteSpace(url))
        {
            Debug.LogWarning("TitleMenu: URL não configurada no Inspector.");
            return;
        }

        Application.OpenURL(url);
    }
}