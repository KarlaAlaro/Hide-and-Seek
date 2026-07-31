using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class StartMinigameOnBatPickup : MonoBehaviour
{
    public ProjectileLauncher projectileLauncher;

    private bool hasStarted;

    public void StartMinigame()
    {
        if (hasStarted || projectileLauncher == null)
        {
            return;
        }

        hasStarted = true;
        projectileLauncher.BeginThrowing();
    }
}
