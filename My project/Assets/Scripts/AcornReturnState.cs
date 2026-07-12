using UnityEngine;

public class AcornReturnState : MonoBehaviour
{
    public bool HasBeenHitBack { get; private set; }
    public bool HasBeenCounted { get; private set; }

    public void ResetForLaunch()
    {
        HasBeenHitBack = false;
        HasBeenCounted = false;
    }

    public void MarkHitBack()
    {
        HasBeenHitBack = true;
    }

    public bool TryMarkCounted()
    {
        if (!HasBeenHitBack || HasBeenCounted)
        {
            return false;
        }

        HasBeenCounted = true;
        return true;
    }
}
