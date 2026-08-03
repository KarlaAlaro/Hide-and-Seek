using UnityEngine;

public class PlayerPerformanceTracker : MonoBehaviour
{
    public int ThrowsAttempted { get; private set; }
    public int AcornsLaunched { get; private set; }
    public int Catches { get; private set; }
    public int Misses { get; private set; }
    public int CurrentCatchStreak { get; private set; }
    public int BestCatchStreak { get; private set; }
    public float LevelTime => Time.time - levelStartTime;
    public int ResolvedAcorns => Catches + Misses;
    public float Accuracy => ResolvedAcorns == 0 ? 0f : (float)Catches / ResolvedAcorns;

    public System.Action Changed;

    private float levelStartTime;

    void Awake()
    {
        ResetLevel();
    }

    public void ResetLevel()
    {
        ThrowsAttempted = 0;
        AcornsLaunched = 0;
        Catches = 0;
        Misses = 0;
        CurrentCatchStreak = 0;
        BestCatchStreak = 0;
        levelStartTime = Time.time;
        Changed?.Invoke();
    }

    public void RecordThrow(int acornCount)
    {
        ThrowsAttempted++;
        AcornsLaunched += Mathf.Max(1, acornCount);
        Changed?.Invoke();
    }

    public void RecordCatch()
    {
        Catches++;
        CurrentCatchStreak++;
        BestCatchStreak = Mathf.Max(BestCatchStreak, CurrentCatchStreak);
        Changed?.Invoke();
    }

    public void RecordMiss()
    {
        Misses++;
        CurrentCatchStreak = 0;
        Changed?.Invoke();
    }
}
