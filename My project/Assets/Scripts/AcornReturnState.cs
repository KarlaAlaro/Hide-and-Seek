// Tracks an acorn's flight, landing, fetch, and scoring state so it is counted only when appropriate.
using UnityEngine;

public class AcornReturnState : MonoBehaviour
{
    public bool HasBeenHitBack { get; private set; }
    public bool HasBeenCounted { get; private set; }
    public bool HasLanded { get; private set; }
    public bool IsBeingFetched { get; private set; }

    public void MarkLanded()
    {
        HasLanded = true;
    }

    public void MarkBeingFetched()
    {
        IsBeingFetched = true;
    }

    public void StopBeingFetched()
    {
        IsBeingFetched = false;
    }

    public void ResetForLaunch()
    {
        HasLanded = false;
        IsBeingFetched = false;
        HasBeenHitBack = false;
        HasBeenCounted = false;
    }

    public void MarkHitBack()
    {
        HasBeenHitBack = true;
    }

    public bool TryMarkCounted()
    {
        if (!HasBeenHitBack || HasBeenCounted || HasLanded || IsBeingFetched)
        {
            return false;
        }

        HasBeenCounted = true;
        return true;
    }
}
