using UnityEngine;

public class SnappableIngredient : MonoBehaviour
{
    public string ingredientName;
    public float snapRange = 0.3f;
    public bool isPlaced = false;

    Rigidbody rb;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    public void TrySnap()
    {
        SnapSlot[] allSlots = FindObjectsOfType<SnapSlot>();
        SnapSlot bestSlot = null;
        float closestDist = Mathf.Infinity;

        foreach (SnapSlot slot in allSlots)
        {
            if (slot.isOccupied) continue;
            if (!string.IsNullOrEmpty(slot.acceptedIngredientName) &&
                slot.acceptedIngredientName != ingredientName) continue;

            float dist = Vector3.Distance(transform.position, slot.transform.position);
            if (dist <= snapRange && dist < closestDist)
            {
                closestDist = dist;
                bestSlot = slot;
            }
        }

        if (bestSlot != null)
        {
            transform.position = bestSlot.transform.position;
            transform.rotation = bestSlot.transform.rotation;

            if (rb != null)
            {
                rb.linearVelocity = Vector3.zero;
                rb.angularVelocity = Vector3.zero;
                rb.isKinematic = true;
            }

            bestSlot.isOccupied = true;
            isPlaced = true;
        }
    }
}