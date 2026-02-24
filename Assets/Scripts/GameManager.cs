using UnityEngine;
using UnityEngine.SceneManagement;

// =====================================================
// MainGameManager.cs
// Versão Final Estável
// Touch + Teclado + Mouse + WebGL + Mobile
// =====================================================

public class MainGameManager : MonoBehaviour
{
    // =====================================================
    // Scenes
    // =====================================================
    [Header("Scenes")]
    [SerializeField] private int menuSceneIndex = 2;

    // =====================================================
    // Spawn
    // =====================================================
    [Header("Spawn")]
    [SerializeField] private Transform spawnPoint;

    [SerializeField] private GameObject lungPrefab;
    [SerializeField] private GameObject bronchusPrefab;
    [SerializeField] private GameObject alveolusPrefab;
    [SerializeField] private GameObject tracheaPrefab;
    [SerializeField] private GameObject heartPrefab;

    // =====================================================
    // UI
    // =====================================================
    [Header("UI")]
    [SerializeField] private GameObject optionsPanel;

    [SerializeField] private GameObject menuOpenButtonObj;
    [SerializeField] private GameObject menuCloseButtonObj;

    [SerializeField] private GameObject playButtonObj;
    [SerializeField] private GameObject pauseButtonObj;

    // =====================================================
    // Lighting
    // =====================================================
    [Header("Lighting")]
    [SerializeField] private Light topLight;
    [SerializeField] private Light bottomLight;
    [SerializeField] private Light[] cameraLights;

    [SerializeField] private GameObject lightsOffButtonObj;
    [SerializeField] private GameObject lightsDefaultButtonObj;
    [SerializeField] private GameObject lightsAllButtonObj;

    // =====================================================
    // Rotation
    // =====================================================
    [Header("Rotation")]
    [SerializeField] private float rotationSpeedDegPerSec = 80f;

    // =====================================================
    // Zoom
    // =====================================================
    [Header("Zoom")]
    [SerializeField] private float zoomSpeedPerSec = 0.8f;
    [SerializeField] private float minScale = 0.2f;
    [SerializeField] private float maxScale = 3f;

    // =====================================================
    // Runtime
    // =====================================================
    private GameObject currentModel;
    private Animator currentAnimator;

    private enum Dir { None, Left, Right }
    private enum LightMode { Off, Default, All }

    private Dir holdRotate = Dir.None;
    private Dir autoRotate = Dir.None;
    private Dir holdZoom = Dir.None;

    private LightMode currentLightMode = LightMode.Default;

    private float lastRightKeyTime;
    private float lastLeftKeyTime;
    private float doubleClickThreshold = 0.35f;

    private Vector3 defaultScale = Vector3.one;

    // =====================================================
    // UNITY
    // =====================================================
    private void Start()
    {
        SpawnSelectedPrefab();

        if (optionsPanel) optionsPanel.SetActive(false);
        SetMenuToggle(false);
        SetPlayPause(true);

        SetLightMode(LightMode.Default);
    }

    private void Update()
    {
        if (currentModel == null) return;

        HandleKeyboard();
        HandleRotation();
        HandleZoom();
    }

