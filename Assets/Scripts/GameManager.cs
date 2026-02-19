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
    // Se quiser colocar o botão aqui também, já está pronto.
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
    private Dir holdRotate = Dir.None;
    private Dir autoRotate = Dir.None;

    private Dir holdZoom = Dir.None; // Left = ZoomOut, Right = ZoomIn

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