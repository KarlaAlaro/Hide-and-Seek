using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class BunnyFetchAcorn : MonoBehaviour
{
    public NavMeshAgent agent;
    public BunnyRunToGameSpot bunnyRunner;
    public Animator bunnyAnimator;
    public AudioSource bunnyAudioSource;
    public AudioClip fetchSound;    

    public float fetchChance = 0.35f;
    public float pickupDistance = 0.6f;
    public float maxFetchDistance = 10f;
    public float pauseAtAcornTime = 1.1f;
    public string pickupAnimationStateName = "Base Layer.Pick up";
    public float pickupAnimationFadeTime = 0.05f;
    public Transform player;
    public float minDistanceFromPlayer = 2f;
    private static readonly int RunningStateHash = Animator.StringToHash("Running");


    private bool isFetching;

    public bool IsFetching => isFetching;

    private static readonly int RunningHash = Animator.StringToHash("Running");

    void Awake()
    {
        if (agent == null)
        {
            agent = GetComponent<NavMeshAgent>();
        }
    }

    public void TryFetchAcorn(GameObject acorn)
    {
        if (isFetching || acorn == null || agent == null || !agent.isOnNavMesh)
        {
            return;
        }

        if (player != null && Vector3.Distance(player.position, acorn.transform.position) < minDistanceFromPlayer)
        {
            return;
        }

        float distanceToAcorn = Vector3.Distance(transform.position, acorn.transform.position);

        if (distanceToAcorn > maxFetchDistance)
        {
            return;
        }

        if (Random.value > fetchChance)
        {
            return;
        }

        StartCoroutine(FetchRoutine(acorn));
    }

    IEnumerator FetchRoutine(GameObject acorn)
    {
        isFetching = true;
        Vector3 returnPosition = transform.position;

        if (bunnyAnimator != null)
        {
            
            bunnyAnimator.SetBool(RunningHash, true);
            bunnyAnimator.CrossFade(RunningStateHash, 0.1f);
        }

        agent.isStopped = false;
        agent.SetDestination(acorn.transform.position);

        while (acorn != null && agent.pathPending)
        {
            yield return null;
        }

        while (acorn != null && agent.remainingDistance > pickupDistance)
        {
            yield return null;
        }

        if (bunnyAnimator != null)
        {
            bunnyAnimator.SetBool(RunningHash, false);
            bunnyAnimator.SetBool("Wave", false);
            if (acorn == null || Vector3.Distance(transform.position, acorn.transform.position) > pickupDistance)
            {
                isFetching = false;
                yield break;
            }
            bunnyAnimator.CrossFadeInFixedTime(pickupAnimationStateName, pickupAnimationFadeTime, 0, 0f);
            bunnyAnimator.SetTrigger("Pick up");
            if (bunnyAudioSource != null && fetchSound != null)
            {
                bunnyAudioSource.Stop();
                bunnyAudioSource.clip = fetchSound;
                bunnyAudioSource.Play();
            }
        }

        yield return new WaitForSeconds(pauseAtAcornTime);

        if (acorn != null)
        {
            Destroy(acorn);
        }
        if (bunnyRunner != null)
        {
            bunnyRunner.RunToGameSpot();

            while (!bunnyRunner.HasArrived)
            {
                yield return null;
            }
        }

        if (bunnyAnimator != null)
        {
            bunnyAnimator.SetBool(RunningHash, true);
        }

       
        isFetching = false;
    }
}
