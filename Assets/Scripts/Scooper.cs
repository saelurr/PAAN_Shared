using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Attach this to the GameObject that should "grab" other objects on collision.
/// Any collider entering this object's collision with a tag matching
/// 'grabbableTags' gets parented to this transform, snapped to local zero,
/// given an identity rotation, and made kinematic.
/// </summary>
[DisallowMultipleComponent]
public class Scooper : MonoBehaviour
{
    [Tooltip("Tags that are allowed to be grabbed on collision.")]
    [SerializeField] private List<string> grabbableTags = new List<string> { "Grabbable" };

    [Tooltip("If true, automatically grabs on trigger enter without needing to call Grab() manually.")]
    [SerializeField] private bool autoGrabOnCollision = true;

    [Tooltip("If true, only one object can be held at a time; while holding, no new grabs are accepted until Release() is called.")]
    [SerializeField] private bool singleHeldObject = true;

    [Tooltip("Seconds to wait after a Release() before another Grab() is allowed.")]
    [SerializeField] private float releaseCooldown = 0.5f;

    [SerializeField] private Vector3 grabLocalPosition = Vector3.zero;
    [SerializeField] private Vector3 grabLocalScale = Vector3.one;
    

    // Set the moment a Release happens; Grab is blocked until Time.time passes this.
    private float cooldownEndTime = -1f;

    // Tracks grabbed objects and their pre-grab state so we can restore on Release.
    private class GrabbedInfo
    {
        public Transform originalParent;
        public bool wasKinematic;
        public Vector3 worldPosition;
        public Quaternion worldRotation;
        public Vector3 originalLocalScale;
    }

    private readonly Dictionary<GameObject, GrabbedInfo> heldObjects = new Dictionary<GameObject, GrabbedInfo>();

    private void OnTriggerEnter(Collider other)
    {
        if (!autoGrabOnCollision) return;

        GameObject otherObject = other.gameObject;
        if (IsGrabbable(otherObject))
        {
            Grab(otherObject);
        }
    }

    private bool IsGrabbable(GameObject obj)
    {
        for (int i = 0; i < grabbableTags.Count; i++)
        {
            if (obj.CompareTag(grabbableTags[i]))
                return true;
        }
        return false;
    }

    /// <summary>
    /// True while a Release() cooldown is still active and Grab() would be rejected.
    /// </summary>
    public bool IsOnCooldown => Time.time < cooldownEndTime;

    /// <summary>
    /// True if this Scooper is currently holding at least one object.
    /// </summary>
    public bool IsHolding => heldObjects.Count > 0;

    /// <summary>
    /// Grabs the given GameObject: parents it to this transform, resets its
    /// local position/rotation, and makes its Rigidbody kinematic.
    /// Rejected if already holding an object (when singleHeldObject is true)
    /// or if still within the post-release cooldown window.
    /// </summary>
    public void Grab(GameObject target)
    {
        if (target == null) return;

        // Block grabbing while on cooldown after a previous release.
        if (IsOnCooldown) return;

        // Block grabbing a new object while already holding one — must Release() first.
        if (singleHeldObject && heldObjects.Count > 0) return;

        if (heldObjects.ContainsKey(target)) return; // already grabbed

        Rigidbody rb = target.GetComponent<Rigidbody>();

        // Capture the object's current world (lossy) scale before we touch its parent,
        // so we can re-derive the correct localScale under the new parent.
        Vector3 originalWorldScale = target.transform.lossyScale;

        GrabbedInfo info = new GrabbedInfo
        {
            originalParent = target.transform.parent,
            wasKinematic = rb != null && rb.isKinematic,
            worldPosition = target.transform.position,
            worldRotation = target.transform.rotation,
            originalLocalScale = target.transform.localScale
        };

        heldObjects[target] = info;

        target.transform.SetParent(transform, worldPositionStays: false);
        target.transform.localPosition = grabLocalPosition;
        target.transform.localScale = grabLocalScale;
        if (rb != null)
        {
            rb.linearVelocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
            rb.isKinematic = true;
        }
    }

    /// <summary>
    /// Releases the given GameObject: restores its original parent and Rigidbody
    /// kinematic state. If no target is specified, releases whatever is currently held.
    /// Starts the post-release cooldown, during which Grab() is rejected.
    /// </summary>
    public void Release(GameObject target = null)
    {
        if (target == null)
        {
            if (heldObjects.Count == 0) return;

            // Release the first (or only) held object.
            var enumerator = heldObjects.GetEnumerator();
            enumerator.MoveNext();
            target = enumerator.Current.Key;
        }

        if (!heldObjects.TryGetValue(target, out GrabbedInfo info)) return;

        target.transform.SetParent(info.originalParent, worldPositionStays: true);
        target.transform.localScale = info.originalLocalScale;

        Rigidbody rb = target.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.isKinematic = false; //info.wasKinematic;
        }
        SnappableIngredient snappable = target.GetComponent<SnappableIngredient>();
        if (snappable != null)
        {
            snappable.TrySnap();
        }

 
        heldObjects.Remove(target);

        // Start the cooldown window; Grab() is blocked until it elapses.
        cooldownEndTime = Time.time + releaseCooldown;
    }

    /// <summary>
    /// Releases everything currently held.
    /// </summary>
    public void ReleaseAll()
    {
        var currentlyHeld = new List<GameObject>(heldObjects.Keys);
        foreach (var obj in currentlyHeld)
        {
            Release(obj);
        }
    }
}