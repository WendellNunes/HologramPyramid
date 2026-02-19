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
    // =====================================================
    [Header("Scenes (Build Settings Index)")]
    [SerializeField] private int titleSceneIndex = 1;
    [SerializeField] private int startSceneIndex = 2;

    // =====================================================
    // TUTORIAL PAGES
    // =====================================================
    [Header("Tutorial Pages")]
    [SerializeField] private Image tutorialImage;
    [SerializeField] private Sprite[] pages;

    // =====================================================
    // BUTTONS
    // =====================================================
    [Header("Buttons")]
    [SerializeField] private Button backButton;  
    [SerializeField] private Button prevButton;
    [SerializeField] private Button nextButton;
    [SerializeField] private Button startButton;
    [SerializeField] private float disabledAlpha = 0.35f;

    private int index = 0;

    private void Start()
    {
        if (pages == null || pages.Length == 0)
        {
            Debug.LogError("TutorialManager: 'pages' está vazio. Adicione os sprites do tutorial no Inspector.");
            return;
        }

        // Deixa o botão Back desabilitado (mas mantém a função no script)
        if (backButton != null)
            backButton.interactable = false;

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
    // - Se estiver na primeira página, volta pro Title
    // =====================================================
    public void PrevPage()
    {
        if (index <= 0)
        {
            SceneManager.LoadScene(titleSceneIndex);
            return;
        }

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
    // =====================================================
    public void StartGame()
    {
        SceneManager.LoadScene(startSceneIndex);
    }

    // =====================================================
    // UPDATE UI
    // =====================================================
    private void UpdateUI()
    {
        tutorialImage.sprite = pages[index];

        bool isLast = (index == pages.Length - 1);

        // PREV agora sempre fica habilitado (porque no 1º sprite ele volta pro Title)
        prevButton.interactable = true;

        // NEXT desativa na última
        nextButton.interactable = !isLast;
        SetButtonAlpha(nextButton, isLast ? disabledAlpha : 1f);

        // START só na última
        startButton.gameObject.SetActive(isLast);
    }

    private void SetButtonAlpha(Button btn, float a)
    {
        var g = btn.targetGraphic;
        if (g == null) return;

        Color c = g.color;
        c.a = a;
        g.color = c;
    }
}