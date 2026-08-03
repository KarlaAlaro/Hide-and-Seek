using UnityEngine;

public class DestroyOnGround : MonoBehaviour
{
    public BunnyFetchAcorn bunnyFetcher;
    public PlayerPerformanceTracker performanceTracker;
    public float destroyIfNotFetchedAfter = 8f;

    private bool hasLanded;
    public Transform player;
    public float disappearNearPlayerDistance = 2f;
    public float freezeNearBunnyDistance = 10f;

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
        RecordMissIfNeeded();

        if (player != null && Vector3.Distance(transform.position, player.position) < disappearNearPlayerDistance)
        {
            Destroy(gameObject);
            return;
        }

        if (bunnyFetcher != null && Vector3.Distance(transform.position, bunnyFetcher.transform.position) > freezeNearBunnyDistance)
        {
            Destroy(gameObject);
            return;
        }

       
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

    void RecordMissIfNeeded()
    {
        if (performanceTracker == null)
        {
            return;
        }

        AcornReturnState acornState = GetComponent<AcornReturnState>();

        if (acornState != null && acornState.HasBeenCounted)
        {
            return;
        }

        performanceTracker.RecordMiss();
    }
}
