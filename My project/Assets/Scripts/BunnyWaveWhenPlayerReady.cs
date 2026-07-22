using UnityEngine;

public class BunnyWaveWhenPlayerReady : MonoBehaviour
{
    public Animator bunnyAnimator;
    public AudioSource bunnyAudioSource;
    public AudioClip waveSound;
    private bool hasWaved;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void OnTriggerEnter(Collider other)
    {
        if (hasWaved)
        {
            return;
        }

        if (!other.CompareTag("Player"))
        {
            return;
        }

        hasWaved = true;

        bunnyAnimator.SetBool("Wave", false);

        /*if (bunnyAudioSource != null && waveSound != null)
        {
            bunnyAudioSource.PlayOneShot(waveSound);
        }*/
    }
}