    // =====================================================
    // KEYBOARD
    // =====================================================
    private void HandleKeyboard()
    {
        // Play / Pause
        if (Input.GetKeyDown(KeyCode.P))
        {
            if (currentAnimator && currentAnimator.speed > 0)
                PauseAnimation();
            else
                PlayAnimation();
        }

        // Menu
        if (Input.GetKeyDown(KeyCode.M))
        {
            if (optionsPanel && optionsPanel.activeSelf)
                CloseOptionsMenu();
            else
                OpenOptionsMenu();
        }

        // Light cycle
        if (Input.GetKeyDown(KeyCode.L))
            CycleLightMode();

        // Double click RIGHT
        if (Input.GetKeyDown(KeyCode.D) || Input.GetKeyDown(KeyCode.RightArrow))
        {
            if (Time.time - lastRightKeyTime < doubleClickThreshold)
                ToggleAutoRotate(Dir.Right);

            lastRightKeyTime = Time.time;
        }

        // Double click LEFT
        if (Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.LeftArrow))
        {
            if (Time.time - lastLeftKeyTime < doubleClickThreshold)
                ToggleAutoRotate(Dir.Left);

            lastLeftKeyTime = Time.time;
        }
    }

    // =====================================================
    // ROTATION (Touch tem prioridade)
    // =====================================================
    private void HandleRotation()
    {
        Dir keyboardDir = Dir.None;

        if (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow))
            keyboardDir = Dir.Right;
        else if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow))
            keyboardDir = Dir.Left;

        Dir finalDir =
            holdRotate != Dir.None ? holdRotate :
            keyboardDir != Dir.None ? keyboardDir :
            autoRotate;

        if (finalDir == Dir.Right)
            currentModel.transform.Rotate(0, rotationSpeedDegPerSec * Time.deltaTime, 0, Space.World);
        else if (finalDir == Dir.Left)
            currentModel.transform.Rotate(0, -rotationSpeedDegPerSec * Time.deltaTime, 0, Space.World);
    }

    private void ToggleAutoRotate(Dir dir)
    {
        autoRotate = (autoRotate == dir) ? Dir.None : dir;
    }

    // =====================================================
    // ZOOM (Touch prioridade)
    // =====================================================
    private void HandleZoom()
    {
        Dir keyboardZoom = Dir.None;

        if (Input.GetKey(KeyCode.W))
            keyboardZoom = Dir.Right;
        else if (Input.GetKey(KeyCode.S))
            keyboardZoom = Dir.Left;

        // Scroll
        float scroll = Input.mouseScrollDelta.y;
        if (scroll > 0) ApplyScaleDelta(+zoomSpeedPerSec * 5f * Time.deltaTime);
        if (scroll < 0) ApplyScaleDelta(-zoomSpeedPerSec * 5f * Time.deltaTime);

        // Scroll click reset
        if (Input.GetMouseButtonDown(2))
            currentModel.transform.localScale = defaultScale;

        Dir finalZoom =
            holdZoom != Dir.None ? holdZoom :
            keyboardZoom;

        if (finalZoom == Dir.Right)
            ApplyScaleDelta(+zoomSpeedPerSec * Time.deltaTime);
        else if (finalZoom == Dir.Left)
            ApplyScaleDelta(-zoomSpeedPerSec * Time.deltaTime);
    }

    private void ApplyScaleDelta(float delta)
    {
        Vector3 s = currentModel.transform.localScale;
        float target = Mathf.Clamp(s.x + delta, minScale, maxScale);
        currentModel.transform.localScale = new Vector3(target, target, target);
    }

    // =====================================================
    // SPAWN
    // =====================================================
    private void SpawnSelectedPrefab()
    {
        if (!spawnPoint) return;

        GameObject prefab = lungPrefab;

        switch (SelectedModel.Selected)
        {
            case SelectedModel.Choice.Lung: prefab = lungPrefab; break;
            case SelectedModel.Choice.Bronchus: prefab = bronchusPrefab; break;
            case SelectedModel.Choice.Alveolus: prefab = alveolusPrefab; break;
            case SelectedModel.Choice.Trachea: prefab = tracheaPrefab; break;
            case SelectedModel.Choice.Heart: prefab = heartPrefab; break;
        }

        if (!prefab) return;

        currentModel = Instantiate(prefab, spawnPoint.position, spawnPoint.rotation);
        currentModel.transform.localScale = defaultScale;

        currentAnimator = currentModel.GetComponentInChildren<Animator>();
        if (currentAnimator) currentAnimator.speed = 1f;
    }

    // =====================================================
    // TOUCH METHODS
    // =====================================================
    public void HoldTurnRightStart() { holdRotate = Dir.Right; }
    public void HoldTurnRightEnd() { if (holdRotate == Dir.Right) holdRotate = Dir.None; }

    public void HoldTurnLeftStart() { holdRotate = Dir.Left; }
    public void HoldTurnLeftEnd() { if (holdRotate == Dir.Left) holdRotate = Dir.None; }

    public void HoldZoomInStart() { holdZoom = Dir.Right; }
    public void HoldZoomInEnd() { if (holdZoom == Dir.Right) holdZoom = Dir.None; }

    public void HoldZoomOutStart() { holdZoom = Dir.Left; }
    public void HoldZoomOutEnd() { if (holdZoom == Dir.Left) holdZoom = Dir.None; }

    // =====================================================
    // AUTO ROTATE (Touch Toggle)
    // =====================================================
    public void ToggleAutoTurnRight()
    {
    autoRotate = (autoRotate == Dir.Right) ? Dir.None : Dir.Right;
    }

    public void ToggleAutoTurnLeft()
    {
    autoRotate = (autoRotate == Dir.Left) ? Dir.None : Dir.Left;
    }

  // =====================================================
// LIGHTING (modo ciclo com botão único)
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

private void CycleLightMode()
{
    if (currentLightMode == LightMode.Default)
        SetLightMode(LightMode.All);
    else if (currentLightMode == LightMode.All)
        SetLightMode(LightMode.Off);
    else
        SetLightMode(LightMode.Default);
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
        foreach (var l in cameraLights)
            if (l) l.enabled = camsOn;

    SetLightingButtons(mode);
}

private void SetLightingButtons(LightMode mode)
{
    if (lightsOffButtonObj) lightsOffButtonObj.SetActive(false);
    if (lightsDefaultButtonObj) lightsDefaultButtonObj.SetActive(false);
    if (lightsAllButtonObj) lightsAllButtonObj.SetActive(false);

    if (mode == LightMode.Default)
    {
        if (lightsAllButtonObj) lightsAllButtonObj.SetActive(true);
    }
    else if (mode == LightMode.All)
    {
        if (lightsOffButtonObj) lightsOffButtonObj.SetActive(true);
    }
    else
    {
        if (lightsDefaultButtonObj) lightsDefaultButtonObj.SetActive(true);
    }
}

    // =====================================================
    // MENU
    // =====================================================
    public void BackToMenu() => SceneManager.LoadScene(menuSceneIndex);

    public void OpenOptionsMenu()
    {
        if (optionsPanel) optionsPanel.SetActive(true);
        SetMenuToggle(true);
    }

    public void CloseOptionsMenu()
    {
        if (optionsPanel) optionsPanel.SetActive(false);
        SetMenuToggle(false);
    }

    private void SetMenuToggle(bool isOpen)
    {
        if (menuOpenButtonObj) menuOpenButtonObj.SetActive(!isOpen);
        if (menuCloseButtonObj) menuCloseButtonObj.SetActive(isOpen);
    }

    // =====================================================
    // PLAY / PAUSE
    // =====================================================
    public void PlayAnimation()
    {
        if (currentAnimator) currentAnimator.speed = 1f;
        SetPlayPause(true);
    }

    public void PauseAnimation()
    {
        if (currentAnimator) currentAnimator.speed = 0f;
        SetPlayPause(false);
    }

    private void SetPlayPause(bool isPlaying)
    {
        if (playButtonObj) playButtonObj.SetActive(!isPlaying);
        if (pauseButtonObj) pauseButtonObj.SetActive(isPlaying);
    }
}