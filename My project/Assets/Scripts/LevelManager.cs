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
    }

    public BunnyRunToGameSpot bunnyRunner;
    public ProjectileLauncher projectileLauncher;
    public BunnyCatchCounter bunnyCatchCounter;

    public LevelSettings[] levels;

    private int currentLevelIndex;

    void Start()
    {
        if (bunnyCatchCounter != null)
        {
            bunnyCatchCounter.LevelCompleted += GoToNextLevel;
        }

        ApplyCurrentLevel();
    }

    void OnDestroy()
    {
        if (bunnyCatchCounter != null)
        {
            bunnyCatchCounter.LevelCompleted -= GoToNextLevel;
        }
    }

    void ApplyCurrentLevel()
    {
        if (levels == null || levels.Length == 0)
        {
            return;
        }

        LevelSettings level = levels[currentLevelIndex];

        if (bunnyRunner != null)
        {
            bunnyRunner.bunnyCanMove = level.bunnyCanMove;
        }

        if (projectileLauncher != null)
        {
            projectileLauncher.throwsBeforeBunnyMoves = level.throwsBeforeBunnyMoves;
            projectileLauncher.acornsPerThrow = level.acornsPerThrow;
            projectileLauncher.acornSpeeds = level.acornSpeeds;
        }

        if (bunnyCatchCounter != null)
        {
            bunnyCatchCounter.ResetCounter(level.catchesNeeded);
        }

        Debug.Log("Starting level: " + level.levelName);
    }

    void GoToNextLevel()
    {
        currentLevelIndex++;

        if (currentLevelIndex >= levels.Length)
        {
            Debug.Log("All levels complete!");
            return;
        }

        ApplyCurrentLevel();
    }
}