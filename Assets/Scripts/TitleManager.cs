using UnityEngine;
using UnityEngine.SceneManagement;

public class TitleMenu : MonoBehaviour
{
    [Header("Scenes")]
    [SerializeField] private int gameSceneIndex = 2;
    [SerializeField] private int tutorialSceneIndex = 3;

    [Header("Social Links")]
    [SerializeField] private string instagramURL;
    [SerializeField] private string tiktokURL;
    [SerializeField] private string facebookURL;
    [SerializeField] private string kwaiURL;

    public void StartGame()
    {
        SceneManager.LoadScene(gameSceneIndex);
    }

    public void OpenTutorial()
    {
        SceneManager.LoadScene(tutorialSceneIndex);
    }

    public void OpenInstagram()
    {
        Application.OpenURL(instagramURL);
    }

    public void OpenTikTok()
    {
        Application.OpenURL(tiktokURL);
    }

    public void OpenFacebook()
    {
        Application.OpenURL(facebookURL);
    }

    public void OpenKwai()
    {
        Application.OpenURL(kwaiURL);
    }
}