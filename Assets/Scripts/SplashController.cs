using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Video;

// =====================================================
// SplashVideoToScene.cs
// Controla vídeo de abertura (splash screen)
// Quando o vídeo termina → carrega próxima cena automaticamente.
// =====================================================

public class SplashVideoToScene : MonoBehaviour
{
    // =====================================================
    // REFERÊNCIAS
    // =====================================================
    [Header("Componentes")]
    [SerializeField] private VideoPlayer videoPlayer; // vídeo da intro
    [SerializeField] private AudioSource audioSource; // áudio do vídeo

    // =====================================================
    // CENA DESTINO
    // =====================================================
    [Header("Next Scene")]
    [SerializeField] private int nextSceneIndex = 1; // índice da próxima cena no Build Settings

    // =====================================================
    // CONTROLE INTERNO
    // Evita carregar a cena duas vezes
    // =====================================================
    private bool loading = false;

    // =====================================================
    // AWAKE
    // Configura o player antes de iniciar
    // =====================================================
    private void Awake()
    {
        // vídeo não inicia automaticamente
        videoPlayer.playOnAwake = false;

        // não fica em loop
        videoPlayer.isLooping = false;

        // espera carregar primeiro frame antes de mostrar
        videoPlayer.waitForFirstFrame = true;

        // -------------------------
        // Configurar áudio do vídeo
        // -------------------------
        videoPlayer.audioOutputMode = VideoAudioOutputMode.AudioSource;
        videoPlayer.EnableAudioTrack(0, true);
        videoPlayer.SetTargetAudioSource(0, audioSource);

        // -------------------------
        // Evento quando vídeo termina
        // -------------------------
        videoPlayer.loopPointReached += OnVideoFinished;
    }

    // =====================================================
    // START
    // Prepara e inicia vídeo
    // =====================================================
    private void Start()
    {
        // prepara o vídeo antes de tocar
        videoPlayer.Prepare();

        // quando terminar de preparar:
        videoPlayer.prepareCompleted += (_) =>
        {
            audioSource.Play();   // toca áudio
            videoPlayer.Play();   // toca vídeo
        };
    }

    // =====================================================
    // QUANDO O VÍDEO TERMINA
    // =====================================================
    private void OnVideoFinished(VideoPlayer vp)
    {
        // evita duplicar load
        if (loading) return;
        loading = true;

        // carrega próxima cena
        SceneManager.LoadScene(nextSceneIndex);
    }
}