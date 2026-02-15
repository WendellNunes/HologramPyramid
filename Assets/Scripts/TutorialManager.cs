using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class TutorialManager : MonoBehaviour
{
    [Header("Scenes (Build Settings Index)")]
    [SerializeField] private int titleSceneIndex = 1;
    [SerializeField] private int startSceneIndex = 2;

    [Header("Tutorial Pages")]
    [SerializeField] private Image tutorialImage;     // UI Image que mostra o sprite
    [SerializeField] private Sprite[] pages;          // lista de imagens do tutorial (sprites)

    [Header("Buttons")]
    [SerializeField] private Button prevButton;       // Back (tutorial)
    [SerializeField] private Button nextButton;       // Next
    [SerializeField] private Button startButton;      // Start (só na última)
    [SerializeField] private float disabledAlpha = 0.35f; // transparência do Next no final

    private int index = 0;

    private void Start()
    {
        // Segurança
        if (pages == null || pages.Length == 0)
        {
            Debug.LogError("TutorialManager: 'pages' está vazio. Adicione os sprites do tutorial no Inspector.");
            return;
        }

        index = 0;
        UpdateUI();
    }

    public void BackToTitle()
    {
        SceneManager.LoadScene(titleSceneIndex);
    }

    public void PrevPage()
    {
        if (index <= 0) return;
        index--;
        UpdateUI();
    }

    public void NextPage()
    {
        if (index >= pages.Length - 1) return;
        index++;
        UpdateUI();
    }

    public void StartGame()
    {
        SceneManager.LoadScene(startSceneIndex);
    }

    private void UpdateUI()
    {
        // Atualiza imagem
        tutorialImage.sprite = pages[index];

        bool isFirst = (index == 0);
        bool isLast  = (index == pages.Length - 1);

        // Back (tutorial) desativado na primeira
        prevButton.interactable = !isFirst;

        // Next desativado na última + transparente
        nextButton.interactable = !isLast;
        SetButtonAlpha(nextButton, isLast ? disabledAlpha : 1f);

        // Start só aparece na última
        startButton.gameObject.SetActive(isLast);
    }

    private void SetButtonAlpha(Button btn, float a)
    {
        // altera o alpha do Target Graphic (Image do botão)
        var g = btn.targetGraphic;
        if (g == null) return;

        Color c = g.color;
        c.a = a;
        g.color = c;
    }
}