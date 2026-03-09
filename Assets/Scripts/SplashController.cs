using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Video;

// =====================================================
// SPLASH CONTROLLER
// Controla o vídeo de abertura.
// Quando o vídeo termina → carrega a próxima cena.
// =====================================================

public class SplashVideoToScene : MonoBehaviour
{
    // =====================================================
    // REFERÊNCIAS DO VÍDEO
    // =====================================================

    [Header("Componentes")]
    [SerializeField] private VideoPlayer videoPlayer;
    [SerializeField] private AudioSource audioSource;

    // =====================================================
    // CENA DESTINO
    // =====================================================

    [Header("Next Scene")]
    [SerializeField] private int nextSceneIndex = 1;

    // =====================================================
    // CONTROLE INTERNO
    // Evita carregar duas vezes a mesma cena
    // =====================================================

    private bool loading = false;

    // =====================================================
    // CONFIGURAÇÃO INICIAL
    // =====================================================

    private void Awake()
    {
        videoPlayer.playOnAwake = false;
        videoPlayer.isLooping = false;
        videoPlayer.waitForFirstFrame = true;

        // configurar áudio
        videoPlayer.audioOutputMode = VideoAudioOutputMode.AudioSource;
        videoPlayer.EnableAudioTrack(0, true);
        videoPlayer.SetTargetAudioSource(0, audioSource);
    }

    // =====================================================
    // INICIAR VÍDEO
    // =====================================================

    private void Start()
    {
        videoPlayer.loopPointReached += OnVideoFinished;
        videoPlayer.Play();
    }

    // =====================================================
    // FINAL DO VÍDEO
    // =====================================================

    private void OnVideoFinished(VideoPlayer vp)
    {
        if (loading) return;

        loading = true;
        SceneManager.LoadScene(nextSceneIndex);
    }
}