using UnityEngine;
using UnityEngine.InputSystem; // Requires Input System Package

public class PaanFoldingController : MonoBehaviour
{
    [Header("Animator Reference")]
    [SerializeField] private Animator paanAnimator;

    [Header("Animator Trigger Names")]
    [SerializeField] private string leftTriggerName = "left_trigger";
    [SerializeField] private string rightTriggerName = "right_trigger";
    [SerializeField] private string tipTriggerName = "tip_trigger";

    [Header("VR Input Action References")]
    [Tooltip("Reference to the VR button action (e.g., PrimaryButton [A/X], SecondaryButton [B/Y], or Trigger)")]
    [SerializeField] private InputActionProperty foldNextInput;

    // Track sequence step: 0 = Left, 1 = Right, 2 = Tip
    private int currentFoldStep = 0;

    private void OnEnable()
    {
        if (foldNextInput.action != null)
        {
            foldNextInput.action.Enable();
            foldNextInput.action.performed += OnButtonPressed;
        }
    }

    private void OnDisable()
    {
        if (foldNextInput.action != null)
        {
            foldNextInput.action.performed -= OnButtonPressed;
            foldNextInput.action.Disable();
        }
    }

    private void Update()
    {
        // Keyboard testing shortcuts in Editor:
        if (Input.GetKeyDown(KeyCode.Alpha1)) TriggerFoldLeft();
        if (Input.GetKeyDown(KeyCode.Alpha2)) TriggerFoldRight();
        if (Input.GetKeyDown(KeyCode.Alpha3)) TriggerFoldTip();
    }

    private void OnButtonPressed(InputAction.CallbackContext context)
    {
        // Cycles through Left -> Right -> Tip on each button press
        switch (currentFoldStep)
        {
            case 0:
                TriggerFoldLeft();
                currentFoldStep = 1;
                break;
            case 1:
                TriggerFoldRight();
                currentFoldStep = 2;
                break;
            case 2:
                TriggerFoldTip();
                currentFoldStep = 3; // Fully folded
                break;
        }
    }

    public void TriggerFoldLeft()
    {
        if (paanAnimator != null)
        {
            paanAnimator.SetTrigger(leftTriggerName);
            Debug.Log("Paan Fold Left Activated");
        }
    }

    public void TriggerFoldRight()
    {
        if (paanAnimator != null)
        {
            paanAnimator.SetTrigger(rightTriggerName);
            Debug.Log("Paan Fold Right Activated");
        }
    }

    public void TriggerFoldTip()
    {
        if (paanAnimator != null)
        {
            paanAnimator.SetTrigger(tipTriggerName);
            Debug.Log("Paan Fold Tip Activated");
        }
    }
}