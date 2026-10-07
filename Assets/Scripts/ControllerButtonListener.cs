using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Events;
using UnityEngine.XR.Interaction.Toolkit.Interactables;
using UnityEngine.XR.Interaction.Toolkit.Interactors;


public class ControllerButtonDetector : MonoBehaviour
{
    [System.Serializable]
    public class ButtonEvents
    {
        public InputActionProperty action;
        public UnityEvent onPressed;
        public UnityEvent onReleased;
    }

    [System.Serializable]
    public class LeftControllerButtons
    {
        public ButtonEvents xButton = new ButtonEvents();
        public ButtonEvents yButton = new ButtonEvents();
    }

    [System.Serializable]
    public class RightControllerButtons
    {
        public ButtonEvents aButton = new ButtonEvents();
        public ButtonEvents bButton = new ButtonEvents();
    }

    public LeftControllerButtons leftController = new LeftControllerButtons();
    public RightControllerButtons rightController = new RightControllerButtons();

    // Fires when the script is first added to a GameObject,
    // and again whenever "Reset" is chosen from the component context menu.
    void Reset()
    {
        EnsureInitialized();
        RemapInternal(logResult: true);
    }

    private void EnsureInitialized()
    {
        if (leftController == null) leftController = new LeftControllerButtons();
        if (leftController.xButton == null) leftController.xButton = new ButtonEvents();
        if (leftController.yButton == null) leftController.yButton = new ButtonEvents();

        if (rightController == null) rightController = new RightControllerButtons();
        if (rightController.aButton == null) rightController.aButton = new ButtonEvents();
        if (rightController.bButton == null) rightController.bButton = new ButtonEvents();
    }

    void OnEnable()
    {
        EnsureInitialized();

        Subscribe(leftController.xButton, OnXButtonPressed, OnXButtonReleased);
        Subscribe(leftController.yButton, OnYButtonPressed, OnYButtonReleased);
        Subscribe(rightController.aButton, OnAButtonPressed, OnAButtonReleased);
        Subscribe(rightController.bButton, OnBButtonPressed, OnBButtonReleased);

        OnEnable_();
    }

    void OnDisable()
    {
        Unsubscribe(leftController.xButton, OnXButtonPressed, OnXButtonReleased);
        Unsubscribe(leftController.yButton, OnYButtonPressed, OnYButtonReleased);
        Unsubscribe(rightController.aButton, OnAButtonPressed, OnAButtonReleased);
        Unsubscribe(rightController.bButton, OnBButtonPressed, OnBButtonReleased);

        OnDisable_();
    }

    private void Subscribe(ButtonEvents b, System.Action<InputAction.CallbackContext> onPressed, System.Action<InputAction.CallbackContext> onReleased)
    {
        var action = b?.action.action;
        if (action == null) return;

        action.Enable();
        action.performed += onPressed.Invoke;
        action.canceled += onReleased.Invoke;
    }

    private void Unsubscribe(ButtonEvents b, System.Action<InputAction.CallbackContext> onPressed, System.Action<InputAction.CallbackContext> onReleased)
    {
        var action = b?.action.action;
        if (action == null) return;

        action.performed -= onPressed.Invoke;
        action.canceled -= onReleased.Invoke;
        action.Disable();
    }

    // --- X Button ---
    void OnXButtonPressed(InputAction.CallbackContext context)
    {
        Debug.Log("X button pressed!");
        leftController.xButton.onPressed?.Invoke();
    }

    void OnXButtonReleased(InputAction.CallbackContext context)
    {
        Debug.Log("X button released!");
        leftController.xButton.onReleased?.Invoke();
    }

    // --- Y Button ---
    void OnYButtonPressed(InputAction.CallbackContext context)
    {
        Debug.Log("Y button pressed!");
        leftController.yButton.onPressed?.Invoke();
    }

    void OnYButtonReleased(InputAction.CallbackContext context)
    {
        Debug.Log("Y button released!");
        leftController.yButton.onReleased?.Invoke();
    }

    // --- A Button ---
    void OnAButtonPressed(InputAction.CallbackContext context)
    {
        Debug.Log("A button pressed!");
        rightController.aButton.onPressed?.Invoke();
    }

    void OnAButtonReleased(InputAction.CallbackContext context)
    {
        Debug.Log("A button released!");
        rightController.aButton.onReleased?.Invoke();
    }

    // --- B Button ---
    void OnBButtonPressed(InputAction.CallbackContext context)
    {
        Debug.Log("B button pressed!");
        rightController.bButton.onPressed?.Invoke();
    }

    void OnBButtonReleased(InputAction.CallbackContext context)
    {
        Debug.Log("B button released!");
        rightController.bButton.onReleased?.Invoke();
    }

    [ContextMenu("Remap Buttons To Controller Paths")]
    private void ReMap()
    {
        EnsureInitialized();
        RemapInternal(logResult: true);
    }

    private void RemapInternal(bool logResult)
    {
        RemapOne(leftController.xButton, "<XRController>{LeftHand}/primaryButton");
        RemapOne(leftController.yButton, "<XRController>{LeftHand}/secondaryButton");
        RemapOne(rightController.aButton, "<XRController>{RightHand}/primaryButton");
        RemapOne(rightController.bButton, "<XRController>{RightHand}/secondaryButton");

        if (logResult)
            Debug.Log("Controller button bindings remapped.");
    }

    private void RemapOne(ButtonEvents b, string path)
    {
        if (b == null) return;

        var action = b.action.action;
        if (action == null) return; // Use Reference w/ no reference assigned yet — skip quietly

        SetBindingPath(action, path);
    }

    private void SetBindingPath(InputAction action, string path)
    {
        if (action == null) return;

        bool wasEnabled = action.enabled;
        if (wasEnabled) action.Disable();

        if (action.bindings.Count == 0)
            action.AddBinding(path);
        else
            action.ApplyBindingOverride(0, path);

        if (wasEnabled) action.Enable();
    }

    [Header("Socket to release from")]
    public XRSocketInteractor socketInteractor;

    private InputAction rightTriggerAction;

    public UnityEvent OnTriggerENTER;

    private void Awake()
    {
        // Auto-bind to the right-hand trigger across common XR controller layouts
        rightTriggerAction = new InputAction(
            name: "RightTriggerRelease",
            type: InputActionType.Button,
            binding: "<XRController>{RightHand}/triggerPressed"
        );

        // Add fallback bindings for common controller-specific paths
        rightTriggerAction.AddBinding("<OculusTouchController>{RightHand}/triggerPressed");
        rightTriggerAction.AddBinding("<ValveIndexController>{RightHand}/triggerPressed");
        rightTriggerAction.AddBinding("<HTCViveController>{RightHand}/triggerPressed");
    }

    private void OnEnable_()
    {
        rightTriggerAction.performed += OnTriggerPressed;
        rightTriggerAction.Enable();
    }

    private void OnDisable_()
    {
        rightTriggerAction.performed -= OnTriggerPressed;
        rightTriggerAction.Disable();
    }

    private void OnTriggerPressed(InputAction.CallbackContext context)
    {
        ReleaseSocketedItem();
    }

    public void ReleaseSocketedItem()
    {
        OnTriggerENTER?.Invoke();
    }
}