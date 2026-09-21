// Checks player entry into the start trigger and gates its start handling on bunny arrival.
using UnityEngine;

public class StartMinigameOnPlayerEnter : MonoBehaviour
{
    public ProjectileLauncher projectileLauncher;
    public BunnyRunToGameSpot bunnyRunner;
    public bool waitForBunnyArrival = true;

    private bool hasStarted;

    void Awake()
    {
        if (projectileLauncher != null)
        {
            //projectileLauncher.enabled = false;
        }
    }
    void OnTriggerEnter(Collider other)
    {
        TryStart(other);
    }

    void OnTriggerStay(Collider other)
    {
        TryStart(other);
    }


    void TryStart(Collider other)
    {
        if (hasStarted)
        {
            return;
        }

        if (!other.CompareTag("Player"))
        {
            return;
        }

        if (waitForBunnyArrival && bunnyRunner != null && !bunnyRunner.HasArrived)
        {
            return;
        }

        hasStarted = true;

        if (projectileLauncher != null)
        {
            //projectileLauncher.enabled = true;
        }
    }
}
