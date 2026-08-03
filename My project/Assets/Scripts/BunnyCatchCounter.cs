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
    public PlayerPerformanceTracker performanceTracker;
    public System.Action LevelCompleted;
    private bool hasCompleted;
    public bool IsCelebrating { get; private set; }
    public float celebrationTime = 1.2f;

    private int currentCatches;

    void Start()
    {
        UpdateUI();
    }

    void OnTriggerEnter(Collider other)
    {
        
        if (!other.CompareTag("Acorn"))
        {
            return;
        }
        
        AcornReturnState acornState = other.GetComponentInParent<AcornReturnState>();
        if (acornState == null || !acornState.TryMarkCounted())
        {
            return;
        }
        StartCoroutine(Celebrate());
        currentCatches++;
        if (performanceTracker != null)
        {
            performanceTracker.RecordCatch();
        }

        Debug.Log("Bunny catches: " + currentCatches);

        PlayCatchReaction();

        if (audioSource != null && catchSound != null)
        {
            audioSource.Stop();
            audioSource.clip = catchSound;
            audioSource.Play();
        }
        UpdateUI();

        if (destroyCaughtAcorn)
        {
            Destroy(acornState.gameObject);
        }
        if (!hasCompleted && currentCatches >= catchesNeeded)
        {
            hasCompleted = true;
            LevelCompleted?.Invoke();
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
        bunnyAnimator.SetBool("Wave", false);
        bunnyAnimator.ResetTrigger("Throw");

        bunnyAnimator.Play("Catch");
    }
    IEnumerator Celebrate()
    {
        IsCelebrating = true;

        yield return new WaitForSeconds(celebrationTime);

        IsCelebrating = false;
    }
    public void ResetCounter(int newCatchesNeeded)
    {
        catchesNeeded = newCatchesNeeded;
        currentCatches = 0;
        hasCompleted = false;
        UpdateUI();
    }
}
