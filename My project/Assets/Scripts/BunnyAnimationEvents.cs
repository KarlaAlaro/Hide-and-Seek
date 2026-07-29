using UnityEngine;

public class BunnyAnimationEvents : MonoBehaviour
{
    public ProjectileLauncher projectileLauncher;

    public void ReleaseThrownAcorns()
    {
        if (projectileLauncher != null)
        {
            projectileLauncher.ReleaseThrownAcorns();
        }
    }
}