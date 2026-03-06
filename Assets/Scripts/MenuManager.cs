using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuManager : MonoBehaviour
{
    [Header("Scenes")]
    [SerializeField] private int titleSceneIndex = 1;
    [SerializeField] private int mainSceneIndex = 4;

    [Header("Beacon")]
    [SerializeField] private string beaconUrl = "https://SEU_LINK_AQUI";

    [Header("Fullscreen Buttons")]
    [SerializeField] private GameObject fullScreenOnButton;
    [SerializeField] private GameObject fullScreenOffButton;

    private bool isFullscreen = false;

    void Start()
    {
        // trava em horizontal
        Screen.autorotateToPortrait = false;
        Screen.autorotateToPortraitUpsideDown = false;
        Screen.autorotateToLandscapeLeft = true;
        Screen.autorotateToLandscapeRight = true;

        // estado inicial
        isFullscreen = false;
        Screen.fullScreen = false;
        UpdateFullscreenButtons();
    }

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

    public void OpenBeacon()
    {
        if (string.IsNullOrWhiteSpace(beaconUrl))
        {
            Debug.LogWarning("Beacon não configurado.");
            return;
        }

        Application.OpenURL(beaconUrl);
    }

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

    private void UpdateFullscreenButtons()
    {
        if (fullScreenOnButton != null)
            fullScreenOnButton.SetActive(isFullscreen);

        if (fullScreenOffButton != null)
            fullScreenOffButton.SetActive(!isFullscreen);
    }
}