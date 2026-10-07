using UnityEngine;

public class VRIngredientSnap : MonoBehaviour
{
    [Header("Transform Anchors")]
    public Transform spoonTip;
    public Transform leafSnapPoint;

    private GameObject currentIngredient = null;

    // Call this when the VR controller selects the ingredient
    public void OnIngredientSelected(GameObject ingredient)
    {
        if (currentIngredient != null) return; // Spoon is already full

        currentIngredient = ingredient;

        // Disable physics/colliders so it doesn't collide with the spoon
        if (currentIngredient.TryGetComponent<Rigidbody>(out Rigidbody rb))
        {
            rb.isKinematic = true;
        }

        if (currentIngredient.TryGetComponent<Collider>(out Collider col))
        {
            col.enabled = false;
        }

        // Snap to spoon tip transform
        currentIngredient.transform.SetParent(spoonTip);
        currentIngredient.transform.localPosition = Vector3.zero;
        currentIngredient.transform.localRotation = Quaternion.identity;
    }

    // Call this when the VR controller selects the paan leaf
    public void OnLeafSelected()
    {
        if (currentIngredient == null) return; // Nothing held on spoon

        // Transfer to leaf transform
        currentIngredient.transform.SetParent(leafSnapPoint);
        currentIngredient.transform.localPosition = Vector3.zero;
        currentIngredient.transform.localRotation = Quaternion.identity;

        // Turn collider back on
        if (currentIngredient.TryGetComponent<Collider>(out Collider col))
        {
            col.enabled = true;
        }

        currentIngredient = null;
    }
}