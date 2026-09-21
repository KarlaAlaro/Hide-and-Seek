// Ends the timed experience, stops throwing, saves the participant session, and starts the finale.
using TMPro;
using UnityEngine;

public class ExperienceTimer : MonoBehaviour
{
    public float experienceLength = 40f;
   public BunnyCelebration endingCelebration;

    private float timeRemaining;
    private bool timerRunning;
    private bool hasEnded;
    public ProjectileLauncher projectileLauncher;
    public ParticipantSessionLogger sessionLogger;


    void Start()
    {
        timeRemaining = experienceLength;
        timerRunning = true;
        Debug.Log(timerRunning);
        if (sessionLogger != null && sessionLogger.performanceTracker != null)
    {
        sessionLogger.performanceTracker.StartParticipantSession();
    }
    }

    void Update()
    {
        if (!timerRunning || hasEnded)
        {
            return;
        }

        timeRemaining -= Time.deltaTime;

        if (timeRemaining <= 0f)
        {
            timeRemaining = 0f;
            EndExperience();
        }
        Debug.Log("Time remaining " + timeRemaining);
    }


    void EndExperience()
    {
        hasEnded = true;
        timerRunning = false;
        Debug.Log("Time is up");
        projectileLauncher.enabled = false;
        if (sessionLogger != null)
        {
            sessionLogger.SaveParticipantRow();
        }
        endingCelebration.StartCelebration();
    }
}
