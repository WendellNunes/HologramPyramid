using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

// =====================================================
// TutorialManager.cs
// Controla o tutorial em páginas (imagens):
// - Avançar / voltar páginas
// - Botão Start só na última página
// - Voltar ao menu inicial
// =====================================================

public class TutorialManager : MonoBehaviour
{
    // =====================================================
    // SCENES
    // Índices das cenas no Build Settings
    // =====================================================
    [Header("Scenes (Build Settings Index)")]
    [SerializeField] private int titleSceneIndex = 1;  // volta pro menu principal
    [SerializeField] private int startSceneIndex = 2;  // inicia o jogo após tutorial

    // =====================================================
    // TUTORIAL PAGES
    // Lista de imagens (sprites) do tutorial
    // =====================================================
    [Header("Tutorial Pages")]
    [SerializeField] private Image tutorialImage; // UI Image que exibe o tutorial
    [SerializeField] private Sprite[] pages;      // array com todas as páginas

    // =====================================================
    // BUTTONS
    // =====================================================
    [Header("Buttons")]
    [SerializeField] private Button prevButton;   // botão voltar página
    [SerializeField] private Button nextButton;   // botão próxima página
    [SerializeField] private Button startButton;  // botão start (só última página)
    [SerializeField] private float disabledAlpha = 0.35f; // transparência do botão desativado

    // =====================================================
    // CONTROLE INTERNO
    // Página atual do tutorial
    // =====================================================
    private int index = 0;

    // =====================================================
    // START
    // =====================================================
    private void Start()
    {
        // segurança: verifica se existem páginas
        if (pages == null || pages.Length == 0)
        {
            Debug.LogError("TutorialManager: 'pages' está vazio. Adicione os sprites do tutorial no Inspector.");
            return;
        }

        index = 0;
        UpdateUI();
    }

    // =====================================================
    // VOLTAR AO MENU PRINCIPAL
    // =====================================================
    public void BackToTitle()
    {
        SceneManager.LoadScene(titleSceneIndex);
    }

    // =====================================================
    // PÁGINA ANTERIOR
    // =====================================================
    public void PrevPage()
    {
        if (index <= 0) return;
        index--;
        UpdateUI();
    }

    // =====================================================
    // PRÓXIMA PÁGINA
    // =====================================================
    public void NextPage()
    {
        if (index >= pages.Length - 1) return;
        index++;
        UpdateUI();
    }

    // =====================================================
    // START GAME
    // Só aparece na última página
    // =====================================================
    public void StartGame()
    {
        SceneManager.LoadScene(startSceneIndex);
    }

    // =====================================================
    // UPDATE UI
    // Atualiza imagem e botões
    // =====================================================
    private void UpdateUI()
    {
        // atualiza imagem atual
        tutorialImage.sprite = pages[index];

        bool isFirst = (index == 0);
        bool isLast  = (index == pages.Length - 1);

        // -------------------------
        // BOTÃO VOLTAR (tutorial)
        // -------------------------
        prevButton.interactable = !isFirst;

        // -------------------------
        // BOTÃO NEXT
        // Desativa na última página
        // -------------------------
        nextButton.interactable = !isLast;
        SetButtonAlpha(nextButton, isLast ? disabledAlpha : 1f);

        // -------------------------
        // BOTÃO START
        // Só aparece na última
        // -------------------------
        startButton.gameObject.SetActive(isLast);
    }

    // =====================================================
    // SET BUTTON ALPHA
    // Ajusta transparência visual do botão
    // =====================================================
    private void SetButtonAlpha(Button btn, float a)
    {
        var g = btn.targetGraphic;
        if (g == null) return;

        Color c = g.color;
        c.a = a;
        g.color = c;
    }
}