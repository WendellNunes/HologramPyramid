using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuManager : MonoBehaviour
{
    [Header("Build Settings Index")]
    [SerializeField] private int titleSceneIndex = 1;
    [SerializeField] private int mainSceneIndex = 4;

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
}