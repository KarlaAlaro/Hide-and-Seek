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


    void Start()
    {
        timeRemaining = experienceLength;
        timerRunning = true;
        Debug.Log(timerRunning);
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
        endingCelebration.StartCelebration();
    }
}