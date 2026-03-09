using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;
using UnityEngine.Events;

// =====================================================
// GAME MANAGER
// Script principal da cena.
// Controla modelo, câmera, interface, luz e interações.
// =====================================================
public class MainGameManager : MonoBehaviour
{
    // =====================================================
    // SCENES
    // =====================================================
    [Header("Scenes")]
    [SerializeField] private int menuSceneIndex = 2;

    // =====================================================
    // SPAWN
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
    // CAMERA MODE UI
    // Botões de troca entre modo 1 face e 4 faces
    // =====================================================
    [Header("Camera Mode UI")]
    [SerializeField] private GameObject oneFaceButtonObj;
    [SerializeField] private GameObject fourFacesButtonObj;

    // =====================================================
    // CAMERAS
    // =====================================================
    [Header("Cameras")]
    [SerializeField] private Camera northCamera;
    [SerializeField] private Camera southCamera;
    [SerializeField] private Camera eastCamera;
    [SerializeField] private Camera westCamera;
    [SerializeField] private Camera centerCamera;

    // =====================================================
    // CAMERA VIEW UI
    // Máscaras visuais de cada câmera
    // =====================================================
    [Header("Camera View UI")]
    [SerializeField] private GameObject maskNorthObj;
    [SerializeField] private GameObject maskSouthObj;
    [SerializeField] private GameObject maskEastObj;
    [SerializeField] private GameObject maskWestObj;
    [SerializeField] private GameObject maskCenterObj;

    // =====================================================
    // LIGHTING
    // =====================================================
    [Header("Lighting")]
    [SerializeField] private Light topLight;
    [SerializeField] private Light bottomLight;
    [SerializeField] private Light[] cameraLights;

    [SerializeField] private GameObject lightsOffButtonObj;
    [SerializeField] private GameObject lightsDefaultButtonObj;
    [SerializeField] private GameObject lightsAllButtonObj;

    // =====================================================
    // ROTATION
    // =====================================================
    [Header("Rotation")]
    [SerializeField] private float rotationSpeedDegPerSec = 80f;

    // =====================================================
    // ZOOM
    // =====================================================
    [Header("Zoom")]
    [SerializeField] private float zoomSpeedPerSec = 0.8f;
    [SerializeField] private float minScale = 0.2f;
    [SerializeField] private float maxScale = 3f;

    // =====================================================
    // RUNTIME
    // Variáveis usadas durante a execução
    // =====================================================
    private GameObject currentModel;
    private Animator currentAnimator;

    private enum Dir { None, Left, Right }
    private enum LightMode { Off, Default, All }
    private enum CameraMode { FourFaces, OneFace }

    private Dir holdRotate = Dir.None;
    private Dir autoRotate = Dir.None;
    private Dir holdZoom = Dir.None;

    private LightMode currentLightMode = LightMode.Default;
    private CameraMode currentCameraMode = CameraMode.FourFaces;

    private float lastRightKeyTime;
    private float lastLeftKeyTime;
    private float doubleClickThreshold = 0.35f;

    private Vector3 defaultScale = Vector3.one;

    // =====================================================
    // UNITY
    // Métodos principais do ciclo de vida
    // =====================================================
    private void Awake()
    {
        ForceLandscapeOnly();
    }

    private void Start()
    {
        SpawnSelectedPrefab();

        if (optionsPanel) optionsPanel.SetActive(false);
        SetMenuToggle(false);
        SetPlayPause(true);

        SetLightMode(LightMode.Default);
        SetCameraMode(CameraMode.FourFaces);
    }

    private void Update()
    {
        if (currentModel == null) return;

        HandleKeyboard();
        HandleRotation();
        HandleZoom();
    }

    // =====================================================
    // ORIENTATION
    // Força uso apenas em horizontal
    // =====================================================
    private void ForceLandscapeOnly()
    {
        Screen.orientation = ScreenOrientation.AutoRotation;

        Screen.autorotateToPortrait = false;
        Screen.autorotateToPortraitUpsideDown = false;
        Screen.autorotateToLandscapeLeft = true;
        Screen.autorotateToLandscapeRight = true;
    }

    // =====================================================
    // KEYBOARD
    // Atalhos de teclado
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

        // Luz
        if (Input.GetKeyDown(KeyCode.L))
            CycleLightMode();

        // Câmera
        if (Input.GetKeyDown(KeyCode.Alpha1) || Input.GetKeyDown(KeyCode.Keypad1))
            ActivateOneFace();

        if (Input.GetKeyDown(KeyCode.Alpha4) || Input.GetKeyDown(KeyCode.Keypad4))
            ActivateFourFaces();

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
    // ROTATION
    // Touch tem prioridade sobre teclado e auto
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
    // ZOOM
    // Touch tem prioridade sobre teclado
    // =====================================================
    private void HandleZoom()
    {
        Dir keyboardZoom = Dir.None;

        if (Input.GetKey(KeyCode.W))
            keyboardZoom = Dir.Right;
        else if (Input.GetKey(KeyCode.S))
            keyboardZoom = Dir.Left;

        float scroll = Input.mouseScrollDelta.y;

        if (scroll > 0) ApplyScaleDelta(+zoomSpeedPerSec * 5f * Time.deltaTime);
        if (scroll < 0) ApplyScaleDelta(-zoomSpeedPerSec * 5f * Time.deltaTime);

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
    // Instancia o prefab selecionado
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
    // Eventos de segurar botão
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
    // AUTO ROTATE
    // Liga ou desliga rotação automática
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
    // CAMERA MODE
    // Alterna entre 4 faces e câmera central
    // =====================================================
    public void ActivateFourFaces()
    {
        SetCameraMode(CameraMode.FourFaces);
    }

    public void ActivateOneFace()
    {
        SetCameraMode(CameraMode.OneFace);
    }

    private void SetCameraMode(CameraMode mode)
    {
        currentCameraMode = mode;

        bool fourFaces = (mode == CameraMode.FourFaces);
        bool oneFace = (mode == CameraMode.OneFace);

        if (northCamera) northCamera.gameObject.SetActive(fourFaces);
        if (southCamera) southCamera.gameObject.SetActive(fourFaces);
        if (eastCamera) eastCamera.gameObject.SetActive(fourFaces);
        if (westCamera) westCamera.gameObject.SetActive(fourFaces);
        if (centerCamera) centerCamera.gameObject.SetActive(oneFace);

        if (maskNorthObj) maskNorthObj.SetActive(fourFaces);
        if (maskSouthObj) maskSouthObj.SetActive(fourFaces);
        if (maskEastObj) maskEastObj.SetActive(fourFaces);
        if (maskWestObj) maskWestObj.SetActive(fourFaces);
        if (maskCenterObj) maskCenterObj.SetActive(oneFace);

        UpdateCameraModeButtons();
    }

    private void UpdateCameraModeButtons()
    {
        if (oneFaceButtonObj) oneFaceButtonObj.SetActive(currentCameraMode == CameraMode.FourFaces);
        if (fourFacesButtonObj) fourFacesButtonObj.SetActive(currentCameraMode == CameraMode.OneFace);
    }

    // =====================================================
    // LIGHTING
    // Controla os modos de luz
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
        {
            foreach (var l in cameraLights)
            {
                if (l) l.enabled = camsOn;
            }
        }

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
    // Controle do painel de opções
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
    // Controle da animação do modelo
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