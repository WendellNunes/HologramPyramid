using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

// =====================================================
// TutorialManager.cs
// Controla o tutorial em páginas
// + Fullscreen on/off
// + Trava horizontal
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

    // =====================================================
    // FULLSCREEN BUTTONS
    // =====================================================
    [Header("Fullscreen Buttons")]
    [SerializeField] private GameObject fullScreenOnButton;
    [SerializeField] private GameObject fullScreenOffButton;

    private int index = 0;

    // =====================================================
    // START
    // =====================================================
    private void Start()
    {
        ForceLandscapeOnly();

        if (pages == null || pages.Length == 0)
        {
            Debug.LogError("TutorialManager: 'pages' está vazio. Adicione os sprites do tutorial no Inspector.");
            return;
        }

        if (backButton != null)
            backButton.interactable = false;

        index = 0;
        UpdateUI();
        UpdateFullscreenButtons();
    }

    // =====================================================
    // UPDATE
    // =====================================================
    private void Update()
    {
        UpdateFullscreenButtons();
    }

    // =====================================================
    // ORIENTATION
    // trava horizontal
    // =====================================================
    private void ForceLandscapeOnly()
    {
        Screen.autorotateToPortrait = false;
        Screen.autorotateToPortraitUpsideDown = false;
        Screen.autorotateToLandscapeLeft = true;
        Screen.autorotateToLandscapeRight = true;
    }

    // =====================================================
    // FULLSCREEN
    // =====================================================
    public void FullScreenOn()
    {
        Screen.fullScreen = true;
        UpdateFullscreenButtons();
    }

    public void FullScreenOff()
    {
        Screen.fullScreen = false;
        UpdateFullscreenButtons();
    }

    private void UpdateFullscreenButtons()
    {
        if (fullScreenOnButton != null)
            fullScreenOnButton.SetActive(!Screen.fullScreen);

        if (fullScreenOffButton != null)
            fullScreenOffButton.SetActive(Screen.fullScreen);
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

        prevButton.interactable = true;

        nextButton.interactable = !isLast;
        SetButtonAlpha(nextButton, isLast ? disabledAlpha : 1f);

        startButton.gameObject.SetActive(isLast);
    }

    // =====================================================
    // BUTTON VISUAL ALPHA
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