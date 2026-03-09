using UnityEngine;
using UnityEngine.SceneManagement;

public class TitleManager : MonoBehaviour
{
    // =====================================================
    // SCENES
    // =====================================================
    [Header("Scenes")]
    [SerializeField] private int gameSceneIndex = 2;
    [SerializeField] private int tutorialSceneIndex = 3;

    // =====================================================
    // LINKS
    // =====================================================
    [Header("Links")]
    [SerializeField] private string beaconURL;

    // =====================================================
    // FULLSCREEN BUTTONS
    // fullScreenOnButton  = botão para ENTRAR em fullscreen
    // fullScreenOffButton = botão para SAIR do fullscreen
    // =====================================================
    [Header("Fullscreen Buttons")]
    [SerializeField] private GameObject fullScreenOnButton;
    [SerializeField] private GameObject fullScreenOffButton;

    // =====================================================
    // AWAKE
    // trava horizontal
    // =====================================================
    private void Awake()
    {
        ForceLandscapeOnly();
    }

    // =====================================================
    // START
    // =====================================================
    private void Start()
    {
        UpdateFullscreenButtons();
    }

    // =====================================================
    // UPDATE
    // mantém sincronizado caso o navegador altere o estado
    // =====================================================
    private void Update()
    {
        UpdateFullscreenButtons();
    }

    // =====================================================
    // ORIENTATION
    // =====================================================
    private void ForceLandscapeOnly()
    {
        Screen.autorotateToPortrait = false;
        Screen.autorotateToPortraitUpsideDown = false;
        Screen.autorotateToLandscapeLeft = true;
        Screen.autorotateToLandscapeRight = true;
    }

    // =====================================================
    // NAVIGATION
    // =====================================================
    public void StartGame()
    {
        SceneManager.LoadScene(gameSceneIndex);
    }

    public void OpenTutorial()
    {
        SceneManager.LoadScene(tutorialSceneIndex);
    }

    // =====================================================
    // LINK
    // =====================================================
    public void OpenBeacon()
    {
        if (!string.IsNullOrWhiteSpace(beaconURL))
            Application.OpenURL(beaconURL);
        else
            Debug.LogWarning("Beacon URL não configurada.");
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

    // =====================================================
    // UPDATE FULLSCREEN BUTTONS
    // =====================================================
    private void UpdateFullscreenButtons()
    {
        if (fullScreenOnButton != null)
            fullScreenOnButton.SetActive(!Screen.fullScreen);

        if (fullScreenOffButton != null)
            fullScreenOffButton.SetActive(Screen.fullScreen);
    }
}