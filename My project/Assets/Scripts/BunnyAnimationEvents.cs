// Relays bunny throw animation events to the projectile launcher.
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
