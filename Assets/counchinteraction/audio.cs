using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class TriggerPlayAudio : MonoBehaviour
{
    [Tooltip("Optional: assign a specific clip here. If left empty, it just plays whatever clip is already set on the AudioSource.")]
    public AudioClip clipToPlay;

    [Tooltip("Only objects with this tag will trigger the sound. Leave as 'Untagged' to trigger on ANYTHING.")]
    public string requiredTag = "Untagged";

    [Tooltip("If true, sound can only play once. If false, it replays every time something passes through.")]
    public bool playOnce = false;

    private AudioSource audioSource;
    private bool hasPlayed = false;

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
    }

    private void OnTriggerEnter(Collider other)
    {
        // If a specific tag is required, check it (skip check if left as "Untagged")
        if (requiredTag != "Untagged" && !other.CompareTag(requiredTag))
            return;

        // If set to play only once, stop if it already played
        if (playOnce && hasPlayed)
            return;

        if (clipToPlay != null)
        {
            audioSource.clip = clipToPlay;
        }

        audioSource.Play();
        hasPlayed = true;
    }
}