using UnityEngine;
using System.Collections;

public class ProjectileLauncher : MonoBehaviour
{
    public Transform launchPoint;
    public BunnyRunToGameSpot bunnyRunner;
    public BunnyCatchCounter bunnyCatchCounter;
    public int throwsBeforeBunnyMoves = 3;
    private int throwsSinceLastMove;
    private bool throwReleased;
    private bool throwCanceled;

    public GameObject projectile;
    public Transform player;
    public float launchSpeed = 15f;
    public float maxSpreadAngle = 35f;
    public float firstThrowDelay = 2f;
    public float throwInterval = 2.75f;
    public float throwReleaseDelay = 0.55f;
    public int acornsPerThrow = 1;
    public float throwEventTimeout = 1.5f;
    public float[] acornSpeeds;
    public Animator bunnyAnimator;
    public BunnyFetchAcorn bunnyFetcher;

    private int noOfAcorns;
    private Coroutine throwLoop;

    void OnDisable()
    {
        if (throwLoop != null)
        {
            StopCoroutine(throwLoop);
            throwLoop = null;
        }
    }
    void OnEnable()
    {
        BeginThrowing();
    }

    public void BeginThrowing()
    {
        if (!enabled)
        {
            enabled = true;
            return;
        }

        if (throwLoop == null)
        {
            throwLoop = StartCoroutine(ThrowLoop());
        }
    }
    IEnumerator ThrowLoop()
    {
        yield return new WaitForSeconds(firstThrowDelay);

        while (true)
        {
            while (bunnyCatchCounter != null && bunnyCatchCounter.IsCelebrating)
            {
                yield return null;
            }

            while (BunnyIsFetching())
            {
                yield return null;
            }

            throwReleased = false;
            throwCanceled = false;
            SpawnAcorn();

            float waitTime = 0f;

            while (!throwReleased && waitTime < throwEventTimeout)
            {
                waitTime += Time.deltaTime;
                yield return null;
            }

            if (!throwReleased)
            {
                Debug.LogWarning("Throw animation event did not fire. Launching acorn using timeout fallback.");
                ReleaseThrownAcorns();
            }

            if (throwCanceled)
            {
                yield return null;
                continue;
            }

            throwsSinceLastMove++;

            if (throwsSinceLastMove >= throwsBeforeBunnyMoves)
            {
                throwsSinceLastMove = 0;

                if (bunnyRunner != null)
                {
                    bunnyRunner.RunToRandomGameSpot();

                    while (!bunnyRunner.HasArrived)
                    {
                        yield return null;
                    }
                }
            }
                yield return new WaitForSeconds(throwInterval);
            }
    }

    void SpawnAcorn()
    {
        noOfAcorns += 1;

        if (bunnyAnimator != null)
        {
            bunnyAnimator.SetBool("Wave", false);
            bunnyAnimator.SetBool("Running", false);
            bunnyAnimator.SetTrigger("Throw");
        }
    }

     public void LaunchAcorn(float speed, int acornIndex, int totalAcorns)
    {
        if (launchPoint == null || projectile == null || player == null)
        {
            return;
        }

        var spawnedProjectile = Instantiate(projectile, launchPoint.position, launchPoint.rotation);
        DestroyOnGround destroyOnGround = spawnedProjectile.GetComponent<DestroyOnGround>();

        if (destroyOnGround != null)
        {
            destroyOnGround.bunnyFetcher = bunnyFetcher;
        }

        AcornReturnState acornState = spawnedProjectile.GetComponent<AcornReturnState>();

        if (acornState == null)
        {
            acornState = spawnedProjectile.AddComponent<AcornReturnState>();
        }

        acornState.ResetForLaunch();

        Vector3 directionToPlayer = (player.position - launchPoint.position).normalized;

        float randomAngle = Random.Range(-maxSpreadAngle, maxSpreadAngle);

        float formationAngle = 0f;
        if (totalAcorns > 1)
        {
            float spacing = maxSpreadAngle / Mathf.Max(1, totalAcorns - 1);
            formationAngle = -maxSpreadAngle * 0.5f + spacing * acornIndex;
        }

        Vector3 spreadDirection = Quaternion.Euler(0f, randomAngle + formationAngle, 0f) * directionToPlayer;

        spreadDirection.y += 0.35f;
        spreadDirection.Normalize();

        Rigidbody projectileRigidbody = spawnedProjectile.GetComponent<Rigidbody>();

        if (projectileRigidbody != null)
        {
            projectileRigidbody.linearVelocity = spreadDirection * speed;
        }
    }
    public void LaunchAcorns()
    {
        int count = Mathf.Max(1, acornsPerThrow);

        for (int i = 0; i < count; i++)
        {
            float speed = GetAcornSpeed(i);
            LaunchAcorn(speed, i, count);
        }
    }
    float GetAcornSpeed(int index)
    {
        if (acornSpeeds == null || acornSpeeds.Length == 0)
        {
            return launchSpeed;
        }

        if (index < acornSpeeds.Length)
        {
            return acornSpeeds[index];
        }

        return acornSpeeds[acornSpeeds.Length - 1];
    }
    public void ReleaseThrownAcorns()
    {
        if (throwReleased)
        {
            return;
        }

        throwReleased = true;

        if (BunnyIsFetching())
        {
            throwCanceled = true;
            return;
        }

        LaunchAcorns();
    }

    bool BunnyIsFetching()
    {
        return bunnyFetcher != null && bunnyFetcher.IsFetching;
    }
}
