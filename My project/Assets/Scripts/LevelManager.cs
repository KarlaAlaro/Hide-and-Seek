using UnityEngine;

public class LevelManager : MonoBehaviour
{
    [System.Serializable]
    public class LevelSettings
    {
        public string levelName;
        public bool bunnyCanMove = true;
        public int throwsBeforeBunnyMoves = 3;
        public int acornsPerThrow = 1;
        public float[] acornSpeeds;
        public int catchesNeeded = 7;
        public float launchSpeed = 15f;
        public float throwInterval = 2.75f;
        public float maxSpreadAngle = 15f;
    }

    public BunnyRunToGameSpot bunnyRunner;
    public ProjectileLauncher projectileLauncher;
    public BunnyCatchCounter bunnyCatchCounter;
    public PlayerPerformanceTracker performanceTracker;

    public LevelSettings[] levels;
    public bool adaptDifficultyToPerformance = true;
    public int resolvedAcornsBeforeAdjustment = 4;
    public float strugglingAccuracy = 0.45f;
    public float thrivingAccuracy = 0.75f;
    public int catchStreakForHarderMode = 3;
    public int maxAdaptiveSteps = 2;

    private int currentLevelIndex;
    private int adaptiveStep;
    private int lastResolvedAcornCheck;

    void Start()
    {
        if (bunnyCatchCounter != null)
        {
            bunnyCatchCounter.LevelCompleted += GoToNextLevel;
        }

        if (performanceTracker != null)
        {
            performanceTracker.Changed += UpdateAdaptiveDifficulty;
            ConnectPerformanceTracker();
        }

        ApplyCurrentLevel(true);
    }

    void OnDestroy()
    {
        if (bunnyCatchCounter != null)
        {
            bunnyCatchCounter.LevelCompleted -= GoToNextLevel;
        }

        if (performanceTracker != null)
        {
            performanceTracker.Changed -= UpdateAdaptiveDifficulty;
        }
    }

    void ApplyCurrentLevel(bool resetProgress)
    {
        if (levels == null || levels.Length == 0)
        {
            return;
        }

        LevelSettings level = levels[currentLevelIndex];
        int easierSteps = Mathf.Max(0, -adaptiveStep);
        int harderSteps = Mathf.Max(0, adaptiveStep);

        if (bunnyRunner != null)
        {
            bunnyRunner.bunnyCanMove = level.bunnyCanMove && adaptiveStep > -maxAdaptiveSteps;
        }

        if (projectileLauncher != null)
        {
            float speedMultiplier = 1f + adaptiveStep * 0.15f;
            projectileLauncher.throwsBeforeBunnyMoves = Mathf.Max(1, level.throwsBeforeBunnyMoves + easierSteps - harderSteps);
            projectileLauncher.acornsPerThrow = Mathf.Max(1, level.acornsPerThrow + (harderSteps >= 2 ? 1 : 0));
            projectileLauncher.launchSpeed = Mathf.Max(1f, GetBaseLaunchSpeed(level) * speedMultiplier);
            projectileLauncher.acornSpeeds = GetAdjustedSpeeds(level.acornSpeeds, speedMultiplier);
            projectileLauncher.throwInterval = Mathf.Max(1.25f, GetBaseThrowInterval(level) - adaptiveStep * 0.3f);
            projectileLauncher.maxSpreadAngle = Mathf.Max(5f, GetBaseSpreadAngle(level) + adaptiveStep * 5f);

            if (resetProgress)
            {
                projectileLauncher.ResetMovementThrowCounter();
            }
        }

        if (bunnyCatchCounter != null && resetProgress)
        {
            bunnyCatchCounter.ResetCounter(level.catchesNeeded);
        }

        if (performanceTracker != null && resetProgress)
        {
            lastResolvedAcornCheck = 0;
            performanceTracker.ResetLevel();
        }

        Debug.Log("Starting level: " + level.levelName + " adaptive step: " + adaptiveStep);
    }

    void ConnectPerformanceTracker()
    {
        if (projectileLauncher != null && projectileLauncher.performanceTracker == null)
        {
            projectileLauncher.performanceTracker = performanceTracker;
        }

        if (bunnyCatchCounter != null && bunnyCatchCounter.performanceTracker == null)
        {
            bunnyCatchCounter.performanceTracker = performanceTracker;
        }
    }

    void GoToNextLevel()
    {
        adaptiveStep = GetNextLevelAdaptiveStep();
        currentLevelIndex++;

        if (currentLevelIndex >= levels.Length)
        {
            Debug.Log("All levels complete!");
            return;
        }

        ApplyCurrentLevel(true);
    }

    void UpdateAdaptiveDifficulty()
    {
        if (!adaptDifficultyToPerformance || performanceTracker == null)
        {
            return;
        }

        int resolvedAcorns = performanceTracker.ResolvedAcorns;

        if (resolvedAcorns - lastResolvedAcornCheck < resolvedAcornsBeforeAdjustment)
        {
            return;
        }

        lastResolvedAcornCheck = resolvedAcorns;
        int nextAdaptiveStep = adaptiveStep;

        if (performanceTracker.Accuracy <= strugglingAccuracy)
        {
            nextAdaptiveStep--;
        }
        else if (performanceTracker.Accuracy >= thrivingAccuracy
            && performanceTracker.CurrentCatchStreak >= catchStreakForHarderMode)
        {
            nextAdaptiveStep++;
        }

        nextAdaptiveStep = Mathf.Clamp(nextAdaptiveStep, -maxAdaptiveSteps, maxAdaptiveSteps);

        if (nextAdaptiveStep == adaptiveStep)
        {
            return;
        }

        adaptiveStep = nextAdaptiveStep;
        ApplyCurrentLevel(false);
    }

    int GetNextLevelAdaptiveStep()
    {
        if (!adaptDifficultyToPerformance || performanceTracker == null || performanceTracker.ResolvedAcorns == 0)
        {
            return 0;
        }

        if (performanceTracker.Accuracy <= strugglingAccuracy)
        {
            return -1;
        }

        if (performanceTracker.Accuracy >= thrivingAccuracy || performanceTracker.BestCatchStreak >= catchStreakForHarderMode)
        {
            return 1;
        }

        return 0;
    }

    float GetBaseLaunchSpeed(LevelSettings level)
    {
        return level.launchSpeed > 0f ? level.launchSpeed : 15f;
    }

    float GetBaseThrowInterval(LevelSettings level)
    {
        return level.throwInterval > 0f ? level.throwInterval : 2.75f;
    }

    float GetBaseSpreadAngle(LevelSettings level)
    {
        return level.maxSpreadAngle > 0f ? level.maxSpreadAngle : 35f;
    }

    float[] GetAdjustedSpeeds(float[] baseSpeeds, float multiplier)
    {
        if (baseSpeeds == null || baseSpeeds.Length == 0)
        {
            return baseSpeeds;
        }

        float[] adjustedSpeeds = new float[baseSpeeds.Length];

        for (int i = 0; i < baseSpeeds.Length; i++)
        {
            adjustedSpeeds[i] = Mathf.Max(1f, baseSpeeds[i] * multiplier);
        }

        return adjustedSpeeds;
    }
}
