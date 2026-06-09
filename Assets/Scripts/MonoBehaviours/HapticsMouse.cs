using System;
using System.ComponentModel;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;
using UnityEngine.InputSystem.Layouts;
using UnityEngine.InputSystem.LowLevel;
using UnityEngine.InputSystem.XR.Haptics;
using UnityEngine.UIElements;
using static UnityEngine.GraphicsBuffer;

public class HapticsMouse : MonoBehaviour
{
    private Mouse virutalMouse;
    [Header("Haptics Device")]
    public HapticPlugin plugin;
    private bool _isHapticControlEnabled = false;

    [Header("Haptics To Cursor Translation Settings")]
    public float screenDividerX = 192.0f;
    public float screenDividerY = 100.8f;

    [Header("Cursor Display Settings")]
    public Texture2D cursorTexture;
    public CursorMode cursorMode = CursorMode.Auto;
    public Vector2 hotSpot = Vector2.zero;

    [Header("Keyboard Input")]
    [SerializeField] private InputActionAsset _inputActions;
    private InputAction _toggle;

    [Header("UI Document")]
    public MenuScript menuScript;

    private void Awake()
    {
        var playerMap = _inputActions.FindActionMap("Player", throwIfNotFound: true);
        _toggle = playerMap.FindAction("Jump", throwIfNotFound: true);
        _toggle.performed += context => ToggleTranslate();
    }

    private void OnEnable()
    {
        _toggle.Enable();
    }

    private void OnDisable()
    {
        _toggle.Disable();
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        UnityEngine.Cursor.SetCursor(cursorTexture, hotSpot, cursorMode);
    }

    // Update is called once per frame
    void Update()
    {
        HapticDeviceCursorControl();
    }

    private void HapticDeviceCursorControl()
    {
        if (plugin.DeviceIdentifier != "Not Connected" && _isHapticControlEnabled)
        {
            Vector2 devicePosition = plugin.CurrentPosition;
            TranslateMousePosition(devicePosition);
        }
    }

    private void TranslateMousePosition(Vector2 newPosition)
    {
        newPosition.x = newPosition.x * (-1 * Screen.width / screenDividerX) + (Screen.width / 2);
        newPosition.y = newPosition.y * (Screen.height / screenDividerY) + (Screen.height / 2);
        Mouse.current.WarpCursorPosition(newPosition);
    }

    // Action Function for when _toggle is performed. 
    private void ToggleTranslate()
    {
        _isHapticControlEnabled = !_isHapticControlEnabled;
    }

    public bool GetControlStatus()
    {
        return _isHapticControlEnabled;
    }

    public void ClickButton()
    {
        menuScript.ClickButton();
    }

    public void LongClickButton()
    {
        menuScript.LongClickButton();
    }
}
