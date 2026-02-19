using UnityEngine;
using UnityEngine.SceneManagement;

// =====================================================
// MainGameManager.cs
// Controla a cena principal: spawn do modelo, rotação, zoom, play/pause, fullscreen.
// =====================================================

public class MainGameManager : MonoBehaviour
{
    // =====================================================
    // Scenes (Build Settings Index)
    // =====================================================
    [Header("Scenes (Build Settings Index)")]
    [SerializeField] private int menuSceneIndex = 2; // índice do MENU

    // =====================================================
    // Spawn
    // (Adicionamos Trachea e Heart)
    // =====================================================
    [Header("Spawn")]
    [SerializeField] private Transform spawnPoint;

    [SerializeField] private GameObject lungPrefab;
    [SerializeField] private GameObject bronchusPrefab;
    [SerializeField] private GameObject alveolusPrefab;
    [SerializeField] private GameObject tracheaPrefab;
    [SerializeField] private GameObject heartPrefab;

    // =====================================================
    // UI Panels
    // =====================================================
    [Header("UI Panels")]
    [SerializeField] private GameObject optionsPanel;

    // =====================================================
    // Toggle Buttons
    // =====================================================
    [Header("Toggle Buttons (mesmo lugar)")]
    [SerializeField] private GameObject menuOpenButtonObj;
    [SerializeField] private GameObject menuCloseButtonObj;

    [SerializeField] private GameObject playButtonObj;
    [SerializeField] private GameObject pauseButtonObj;

    [SerializeField] private GameObject fullscreenOnButtonObj;
    [SerializeField] private GameObject fullscreenOffButtonObj;

    // =====================================================
    // Lighting (3 modos no mesmo lugar)
    // Default: só Superior + Inferior
    // All: Superior + Inferior + luzes perto das câmeras (N/S/L/O)
    // Off: tudo desligado
    // =====================================================
    [Header("Lighting")]
    [SerializeField] private Light topLight;
    [SerializeField] private Light bottomLight;
    [Tooltip("Luzes próximas às câmeras (Norte, Sul, Leste, Oeste)")]
    [SerializeField] private Light[] cameraLights;

    [Header("Lighting Buttons (mesmo lugar)")]
    [SerializeField] private GameObject lightsOffButtonObj;      // botão que desliga tudo
    [SerializeField] private GameObject lightsDefaultButtonObj;  // botão que volta pro padrão (2)
    [SerializeField] private GameObject lightsAllButtonObj;      // botão que liga todas (6)

    // =====================================================
    // Rotation
    // =====================================================
    [Header("Rotation")]
    [SerializeField] private float rotationSpeedDegPerSec = 80f;

    // =====================================================
    // Zoom
    // =====================================================
    [Header("Zoom")]
    [SerializeField] private float zoomSpeedPerSec = 0.6f;
    [SerializeField] private float minScale = 0.2f;
    [SerializeField] private float maxScale = 3.0f;

    // =====================================================
    // Beacon (opcional também na cena principal)
    // =====================================================
    [Header("Beacon URL (optional)")]
    [SerializeField] private string beaconUrl = "https://SEU_LINK_AQUI";

    // =====================================================
    // Runtime refs
    // =====================================================
    private GameObject currentModel;
    private Animator currentAnimator;

    // =====================================================
    // Input state
    // =====================================================
    private enum Dir { None, Left, Right }
    private enum LightMode { Off, Default, All }

    private Dir holdRotate = Dir.None;
    private Dir autoRotate = Dir.None;

    private Dir holdZoom = Dir.None; // Left = ZoomOut, Right = ZoomIn

    private LightMode currentLightMode = LightMode.Default;

    // =====================================================
    // Unity lifecycle
    // =====================================================
    private void Start()
    {
        SpawnSelectedPrefab();
        SetOptionsPanel(false);
        SetMenuToggle(isOpen: false);

        SetPlayPause(isPlaying: true);
        SetFullscreenToggle(isFullscreen: Screen.fullScreen);

        // Luzes: por padrão, Superior + Inferior
        SetLightMode(LightMode.Default);
    }

