using TMPro;
using UnityEngine;
using System.Collections;

public class BunnyCatchCounter : MonoBehaviour
{
    public int catchesNeeded = 7;
    public TextMeshProUGUI counterText;
    
    public Animator bunnyAnimator;
    public bool destroyCaughtAcorn = true;
    public string catchAnimationStateName = "Base Layer.Catch";
    public float catchAnimationFadeTime = 0.05f;
    public bool forceCatchAnimation = true;
    public AudioSource audioSource;
    public AudioClip catchSound;
    public bool IsCelebrating { get; private set; }
    public float celebrationTime = 1.2f;

    private int currentCatches;

    void Start()
    {
        UpdateUI();
    }

    void OnTriggerEnter(Collider other)
    {
        StartCoroutine(Celebrate());
        if (!other.CompareTag("Acorn"))
        {
            return;
        }

        AcornReturnState acornState = other.GetComponentInParent<AcornReturnState>();
        if (acornState == null || !acornState.TryMarkCounted())
        {
            return;
        }

        currentCatches++;
        Debug.Log("Bunny catches: " + currentCatches);

        PlayCatchReaction();

        if (audioSource != null && catchSound != null)
        {
            audioSource.PlayOneShot(catchSound);
        }
        UpdateUI();

        if (destroyCaughtAcorn)
        {
            Destroy(acornState.gameObject);
        }
    }

    void UpdateUI()
    {
        if (counterText == null)
        {
            return;
        }

        counterText.text = currentCatches >= catchesNeeded
            ? "Complete!"
            : currentCatches + "/" + catchesNeeded;
    }

    void PlayCatchReaction()
    {
        if (bunnyAnimator == null)
        {
            return;
        }

        bunnyAnimator.ResetTrigger("Throw");
        bunnyAnimator.ResetTrigger("Catch");

        if (forceCatchAnimation && !string.IsNullOrEmpty(catchAnimationStateName))
        {
            bunnyAnimator.CrossFadeInFixedTime(catchAnimationStateName, catchAnimationFadeTime, 0, 0f);
        }

        bunnyAnimator.SetTrigger("Catch");
    }
    IEnumerator Celebrate()
    {
        IsCelebrating = true;

        yield return new WaitForSeconds(celebrationTime);

        IsCelebrating = false;
    }
}
