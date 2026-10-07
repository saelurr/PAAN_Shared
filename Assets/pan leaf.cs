using UnityEngine;
using UnityEngine.InputSystem;

public class VRSequentialTrigger : MonoBehaviour
{
    [Tooltip("The Animator containing your pan leaf animations.")]
    public Animator panLeafAnimator;

    [Tooltip("List of trigger names in the exact order you want them to play.")]
    public string[] triggerSequence;

    [Tooltip("The VR button you want to press to progress the animations.")]
    public InputActionReference buttonAction;

    [Tooltip("The leaf's XR Grab Interactable, enabled once folding starts.")]
    public UnityEngine.XR.Interaction.Toolkit.Interactables.XRGrabInteractable leafGrabInteractable;

    private int currentStep = 0;

    private void OnEnable()
    {
        if (buttonAction != null)
        {
            buttonAction.action.performed += TriggerNextStep;
        }
    }

    private void OnDisable()
    {
        if (buttonAction != null)
        {
            buttonAction.action.performed -= TriggerNextStep;
        }
    }

    private void TriggerNextStep(InputAction.CallbackContext context)
    {
        if (panLeafAnimator != null && currentStep < triggerSequence.Length)
        {
            if (currentStep == 0)
            {
                if (leafGrabInteractable != null)
                    leafGrabInteractable.enabled = true;

                HidePlacedIngredients();
            }

            string triggerToFire = triggerSequence[currentStep];
            panLeafAnimator.SetTrigger(triggerToFire);
            currentStep++;
        }
        else if (currentStep >= triggerSequence.Length)
        {
            currentStep = 0;
        }
    }

    private void HidePlacedIngredients()
    {
        SnappableIngredient[] allIngredients = FindObjectsOfType<SnappableIngredient>();
        Debug.Log("HidePlacedIngredients called. Found " + allIngredients.Length + " ingredients total.");

        foreach (SnappableIngredient ingredient in allIngredients)
        {
            Debug.Log(ingredient.ingredientName + " isPlaced: " + ingredient.isPlaced);
            if (ingredient.isPlaced)
            {
                ingredient.gameObject.SetActive(false);
                Debug.Log("Hiding: " + ingredient.ingredientName);
            }
        }
    }
}