using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class TitleMenu : MonoBehaviour
{
    [Header("Scenes")]
    [SerializeField] private int gameSceneIndex = 2;
    [SerializeField] private int tutorialSceneIndex = 3;

    [Header("Links")]
    [SerializeField] private string beaconURL;

    [Header("Fullscreen Buttons")]
    [SerializeField] private GameObject fullScreenOnButton;
    [SerializeField] private GameObject fullScreenOffButton;

    void Awake()
    {
        // força horizontal
        Screen.autorotateToPortrait = false;
        Screen.autorotateToPortraitUpsideDown = false;
        Screen.autorotateToLandscapeLeft = true;
        Screen.autorotateToLandscapeRight = true;

        // estado inicial
        Screen.fullScreen = false;
        UpdateFullscreenButtons();
    }

    public void StartGame()
    {
        SceneManager.LoadScene(gameSceneIndex);
    }

    public void OpenTutorial()
    {
        SceneManager.LoadScene(tutorialSceneIndex);
    }

    public void OpenBeacon()
    {
        if (!string.IsNullOrWhiteSpace(beaconURL))
            Application.OpenURL(beaconURL);
        else
            Debug.LogWarning("Beacon URL não configurada.");
    }

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

    void UpdateFullscreenButtons()
    {
        fullScreenOnButton.SetActive(Screen.fullScreen);
        fullScreenOffButton.SetActive(!Screen.fullScreen);
    }
}