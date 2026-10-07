using UnityEngine;

public class IngredientDetector : MonoBehaviour
{
    public NarrationManager narrationManager;

    void OnTriggerEnter(Collider other)
    {
        IngredientInfo info = other.GetComponent<IngredientInfo>();
        if (info != null)
        {
            narrationManager.PlayIngredient(info);
        }
    }
}