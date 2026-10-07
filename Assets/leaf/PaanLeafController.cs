using UnityEngine;

/// <summary>
/// Drives the paan leaf Animator, letting each part (left, right, tip)
/// play its baked fold animation forward (close) or backward (open)
/// independently, without needing separate reversed clips.
///
/// Setup required in the Animator (per state you want reversible):
///   1. Add a Float parameter, e.g. "LeftSpeed" (default 1).
///   2. Select the animation state (e.g. "Armature_001|1fold_left").
///   3. Under Speed in the Inspector, tick "Multiplier" and assign that Float.
///
/// Then hook up a UI Button's OnClick() to call e.g. OpenLeft() / CloseLeft().
/// </summary>
public class PaanLeafController : MonoBehaviour
{
    [Header("Animator")]
    public Animator animator;

    [Header("State names (must match the Animator state names exactly)")]
    public string leftStateName = "Armature_001|1fold_left";
    public string rightStateName = "Armature_001|1fold_right";
    public string tipStateName = "Armature_001|1fold_tip";

    [Header("Speed parameter names (must match Animator float params)")]
    public string leftSpeedParam = "LeftSpeed";
    public string rightSpeedParam = "RightSpeed";
    public string tipSpeedParam = "TipSpeed";

    // ---------- Public button hooks ----------

    public void OpenLeft()  => PlayReverse(leftStateName, leftSpeedParam);
    public void CloseLeft() => PlayForward(leftStateName, leftSpeedParam);

    public void OpenRight()  => PlayReverse(rightStateName, rightSpeedParam);
    public void CloseRight() => PlayForward(rightStateName, rightSpeedParam);

    public void OpenTip()  => PlayReverse(tipStateName, tipSpeedParam);
    public void CloseTip() => PlayForward(tipStateName, tipSpeedParam);

    // Optional: open everything at once, e.g. for a "reset/open all" button
    public void OpenAll()
    {
        OpenLeft();
        OpenRight();
        OpenTip();
    }

    // ---------- Core logic ----------

    void PlayForward(string stateName, string speedParam)
    {
        animator.SetFloat(speedParam, 1f);
        // Start from the beginning of the clip (closed -> ... forward)
        animator.Play(stateName, 0, 0f);
    }

    void PlayReverse(string stateName, string speedParam)
    {
        animator.SetFloat(speedParam, -1f);
        // Start from the end of the clip and let negative speed run it backward
        animator.Play(stateName, 0, 1f);
    }
}