    private void Update()
    {
        if (currentModel == null) return;

        // ---------------------
        // Rotation (hold > auto)
        // ---------------------
        Dir rot = (holdRotate != Dir.None) ? holdRotate : autoRotate;

        if (rot == Dir.Right)
            currentModel.transform.Rotate(0f, rotationSpeedDegPerSec * Time.deltaTime, 0f, Space.World);
        else if (rot == Dir.Left)
            currentModel.transform.Rotate(0f, -rotationSpeedDegPerSec * Time.deltaTime, 0f, Space.World);

        // ---------------------
        // Zoom (hold)
        // ---------------------
        if (holdZoom == Dir.Right) // Zoom In
            ApplyScaleDelta(+zoomSpeedPerSec * Time.deltaTime);
        else if (holdZoom == Dir.Left) // Zoom Out
            ApplyScaleDelta(-zoomSpeedPerSec * Time.deltaTime);
    }

    // =====================================================
    // Spawn
    // =====================================================
    private void SpawnSelectedPrefab()
    {
        if (spawnPoint == null)
        {
            Debug.LogError("MainGameManager: spawnPoint não foi setado.");
            return;
        }

        GameObject prefab = lungPrefab;

        switch (SelectedModel.Selected)
        {
            case SelectedModel.Choice.Lung:     prefab = lungPrefab; break;
            case SelectedModel.Choice.Bronchus: prefab = bronchusPrefab; break;
            case SelectedModel.Choice.Alveolus: prefab = alveolusPrefab; break;
            case SelectedModel.Choice.Trachea:  prefab = tracheaPrefab; break;
            case SelectedModel.Choice.Heart:    prefab = heartPrefab; break;
        }

        if (prefab == null)
        {
            Debug.LogError("MainGameManager: prefab selecionado está NULL no Inspector.");
            return;
        }

        currentModel = Instantiate(prefab, spawnPoint.position, spawnPoint.rotation);
        currentModel.transform.localScale = Vector3.one;

        currentAnimator = currentModel.GetComponentInChildren<Animator>();
        if (currentAnimator != null) currentAnimator.speed = 1f;
    }

    // =====================================================
    // Scene navigation
    // =====================================================
    public void BackToMenu()
    {
        SceneManager.LoadScene(menuSceneIndex);
    }

    // =====================================================
    // Options menu open/close
    // =====================================================
    public void OpenOptionsMenu()
    {
        SetOptionsPanel(true);
        SetMenuToggle(isOpen: true);
    }

    public void CloseOptionsMenu()
    {
        SetOptionsPanel(false);
        SetMenuToggle(isOpen: false);
    }

    private void SetOptionsPanel(bool open)
    {
        if (optionsPanel != null) optionsPanel.SetActive(open);
    }

    private void SetMenuToggle(bool isOpen)
    {
        if (menuOpenButtonObj) menuOpenButtonObj.SetActive(!isOpen);
        if (menuCloseButtonObj) menuCloseButtonObj.SetActive(isOpen);
    }

    // =====================================================
    // Play / Pause animation toggle
    // =====================================================
    public void PlayAnimation()
    {
        if (currentAnimator != null) currentAnimator.speed = 1f;
        SetPlayPause(isPlaying: true);
    }

    public void PauseAnimation()
    {
        if (currentAnimator != null) currentAnimator.speed = 0f;
        SetPlayPause(isPlaying: false);
    }

    private void SetPlayPause(bool isPlaying)
    {
        if (playButtonObj) playButtonObj.SetActive(!isPlaying);
        if (pauseButtonObj) pauseButtonObj.SetActive(isPlaying);
    }

    // =====================================================
    // Fullscreen toggle (Itch.io / WebGL)
    // =====================================================
    public void FullscreenOn()
    {
        Screen.fullScreen = true;
        SetFullscreenToggle(isFullscreen: true);
    }

    public void FullscreenOff()
    {
        Screen.fullScreen = false;
        SetFullscreenToggle(isFullscreen: false);
    }

    private void SetFullscreenToggle(bool isFullscreen)
    {
        if (fullscreenOnButtonObj) fullscreenOnButtonObj.SetActive(!isFullscreen);
        if (fullscreenOffButtonObj) fullscreenOffButtonObj.SetActive(isFullscreen);
    }

