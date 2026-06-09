using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Composites;
using UnityEngine.Rendering;
using UnityEngine.UIElements;

public class MenuScript : MonoBehaviour
{
    [Header("UI Document Settings")]
    private VisualElement _root;

    [Header("Haptics Settings")]
    public GameObject hapticActor;
    public float hapticReadDelay = 0.5f; 
    private HapticPlugin _hapticPlugin;
    private HapticsMouse _hapticMouse;

    [Header("Button Hover Settings")]
    public Vector3 hoverGDirection = new Vector3(1, 0, 0);
    public float hoverGMag = 1;
    public int hoverFrequency = 50;
    public float hoverTime = 10.0f;

    [Header("Braille Translation Settinsg")]
    [SerializeField] private BrailleTranslator _translator;

    private void Start()
    {
        // Get Haptic Device components
        _hapticPlugin = hapticActor.GetComponent<HapticPlugin>();
        _hapticMouse = hapticActor.GetComponent<HapticsMouse>();

        // Get root and all buttons with menu-item class name in 
        // the document. 
        _root = GetComponent<UIDocument>().rootVisualElement;
        var buttons = _root.Query<Button>(className: "menu-item").ToList();

        // Loop through every button and add the long press manipulator
        // to each with actions assigned.
        foreach (var btn in buttons)
        {
            btn.clickable = null;
            LongPressManipulator lp = new LongPressManipulator(
                onSinglePress: () => ReadText(btn.text),
                onLongPress: () => AddToCart(btn.text),
                onHover: () => ButtonHover()
            );
            btn.AddManipulator(lp);
            btn.userData = lp;
        }

        var checkOut = _root.Query<Button>(className: "check-out-button").ToList()[0];

        checkOut.clickable = null;
        LongPressManipulator check = new LongPressManipulator(
                onSinglePress: () => ReadText(checkOut.text),
                onLongPress: () => TransitionToCheckOut(),
                onHover: () => ButtonHover()
            );
        checkOut.AddManipulator(check);
        checkOut.userData = check;

        UpdateCartList();
        ShuffleButtons();
    }

    // Read name (text) of the button. Use this to output braille.
    public void ReadText(string text)
    {
        if (_hapticPlugin.DeviceIdentifier != "Not Connected" && _hapticMouse.GetControlStatus())
        {
            if (TestManager.instance.IsAudioPlayback())
                PlaySoundByName(text);
            else
                _translator.HapticTranslation(_translator.StringTranslate(text));
        }
        else
        {
            if (TestManager.instance.IsAudioPlayback())
                PlaySoundByName(text);
            else
            {
                string braille = _translator.StringTranslate(text);
                Debug.Log(braille);
                Debug.Log(_translator.BrailleTranslate(braille));
                _translator.HapticTranslation(_translator.StringTranslate(text));
            }
                
        }
    }

    // Add menu item to cart. Will take the text from the button
    // and add it to cart for checkout.
    public void AddToCart(string text)
    {
        CartManager.instance.AddToCart(text);
        //CartManager.instance.PrintCartContents();
        UpdateCartList();

        _hapticPlugin.ActivateVibration(new Vector3(1, 0, 0), 1, 200, 10.0f);
    }

    private void TransitionToCheckOut()
    {
        TestManager.instance.TestComplete();
        CartManager.instance.SaveCartHistory();
        CartManager.instance.ClearCart();
        UpdateCartList();
        ShuffleButtons();
        _hapticPlugin.ActivateVibration(new Vector3(1, 0, 0), 3, 200, 10.0f);
    }

    private void UpdateCartList()
    {
        var listView = _root.Query<ListView>(className: "cart-list").ToList()[0];
        List<string> cartReference = CartManager.instance.GetCartList();
        listView.itemsSource = cartReference;

        listView.makeItem = () => new Label();
        listView.bindItem = (VisualElement element, int index) =>
        {
            (element as Label).text = cartReference[index];
        };
    }

    private void ShuffleButtons()
    {
        var buttons = _root.Query<Button>(className: "menu-item").ToList();
        List<string> shuffledItems = TestManager.instance.GetShuffledTestItems();

        int i = 0;
        foreach (Button button in buttons)
        {
            button.text = shuffledItems[i++];
        }
    }

    public Button PickButton()
    {
        Vector2 mousePosition = Mouse.current.position.ReadValue();
        mousePosition.y = Screen.height - mousePosition.y;
        var picked = _root.panel.Pick(mousePosition);
        if (picked is Button button)
        {
            return button;
        }
        return null;
    }

    public void ClickButton()
    {
        Button button = PickButton();
        if (button != null && button.userData is LongPressManipulator)
        {
            LongPressManipulator lp = (LongPressManipulator)button.userData;
            lp.InvokeSinglePress();
        }
    }

    public void LongClickButton()
    {
        Button button = PickButton();
        if (button != null && button.userData is LongPressManipulator)
        {
            LongPressManipulator lp = (LongPressManipulator)button.userData;
            lp.InvokeLongPress();
        }
    }

    public void ButtonHover()
    {
        if (!_translator.GetReadStatus())
            _hapticPlugin.ActivateVibration(hoverGDirection, hoverGMag, hoverFrequency, hoverTime);
    }

    public void PlaySoundByName(string name)
    {
        AudioManager.Instance.Play(name);
    }
}

