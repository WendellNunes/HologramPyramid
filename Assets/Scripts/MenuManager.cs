using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuManager : MonoBehaviour
{
    // =====================================================
    // SCENES
    // =====================================================
    [Header("Scenes")]
    [SerializeField] private int titleSceneIndex = 1;
    [SerializeField] private int mainSceneIndex = 4;

    // =====================================================
    // BEACON
    // =====================================================
    [Header("Beacon")]
    [SerializeField] private string beaconUrl = "https://SEU_LINK_AQUI";

    // =====================================================
    // FULLSCREEN BUTTONS
    // fullScreenOnButton  = botão para ENTRAR em fullscreen
    // fullScreenOffButton = botão para SAIR do fullscreen
    // =====================================================
    [Header("Fullscreen Buttons")]
    [SerializeField] private GameObject fullScreenOnButton;
    [SerializeField] private GameObject fullScreenOffButton;

    private bool isFullscreen = false;

    // =====================================================
    // START
    // =====================================================
    private void Start()
    {
        ForceLandscapeOnly();

        isFullscreen = Screen.fullScreen;
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
    // NAVIGATION
    // =====================================================
    public void BackToTitle()
    {
        SceneManager.LoadScene(titleSceneIndex);
    }

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
    // BEACON LINK
    // =====================================================
    public void OpenBeacon()
    {
        if (string.IsNullOrWhiteSpace(beaconUrl))
        {
            Debug.LogWarning("Beacon não configurado.");
            return;
        }

        Application.OpenURL(beaconUrl);
    }

    // =====================================================
    // FULLSCREEN
    // =====================================================
    public void FullScreenOn()
    {
        isFullscreen = true;
        Screen.fullScreen = true;
        UpdateFullscreenButtons();
    }

    public void FullScreenOff()
    {
        isFullscreen = false;
        Screen.fullScreen = false;
        UpdateFullscreenButtons();
    }

    // =====================================================
    // UPDATE FULLSCREEN BUTTONS
    // Mostra o botão da ação disponível
    // =====================================================
    private void UpdateFullscreenButtons()
    {
        if (fullScreenOnButton != null)
            fullScreenOnButton.SetActive(!isFullscreen);

        if (fullScreenOffButton != null)
            fullScreenOffButton.SetActive(isFullscreen);
    }
}