    // =====================================================
    // Rotation controls (Hold + Double click auto)
    // =====================================================
    public void HoldTurnRightStart() { holdRotate = Dir.Right; }
    public void HoldTurnRightEnd()   { if (holdRotate == Dir.Right) holdRotate = Dir.None; }

    public void HoldTurnLeftStart()  { holdRotate = Dir.Left; }
    public void HoldTurnLeftEnd()    { if (holdRotate == Dir.Left) holdRotate = Dir.None; }

    public void ToggleAutoTurnRight()
    {
        autoRotate = (autoRotate == Dir.Right) ? Dir.None : Dir.Right;
    }

    public void ToggleAutoTurnLeft()
    {
        autoRotate = (autoRotate == Dir.Left) ? Dir.None : Dir.Left;
    }

    // =====================================================
    // Zoom controls (Hold)
    // =====================================================
    public void HoldZoomInStart()  { holdZoom = Dir.Right; }
    public void HoldZoomInEnd()    { if (holdZoom == Dir.Right) holdZoom = Dir.None; }

    public void HoldZoomOutStart() { holdZoom = Dir.Left; }
    public void HoldZoomOutEnd()   { if (holdZoom == Dir.Left) holdZoom = Dir.None; }

    private void ApplyScaleDelta(float delta)
    {
        Vector3 s = currentModel.transform.localScale;
        float target = Mathf.Clamp(s.x + delta, minScale, maxScale);
        currentModel.transform.localScale = new Vector3(target, target, target);
    }

    // =====================================================
    // Lighting controls (3 botões no mesmo lugar)
    // Sequência: Default (2) -> All (6) -> Off (0) -> Default...
    // =====================================================
    public void LightsOff()
    {
        SetLightMode(LightMode.Off);
    }

    public void LightsDefault()
    {
        SetLightMode(LightMode.Default);
    }

    public void LightsAll()
    {
        SetLightMode(LightMode.All);
    }

    private void SetLightMode(LightMode mode)
    {
        currentLightMode = mode;

        bool topOn = (mode == LightMode.Default || mode == LightMode.All);
        bool bottomOn = (mode == LightMode.Default || mode == LightMode.All);
        bool camsOn = (mode == LightMode.All);

        if (topLight) topLight.enabled = topOn;
        if (bottomLight) bottomLight.enabled = bottomOn;

        if (cameraLights != null)
        {
            for (int i = 0; i < cameraLights.Length; i++)
            {
                if (cameraLights[i]) cameraLights[i].enabled = camsOn;
            }
        }

        SetLightingButtons(mode);
    }

    private void SetLightingButtons(LightMode mode)
    {
        // Só 1 botão aparece por vez (mesmo lugar), seguindo a sequência:
        // Default (2 luzes) -> All (6 luzes) -> Off (tudo) -> Default...
        if (lightsOffButtonObj)     lightsOffButtonObj.SetActive(false);
        if (lightsDefaultButtonObj) lightsDefaultButtonObj.SetActive(false);
        if (lightsAllButtonObj)     lightsAllButtonObj.SetActive(false);

        if (mode == LightMode.Default)
        {
            // próximo clique: ligar todas (6)
            if (lightsAllButtonObj) lightsAllButtonObj.SetActive(true);
        }
        else if (mode == LightMode.All)
        {
            // próximo clique: desligar tudo
            if (lightsOffButtonObj) lightsOffButtonObj.SetActive(true);
        }
        else // Off
        {
            // próximo clique: voltar pro padrão (2)
            if (lightsDefaultButtonObj) lightsDefaultButtonObj.SetActive(true);
        }
    }

    // =====================================================
    // Beacon Button (optional)
    // =====================================================
    public void OpenBeacon()
    {
        if (string.IsNullOrWhiteSpace(beaconUrl))
        {
            Debug.LogError("MainGameManager: beaconUrl está vazio. Preencha no Inspector.");
            return;
        }

        Application.OpenURL(beaconUrl);
    }
}