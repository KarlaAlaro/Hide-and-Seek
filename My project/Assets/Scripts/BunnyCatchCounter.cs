using TMPro;
using UnityEngine;

public class BunnyCatchCounter : MonoBehaviour
{
    public int catchesNeeded = 7;
    public TextMeshProUGUI counterText;
    public Animator bunnyAnimator;
    public bool destroyCaughtAcorn = true;

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

        currentCatches++;
        Debug.Log("Bunny catches: " + currentCatches);

        if (bunnyAnimator != null)
        {
            bunnyAnimator.SetTrigger("Catch");
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
}
