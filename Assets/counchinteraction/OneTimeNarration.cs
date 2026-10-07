using UnityEngine;

public class OneTimeNarration : MonoBehaviour
{
    public AudioSource audioSource;
    public AudioClip narrationClip;
    public string playerTag = "Player";

    private bool hasPlayed = false;

    void OnTriggerEnter(Collider other)
    {
        if (hasPlayed) return;

        if (other.CompareTag(playerTag))
        {
            audioSource.clip = narrationClip;
            audioSource.Play();
            hasPlayed = true;
        }
    }
}