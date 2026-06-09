// Not used in program.

using UnityEngine;
using UnityEngine.UIElements;

public class DoubleClickManipulator : PointerManipulator
{
    private float lastClickTime;
    private float threshold = 1.0f;

    public System.Action onDoubleClick;


    // Manipulator function where action is assigned
    public DoubleClickManipulator(System.Action onDoubleClick)
    {
        this.onDoubleClick = onDoubleClick;

        // Activators are what controls trigger the manipulators 
        activators.Add(new ManipulatorActivationFilter
        {
            button = MouseButton.LeftMouse
        });
    }


    // Registers event to targeted element
    protected override void RegisterCallbacksOnTarget()
    {
        target.RegisterCallback<PointerDownEvent>(OnPointerDown);
    }


    // Unregisters event to targeted element
    protected override void UnregisterCallbacksFromTarget()
    {
        target.UnregisterCallback<PointerDownEvent>(OnPointerDown);
    }

    // Function that determines when a double click has occured
    private void OnPointerDown(PointerDownEvent e)
    {
        // Cancel if manipulator can't be started
        if (!CanStartManipulation(e))
            return;

        float time = Time.time;

        // Invoke action event if double click has occured
        if (time - lastClickTime <= threshold)
        {
            onDoubleClick?.Invoke();
            lastClickTime = 0f;
        }
        else // Set last click time to current time
        {
            lastClickTime = time;
        }
    }
}
