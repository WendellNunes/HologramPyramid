using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Video;

public class SplashVideoToScene : MonoBehaviour
{
    [SerializeField] private VideoPlayer videoPlayer;
    [SerializeField] private AudioSource audioSource;

    [SerializeField] private int nextSceneIndex = 1; // Scene 1 no Build Settings

    private bool loading = false;

    private void Awake()
    {
        videoPlayer.playOnAwake = false;
        videoPlayer.isLooping = false;
        videoPlayer.waitForFirstFrame = true;

        videoPlayer.audioOutputMode = VideoAudioOutputMode.AudioSource;
        videoPlayer.EnableAudioTrack(0, true);
        videoPlayer.SetTargetAudioSource(0, audioSource);

        videoPlayer.loopPointReached += OnVideoFinished;
    }

    private void Start()
    {
        videoPlayer.Prepare();
        videoPlayer.prepareCompleted += (_) =>
        {
            audioSource.Play();
            videoPlayer.Play();
        };
    }

    private void OnVideoFinished(VideoPlayer vp)
    {
        if (loading) return;
        loading = true;
        SceneManager.LoadScene(nextSceneIndex);
    }
}