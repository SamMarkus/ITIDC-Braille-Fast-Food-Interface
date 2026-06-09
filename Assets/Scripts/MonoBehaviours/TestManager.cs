using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;

public class TestManager : MonoBehaviour
{
    public static TestManager instance { get; set; }

    [Header("Test Settings")]
    [SerializeField] private bool EnableTest;
    [SerializeField] private bool PlaybackAudio;
    [SerializeField] private int testCount = 10;
    [SerializeField] private int numOfRequired = 3;
    private int currentTests = 0;

    [Header("Test Environment Settings")]
    [SerializeField] private List<String> testItems;
    [SerializeField] private List<String> requiredItems;
    [SerializeField] private List<int> testOutcomes;

    [Header("Timer Variables")]
    [SerializeField] private List<float> timeToTake = new();
    private float currentTime = 0f;

    [Header("UI Document")]
    public UIDocument document;
    private VisualElement _root;
    private Label _timerLabel;
    private Label _cartNumber;

    [Header("Plugin")]
    public HapticPlugin plugin;

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(this);
        }
        else
        {
            instance = this;
        }
    }

    private void Start()
    {
        if (EnableTest)
        {
            _root = document.rootVisualElement;
            VisualElement container = _root.Query<VisualElement>(className: "test-container").ToList()[0];
            container.visible = true;
            _timerLabel = _root.Query<Label>(className: "timer-text").ToList()[0];
            _cartNumber = _root.Query<Label>(className: "cart-number").ToList()[0];

            GetNewRequiredList();
        }
        
    }

    private void Update()
    {
        currentTime += Time.deltaTime;
        if (EnableTest)
        {
            UpdateTestCounter();
            UpdateCartNumber();
        }
        CheckIfTestsAreDone();
    }


    private void CheckIfTestsAreDone()
    {
        if (currentTests == testCount)
        {
            plugin.DisableVibration();
            Debug.Break();
        }
    }

    public void IncrementTestCounter()
    {
        currentTests++;
    }

    public void TestComplete()
    {
        timeToTake.Add(currentTime);
        currentTime = 0;
        CheckCart();
        IncrementTestCounter();

        if (EnableTest) GetNewRequiredList();
    }

    public List<String> GetShuffledTestItems()
    {
        // Using Knuth Shuffle 
        System.Random seed = new System.Random();
        int i = testItems.Count;
        while (i > 1)
        {
            int l = seed.Next(i--);
            String temp = testItems[i];
            testItems[i] = testItems[l];
            testItems[l] = temp;
        }

        return testItems;
    }

    public void GetNewRequiredList()
    {
        UnityEngine.Random.InitState((int)DateTime.Now.Ticks);
        requiredItems.Clear();
        for (int j = 0; j < numOfRequired; j++)
        {
            int index = UnityEngine.Random.Range(0, testItems.Count);
            requiredItems.Add(testItems[index]);
        }
        UpdateRequiredList();
    }

    private void UpdateTestCounter()
    {
        if (currentTests == testCount)
        {
            _timerLabel.text = $"Experiment completed.";
        }
        else
        {
            _timerLabel.text = $"Test {(int)currentTests + 1}";
        }
    }

    private void UpdateRequiredList()
    {
        var listView = _root.Query<ListView>(className: "required-list").ToList()[0];
        List<String> items = requiredItems; 
        listView.itemsSource = items;

        listView.makeItem = () => new Label();
        listView.bindItem = (VisualElement element, int index) =>
        {
            (element as Label).text = items[index];
        };
    }

    private void UpdateCartNumber()
    {
        int num = CartManager.instance.GetCount();
        _cartNumber.text = $"In Cart: {num}";
    }

    private void CheckCart()
    {
        List<String> cart = CartManager.instance.GetCartList();
        int outcome = 0;
        if (cart.Count == requiredItems.Count)
        {
            foreach (var element in requiredItems)
            {
                if (cart.Contains(element))
                {
                    outcome++;
                }
            }
            
        }
        testOutcomes.Add(outcome);
        CartManager.instance.SaveRequiredHistory(requiredItems);
    }

    public bool GetTestStatus()
    {
        return EnableTest;
    }

    public bool IsAudioPlayback()
    {
        return PlaybackAudio;
    }
}
