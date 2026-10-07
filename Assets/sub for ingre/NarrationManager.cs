using UnityEngine;
using TMPro;

public class NarrationManager : MonoBehaviour
{
    public AudioSource audioSource;
    public GameObject subtitleCanvas;
    public TextMeshProUGUI subtitleText;
    public Transform followTarget;

    string currentIngredient = "";
    SubtitleLine[] currentLines;
    int lineIndex;
    float lineTimer;

    void Update()
    {
        if (subtitleCanvas.activeSelf && followTarget != null)
        {
            subtitleCanvas.transform.position = followTarget.position + Vector3.up * 0.1f;
            subtitleCanvas.transform.rotation = Quaternion.LookRotation(
                subtitleCanvas.transform.position - Camera.main.transform.position);

            if (!audioSource.isPlaying)
            {
                subtitleCanvas.SetActive(false);
                currentIngredient = "";
            }
            else if (currentLines != null && currentLines.Length > 0)
            {
                lineTimer += Time.deltaTime;
                if (lineTimer >= currentLines[lineIndex].duration && lineIndex < currentLines.Length - 1)
                {
                    lineIndex++;
                    lineTimer = 0f;
                    subtitleText.text = currentLines[lineIndex].text;
                }
            }
        }
    }

    public void PlayIngredient(IngredientInfo info)
    {
        if (currentIngredient == info.ingredientName && audioSource.isPlaying) return;

        currentIngredient = info.ingredientName;
        audioSource.clip = info.narrationClip;
        audioSource.Play();

        currentLines = info.subtitleLines;
        lineIndex = 0;
        lineTimer = 0f;

        subtitleText.text = (currentLines != null && currentLines.Length > 0) ? currentLines[0].text : "";
        subtitleCanvas.SetActive(true);
    }
}