using UnityEngine;
using System.Collections;

public class ProjectileLauncher : MonoBehaviour
{
    public Transform launchPoint;
    public GameObject projectile;
    public Transform player;
    public float launchSpeed = 15f;
    public float maxSpreadAngle = 35f;
    public float firstThrowDelay = 2f;
    public float throwInterval = 2.75f;
    public float throwReleaseDelay = 0.55f;
    public Animator bunnyAnimator;

    private int noOfAcorns;
    private Coroutine throwLoop;

    void Start()
    {
        throwLoop = StartCoroutine(ThrowLoop());
    }

    void OnDisable()
    {
        if (throwLoop != null)
        {
            StopCoroutine(throwLoop);
            throwLoop = null;
        }
    }

    IEnumerator ThrowLoop()
    {
        yield return new WaitForSeconds(firstThrowDelay);

        while (true)
        {
            SpawnAcorn();
            yield return new WaitForSeconds(throwReleaseDelay);
            LaunchAcorn();
            yield return new WaitForSeconds(throwInterval);
        }
    }

    void SpawnAcorn()
    {
        noOfAcorns += 1;

        if (bunnyAnimator != null)
        {
            bunnyAnimator.SetTrigger("Throw");
        }
    }

    public void LaunchAcorn()
    {   
        if (launchPoint == null || projectile == null || player == null)
        {
            return;
        }

        var _projectile = Instantiate(projectile, launchPoint.position, launchPoint.rotation);
        AcornReturnState acornState = _projectile.GetComponent<AcornReturnState>();

<<<<<<< Updated upstream
=======
        if (acornState == null)
        {
            acornState = _projectile.AddComponent<AcornReturnState>();
        }

        acornState.ResetForLaunch();

>>>>>>> Stashed changes
        // Get direction toward player
        Vector3 directionToPlayer = (player.position - launchPoint.position).normalized;
        
        // Add random angle spread
        float randomAngle = Random.Range(-maxSpreadAngle, maxSpreadAngle);
        Vector3 spreadDirection = Quaternion.Euler(0, randomAngle, 0) * directionToPlayer;
        
        // Add slight upward arc
        spreadDirection.y += 0.3f;
        spreadDirection.Normalize();
        
        Rigidbody projectileRigidbody = _projectile.GetComponent<Rigidbody>();

        if (projectileRigidbody != null)
        {
            projectileRigidbody.linearVelocity = spreadDirection * launchSpeed;
        }
        
    }
}
