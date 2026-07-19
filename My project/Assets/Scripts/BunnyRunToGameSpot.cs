using UnityEngine;
using UnityEngine.AI;

public class BunnyRunToGameSpot : MonoBehaviour
{
    public NavMeshAgent agent;
    public Transform[] gameSpots;
    public Transform gameSpot;
    private int currentGameSpotIndex;
    public Transform lookAtTarget;
    public Animator bunnyAnimator;
    public bool runOnStart = true;
    public float arrivalDistance = 0.6f;
    public float runningVelocityThreshold = 0.05f;
    public bool faceTargetAfterArrival = true;
    public float turnSpeed = 360f;
    public float facingOffsetY = 0f;

    public bool HasArrived { get; private set; }

    private bool hasDestination;
    private static readonly int RunningHash = Animator.StringToHash("Running");
    private static readonly int RunningStateHash = Animator.StringToHash("Running");

    void Start()
    {
        if (agent == null)
        {
            agent = GetComponent<NavMeshAgent>();
        }

        if (runOnStart)
        {
            RunToGameSpot();
        }
    }

    public void RunToGameSpot()
    {
        if (agent == null || gameSpot == null || !agent.isOnNavMesh)
        {
            return;
        }
        
        if (NavMesh.SamplePosition(gameSpot.position, out NavMeshHit hit, 2f, agent.areaMask))
        {

            HasArrived = false;
            agent.isStopped = false;
            agent.SetDestination(hit.position);
        }
        else
        {
            return;
        }

        hasDestination = true;

        if (bunnyAnimator != null)
        {
            bunnyAnimator.SetBool(RunningHash, true);
            bunnyAnimator.CrossFade(RunningStateHash, 0.1f);
        }
    }

    void Update()
    {
        if (agent == null)
        {
            return;
        }

        bool isRunning = hasDestination || agent.velocity.sqrMagnitude > runningVelocityThreshold * runningVelocityThreshold;

        if (hasDestination && !agent.pathPending && agent.remainingDistance <= Mathf.Max(agent.stoppingDistance, arrivalDistance))
        {
            HasArrived = true;
            hasDestination = false;
            agent.isStopped = true;
            isRunning = false;
        }

        if (bunnyAnimator != null)
        {
            bunnyAnimator.SetBool(RunningHash, isRunning);
        }

        if (HasArrived && faceTargetAfterArrival)
        {
            FaceTarget();
        }
    }

    void FaceTarget()
    {
        if (lookAtTarget == null)
        {
            return;
        }

        Vector3 direction = lookAtTarget.position - transform.position;
        direction.y = 0f;

        if (direction.sqrMagnitude < 0.001f)
        {
            return;
        }

        Quaternion targetRotation = Quaternion.LookRotation(direction.normalized) * Quaternion.Euler(0f, facingOffsetY, 0f);
        transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, turnSpeed * Time.deltaTime);
    }
    public void RunToRandomGameSpot()
    {
        if (gameSpots == null || gameSpots.Length == 0)
        {
            RunToGameSpot();
            return;
        }

        int nextIndex = Random.Range(0, gameSpots.Length);

        while (nextIndex == currentGameSpotIndex && gameSpots.Length > 1)
        {
            nextIndex = Random.Range(0, gameSpots.Length);
        }

        currentGameSpotIndex = nextIndex;
        gameSpot = gameSpots[currentGameSpotIndex];

        RunToGameSpot();
    }
}
