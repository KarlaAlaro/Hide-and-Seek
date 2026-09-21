// Runs the bunny near the player and plays its end-of-experience celebration.
using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class BunnyCelebration : MonoBehaviour
{
    public Transform player;
    public NavMeshAgent agent;
    public Animator bunnyAnimator;

    public GameObject confetti;
    public AudioSource audioSource;
    public AudioClip celebrationSound;

    public float stopDistanceFromPlayer = 1.5f;
    public float arrivalDistance = 0.4f;
    public float maxRunTime = 5f;
    public float turnSpeed = 360f;

    public string runningBool = "Running";
    public string danceTrigger = "Dance";

    private bool isCelebrating;


    public void StartCelebration()
    {
        if (isCelebrating)
        {
            return;
        }

        StartCoroutine(CelebrationRoutine());
    }

    IEnumerator CelebrationRoutine()
    {
        isCelebrating = true;

        if (confetti != null)
        {
            confetti.SetActive(false);
        }

        Vector3 targetPosition = GetPointNearPlayer();

        if (agent != null && agent.isOnNavMesh)
        {
            agent.isStopped = false;
            agent.SetDestination(targetPosition);

            if (bunnyAnimator != null)
            {
                bunnyAnimator.SetBool(runningBool, true);
            }

            float timer = 0f;

            while (timer < maxRunTime)
            {
                timer += Time.deltaTime;

                if (!agent.pathPending && agent.remainingDistance <= arrivalDistance)
                {
                    break;
                }

                yield return null;
            }

            agent.isStopped = true;
            agent.ResetPath();
        }

        if (bunnyAnimator != null)
        {
            bunnyAnimator.SetBool(runningBool, false);
        }

        yield return FacePlayer();

        if (bunnyAnimator != null)
        {
            bunnyAnimator.SetTrigger(danceTrigger);
        }

        if (confetti != null)
        {
            confetti.SetActive(true);
        }

        if (audioSource != null && celebrationSound != null)
        {
            audioSource.PlayOneShot(celebrationSound);
        }

        isCelebrating = false;
    }

    Vector3 GetPointNearPlayer()
    {
        Vector3 directionFromPlayer = transform.position - player.position;
        directionFromPlayer.y = 0f;

        if (directionFromPlayer.sqrMagnitude < 0.001f)
        {
            directionFromPlayer = -player.forward;
            directionFromPlayer.y = 0f;
        }

        return player.position + directionFromPlayer.normalized * stopDistanceFromPlayer;
    }

    IEnumerator FacePlayer()
    {
        if (player == null)
        {
            yield break;
        }

        float timer = 0f;

        while (timer < 1f)
        {
            Vector3 direction = player.position - transform.position;
            direction.y = 0f;

            if (direction.sqrMagnitude > 0.001f)
            {
                Quaternion targetRotation = Quaternion.LookRotation(direction.normalized);
                transform.rotation = Quaternion.RotateTowards(
                    transform.rotation,
                    targetRotation,
                    turnSpeed * Time.deltaTime
                );
            }

            timer += Time.deltaTime;
            yield return null;
        }
    }
}
