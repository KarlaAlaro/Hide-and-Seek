using System.Globalization;
using System.IO;
using UnityEngine;

public class ParticipantSessionLogger : MonoBehaviour
{
    public PlayerPerformanceTracker performanceTracker;
    public string fileName = "participant_results.csv";
    public string participantId = "P001";
    private bool hasSaved;
    public string folderPath = "";


    public void SaveParticipantRow()
    {
        if (hasSaved || performanceTracker == null)
        {
            return;
        }

        hasSaved = true;

        string folder = string.IsNullOrWhiteSpace(folderPath)
            ? Application.persistentDataPath
            : folderPath;

        Directory.CreateDirectory(folder);

        string path = Path.Combine(folder, fileName);
        bool fileExists = File.Exists(path);

        using StreamWriter writer = new StreamWriter(path, true);

        if (!fileExists)
        {
            writer.WriteLine("participant_id,session_time,throws_attempted_by_bunny,Bunny_catches,misses,accuracy,acorn_hit_attempts,successful_acorn_hits,bunny_approach_attempts,bat_swings,bat_swing_misses");
        }

        float accuracy = performanceTracker.SessionCatches + performanceTracker.SessionMisses == 0
            ? 0f
            : (float)performanceTracker.SessionCatches / (performanceTracker.SessionCatches + performanceTracker.SessionMisses);

        writer.WriteLine(string.Join(",",
            participantId,
            performanceTracker.SessionTime.ToString("F2", CultureInfo.InvariantCulture),
            performanceTracker.SessionThrowsAttempted,
            performanceTracker.SessionCatches,
            performanceTracker.SessionMisses,
            accuracy.ToString("F3", CultureInfo.InvariantCulture),
            performanceTracker.SessionAcornHitAttempts,
            performanceTracker.SessionSuccessfulAcornHits,
            performanceTracker.SessionBunnyApproachAttempts,
            performanceTracker.SessionBatSwings,
            performanceTracker.SessionBatSwingMisses
        ));

        Debug.Log("Saved participant results to: " + path);
    }
}