using UnityEngine;

public class IngredientInfo : MonoBehaviour
{
    public string ingredientName;
    public AudioClip narrationClip;
    public SubtitleLine[] subtitleLines;
}

[System.Serializable]
public class SubtitleLine
{
    [TextArea] public string text;
    public float duration; // how many seconds this line stays on screen
}