using UnityEngine;

public class DestroyOnGround : MonoBehaviour
{
    public BunnyFetchAcorn bunnyFetcher;
    public float destroyIfNotFetchedAfter = 8f;

    private bool hasLanded;

    void OnCollisionEnter(Collision collision)
    {
        if (hasLanded)
        {
            return;
        }

        if (!collision.gameObject.CompareTag("Ground"))
        {
            return;
        }

        hasLanded = true;
        AcornReturnState acornState = GetComponent<AcornReturnState>();

        if (acornState != null)
        {
            acornState.MarkLanded();
        }
        Rigidbody rb = GetComponent<Rigidbody>();

        if (rb != null)
        {
            rb.linearVelocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
            rb.useGravity = true;
            rb.isKinematic = true;
        }

        if (bunnyFetcher != null)
        {
            bunnyFetcher.TryFetchAcorn(gameObject);
        }

        Destroy(gameObject, destroyIfNotFetchedAfter);
    }
}