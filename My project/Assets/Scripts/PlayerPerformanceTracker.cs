// Tracks level and session statistics for throws, catches, misses, swings, and accuracy.
using UnityEngine;

public class PlayerPerformanceTracker : MonoBehaviour
{
    public int ThrowsAttempted { get; private set; }
    public int Catches { get; private set; }
    public int Misses { get; private set; }
    public int CurrentCatchStreak { get; private set; }
    public int BestCatchStreak { get; private set; }
    public float LevelTime => Time.time - levelStartTime;
    public int ResolvedAcorns => Catches + Misses;
    public float Accuracy => ResolvedAcorns == 0 ? 0f : (float)Catches / ResolvedAcorns;

    //tracking session information
    public int SessionBatSwings { get; private set; } //how many times player swung bat
    public int SessionBatSwingMisses => SessionBatSwings - SessionAcornHitAttempts; //player swung bat but didn't hit the acorn
    public int SessionThrowsAttempted { get; private set; }
    public int SessionCatches { get; private set; }
    public int SessionMisses { get; private set; }
    public int SessionAcornHitAttempts { get; private set; } //acorn not hit with enough forcw
    public int SessionSuccessfulAcornHits { get; private set; } //acorn hit with enough force to send it back
    public int SessionBunnyApproachAttempts { get; private set; }

    private float sessionStartTime;
    public float SessionTime => Time.time - sessionStartTime;

    public System.Action Changed;

    private float levelStartTime;

    void Awake()
    {
        ResetLevel();
    }

    public void ResetLevel()
    {
        ThrowsAttempted = 0;
        Catches = 0;
        Misses = 0;
        CurrentCatchStreak = 0;
        BestCatchStreak = 0;
        levelStartTime = Time.time;
        Changed?.Invoke();
    }
    public void StartParticipantSession()
    {
        SessionThrowsAttempted = 0;
        SessionCatches = 0;
        SessionMisses = 0;
        SessionBatSwings = 0;
        SessionAcornHitAttempts = 0;
        SessionSuccessfulAcornHits = 0;
        SessionBunnyApproachAttempts = 0;
        sessionStartTime = Time.time;

        ResetLevel();
    }
    //number of throws made by bunny
    public void RecordThrow(int acornCount)
    {
        int count = Mathf.Max(1, acornCount);
        ThrowsAttempted++;

        SessionThrowsAttempted++;
        Changed?.Invoke();
    }

//acorn caught by the bunny
    public void RecordCatch()
    {
        Catches++;
        CurrentCatchStreak++;
        SessionAcornHitAttempts++;
        SessionCatches++;

        BestCatchStreak = Mathf.Max(BestCatchStreak, CurrentCatchStreak);
        Changed?.Invoke();
    }

//acorn hit back but not caught by the bunnt
    public void RecordMiss()
    {
        Misses++;
        SessionMisses++;
        CurrentCatchStreak = 0;
        Changed?.Invoke();
    }
    public void RecordAcornHitAttempt() //the bat is not swung with enough speed to hit back the acorn
    {
        SessionAcornHitAttempts++;
        Changed?.Invoke();
    }

    public void RecordSuccessfulAcornHit() //bat swung with enough speed to return the acorn
    {
        SessionSuccessfulAcornHits++;
        Changed?.Invoke();
    }

    public void RecordBunnyApproachAttempt()
    {
        SessionBunnyApproachAttempts++;
        Changed?.Invoke();
    }
    public void RecordBatSwing()
    {
        SessionBatSwings++;
        Changed?.Invoke();
    }
}
