using System;
using UnityEngine;
using UnityEngine.InputSystem.XR;
using UnityEngine.UIElements;

public class LongPressManipulator : PointerManipulator
{
    public System.Action _onSinglePress;
    public System.Action _onLongPress;
    public System.Action _onHover;
    private float longPressTimeActivation = 1.0f;
    private float startTime = 0.0f;
    private bool triggered = false;
    private IVisualElementScheduledItem scheduledItem;


    // Constructor: Take in a onSinglePress action and a onLongPress action
    public LongPressManipulator(System.Action onSinglePress, System.Action onLongPress, System.Action onHover)
    {
        // Assign actions to class attributes   
        _onSinglePress = onSinglePress;
        _onLongPress = onLongPress;
        _onHover = onHover;

        // Add activators, what controls will trigger the manipulation
        activators.Add(new ManipulatorActivationFilter { button = MouseButton.LeftMouse });
    }

    // OverrideMethod: Register events 
    protected override void RegisterCallbacksOnTarget()
    {
        target.RegisterCallback<PointerDownEvent>(OnDown);
        target.RegisterCallback<PointerUpEvent>(OnUp);
        target.RegisterCallback<PointerOverEvent>(OnHover);
    }

    // OverrideMethod: Unregister events
    protected override void UnregisterCallbacksFromTarget()
    {
        target.UnregisterCallback<PointerDownEvent>(OnDown);
        target.UnregisterCallback<PointerUpEvent>(OnUp);
        target.UnregisterCallback<PointerOverEvent>(OnHover);
    }

    // Method: On pointer down, start checking for long press
    private void OnDown(PointerDownEvent evt)
    {
        startTime = Time.time;
        scheduledItem = target.schedule.Execute(CheckCondition).Every(10);
    }

    // Method: On pointer up, clean up and check if long press was actvated or not.
    //         Activate single press if true.
    private void OnUp(PointerUpEvent evt)
    {
        scheduledItem?.Pause();
        if (triggered)
        {
            evt.StopImmediatePropagation();
            triggered = false;
        }
        else
        {
            _onSinglePress.Invoke();
        }
    }

    private void OnHover(PointerOverEvent evt)
    {
        _onHover.Invoke();
    }

    // Method: Scheduled event that is called to check if a long press occured
    private void CheckCondition()
    {
        if (CheckTimeElapsed() >= longPressTimeActivation && !triggered)
        {
            _onLongPress.Invoke();
            triggered = true;
        }
    }

    // Method: Check how much time has elapsed
    private float CheckTimeElapsed()
    {
        return Time.time - startTime;
    }

    public void InvokeSinglePress()
    {
        _onSinglePress.Invoke();
    }

    public void InvokeLongPress()
    {
        _onLongPress.Invoke();
    }
}