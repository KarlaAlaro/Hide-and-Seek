using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class BunnyFriendlyInteraction : MonoBehaviour
{
    public Transform player;
    public NavMeshAgent agent;
    public Animator bunnyAnimator;
    public AudioSource bunnyAudioSource;
    public AudioClip friendlySound;
    public ProjectileLauncher projectileLauncher;
    public BunnyRunToGameSpot bunnyRunner;

    public bool reactWhenPlayerIsStill = true;
    public float stillTimeBeforeWave = 5f;
    public float playerMoveThreshold = 0.08f;

    public bool doRandomFriendlyMoments = true;
    public Vector2 randomMomentDelay = new Vector2(12f, 25f);
    public bool runNearPlayer = true;
    public float stopDistanceFromPlayer = 2f;
    public float maxRunToPlayerDistance = 8f;
    public float maxRunTowardPlayerTime = 4f;
    public float waveTime = 2f;
    public float turnSpeed = 360f;
    public bool onlyWhenMinigameIsNotRunning = true;
    public bool onlyAfterBunnyArrives = true;

    private Vector3 lastPlayerPosition;
    private float stillTimer;
    private float randomTimer;
    private bool isInteracting;

    private static readonly int RunningHash = Animator.StringToHash("Running");
    private static readonly int WaveHash = Animator.StringToHash("Wave");

    void Start()
    {
        if (agent == null)
        {
            agent = GetComponent<NavMeshAgent>();
        }

        if (player != null)
        {
            lastPlayerPosition = player.position;
        }

        ResetRandomTimer();
    }

    void Update()
    {
        if (player == null || isInteracting || MinigameIsRunning() || BunnyIsStillTravellingToGameSpot())
        {
            return;
        }

        TrackPlayerStillness();
        TrackRandomFriendlyMoment();
    }

    void TrackPlayerStillness()
    {
        if (!reactWhenPlayerIsStill)
        {
            return;
        }

        float playerMovement = Vector3.Distance(player.position, lastPlayerPosition);
        lastPlayerPosition = player.position;

        if (playerMovement <= playerMoveThreshold * Time.deltaTime)
        {
            stillTimer += Time.deltaTime;
        }
        else
        {
            stillTimer = 0f;
        }

        if (stillTimer >= stillTimeBeforeWave)
        {
            stillTimer = 0f;
            StartCoroutine(WaveAtPlayer());
        }
    }

    void TrackRandomFriendlyMoment()
    {
        if (!doRandomFriendlyMoments)
        {
            return;
        }

        randomTimer -= Time.deltaTime;

        if (randomTimer > 0f)
        {
            return;
        }

        ResetRandomTimer();

        if (runNearPlayer && PlayerIsCloseEnoughToVisit())
        {
            StartCoroutine(RunNearPlayerThenWave());
        }
        else
        {
            StartCoroutine(WaveAtPlayer());
        }
    }

    IEnumerator WaveAtPlayer()
    {
        isInteracting = true;
        StopAgent();

        float timer = 0f;
        SetWaving(true);
        PlayFriendlySound();

        while (timer < waveTime)
        {
            FacePlayer();
            timer += Time.deltaTime;
            yield return null;
        }

        SetWaving(false);
        isInteracting = false;
    }

    IEnumerator RunNearPlayerThenWave()
    {
        if (agent == null || !agent.isOnNavMesh)
        {
            yield return WaveAtPlayer();
            yield break;
        }

        isInteracting = true;

        Vector3 targetPosition = GetPointNearPlayer();

        if (NavMesh.SamplePosition(targetPosition, out NavMeshHit hit, 2f, agent.areaMask))
        {
            agent.isStopped = false;
            agent.SetDestination(hit.position);
            SetRunning(true);

            float runTimer = 0f;

            while ((agent.pathPending || agent.remainingDistance > Mathf.Max(agent.stoppingDistance, 0.25f))
                && runTimer < maxRunTowardPlayerTime)
            {
                runTimer += Time.deltaTime;
                yield return null;
            }
        }

        StopAgent();

        float timer = 0f;
        SetWaving(true);
        PlayFriendlySound();

        while (timer < waveTime)
        {
            FacePlayer();
            timer += Time.deltaTime;
            yield return null;
        }

        SetWaving(false);
        isInteracting = false;
    }

    Vector3 GetPointNearPlayer()
    {
        Vector3 awayFromPlayer = transform.position - player.position;
        awayFromPlayer.y = 0f;

        if (awayFromPlayer.sqrMagnitude < 0.001f)
        {
            awayFromPlayer = -player.forward;
            awayFromPlayer.y = 0f;
        }

        return player.position + awayFromPlayer.normalized * stopDistanceFromPlayer;
    }

    bool PlayerIsCloseEnoughToVisit()
    {
        return Vector3.Distance(transform.position, player.position) <= maxRunToPlayerDistance;
    }

    bool MinigameIsRunning()
    {
        return onlyWhenMinigameIsNotRunning && projectileLauncher != null && projectileLauncher.enabled;
    }

    bool BunnyIsStillTravellingToGameSpot()
    {
        return onlyAfterBunnyArrives && bunnyRunner != null && !bunnyRunner.HasArrived;
    }

    void FacePlayer()
    {
        Vector3 direction = player.position - transform.position;
        direction.y = 0f;

        if (direction.sqrMagnitude < 0.001f)
        {
            return;
        }

        Quaternion targetRotation = Quaternion.LookRotation(direction.normalized);
        transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, turnSpeed * Time.deltaTime);
    }

    void StopAgent()
    {
        if (agent != null && agent.isOnNavMesh)
        {
            agent.ResetPath();
            agent.isStopped = true;
        }

        SetRunning(false);
    }

    void SetRunning(bool isRunning)
    {
        if (bunnyAnimator != null)
        {
            bunnyAnimator.SetBool(RunningHash, isRunning);
        }
    }

    void SetWaving(bool isWaving)
    {
        if (bunnyAnimator != null)
        {
            bunnyAnimator.SetBool(WaveHash, isWaving);
        }
    }

    void PlayFriendlySound()
    {
        if (bunnyAudioSource != null && friendlySound != null)
        {
            bunnyAudioSource.PlayOneShot(friendlySound);
        }
    }

    void ResetRandomTimer()
    {
        randomTimer = Random.Range(randomMomentDelay.x, randomMomentDelay.y);
    }
}
