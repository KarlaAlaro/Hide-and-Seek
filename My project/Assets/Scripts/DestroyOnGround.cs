// Handles acorn ground impacts, fetch eligibility, miss recording, and eventual cleanup.
using System.Collections;
using UnityEngine;

public class DestroyOnGround : MonoBehaviour
{
    private const float MinimumGroundNormalY = 0.5f;

    public BunnyFetchAcorn bunnyFetcher;
    public PlayerPerformanceTracker performanceTracker;
    public float destroyIfNotFetchedAfter = 8f;

    private bool hasLanded;
    public Transform player;
    public float disappearNearPlayerDistance = 2f;
    public float freezeNearBunnyDistance = 10f;

    void OnCollisionEnter(Collision collision)
    {
        if (hasLanded || !IsGroundContact(collision))
        {
            return;
        }

        hasLanded = true;
        RecordMissIfNeeded();

        float playerClearanceDistance = disappearNearPlayerDistance;
        if (bunnyFetcher != null)
        {
            playerClearanceDistance = Mathf.Max(playerClearanceDistance, bunnyFetcher.minDistanceFromPlayer);
        }

        if (player != null && Vector3.Distance(transform.position, player.position) < playerClearanceDistance)
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
            rb.useGravity = false;
            rb.isKinematic = true;
        }

        if (bunnyFetcher != null)
        {
            bunnyFetcher.TryFetchAcorn(gameObject);
        }

        StartCoroutine(DestroyAfterDelay());
    }

    IEnumerator DestroyAfterDelay()
    {
        yield return new WaitForSeconds(destroyIfNotFetchedAfter);

        AcornReturnState acornState = GetComponent<AcornReturnState>();
        while (acornState != null && acornState.IsBeingFetched)
        {
            yield return null;
        }

        if (gameObject != null)
        {
            Destroy(gameObject);
        }
    }

    bool IsGroundContact(Collision collision)
    {
        Transform other = collision.collider.transform;
        bool isGround = false;

        while (other != null)
        {
            if (other.CompareTag("Ground"))
            {
                isGround = true;
                break;
            }

            other = other.parent;
        }

        if (!isGround)
        {
            return false;
        }

        for (int i = 0; i < collision.contactCount; i++)
        {
            if (collision.GetContact(i).normal.y >= MinimumGroundNormalY)
            {
                return true;
            }
        }

        return false;
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
