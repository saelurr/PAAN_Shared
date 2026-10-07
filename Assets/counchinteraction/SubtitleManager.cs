using UnityEngine;
using TMPro;
using System.Collections;

public class SubtitleManager : MonoBehaviour
{
    public static SubtitleManager Instance;

    [Tooltip("Drag your TextMeshPro subtitle text object here.")]
    public TextMeshProUGUI subtitleText;

    private Coroutine currentSubtitle;

    private void Awake()
    {
        Instance = this;
        subtitleText.text = "";
    }

    public void ShowSubtitle(string text, float duration)
    {
        if (currentSubtitle != null)
            StopCoroutine(currentSubtitle);

        currentSubtitle = StartCoroutine(SubtitleRoutine(text, duration));
    }

    private IEnumerator SubtitleRoutine(string text, float duration)
    {
        subtitleText.text = text;
        yield return new WaitForSeconds(duration);
        subtitleText.text = "";
    }
}