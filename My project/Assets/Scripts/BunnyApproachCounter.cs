// Records rate-limited attempts when the player enters the bunny's approach trigger.
using UnityEngine;

public class BunnyApproachCounter : MonoBehaviour
{
    public PlayerPerformanceTracker performanceTracker;
    public float cooldown = 2f;

    private float nextAllowedTime;

    void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player"))
        {
            return;
        }

        if (Time.time < nextAllowedTime)
        {
            return;
        }

        nextAllowedTime = Time.time + cooldown;

        if (performanceTracker != null)
        {
            performanceTracker.RecordBunnyApproachAttempt();
        }
    }
}
