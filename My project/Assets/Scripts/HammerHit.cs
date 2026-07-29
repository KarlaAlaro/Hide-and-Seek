using UnityEngine;

public class HammerHit : MonoBehaviour
{
    public float hitForce = 14f;
    public float minimumHitSpeed = 0f;
    public Transform bunny;
    public Transform returnTarget;
    public float upwardAim = 0.6f;
    public bool clearExistingVelocity = true;
    public bool logHits = true;
      public AudioSource audioSource;
    public AudioClip hitSound;

    void OnCollisionEnter(Collision collision)
    {
        if (logHits)
        {
            Debug.Log("Branch collided with: " + collision.gameObject.name +
                " speed: " + collision.relativeVelocity.magnitude);
        }
         if (audioSource != null && hitSound != null && collision.gameObject.name == "Acorn")
        {
            audioSource.PlayOneShot(hitSound);
        }

        TryHitAcorn(collision.collider, collision.transform.position, collision.relativeVelocity.magnitude);
    }

    void OnTriggerEnter(Collider other)
    {
       
        if (logHits)
        {
            Debug.Log("Branch trigger touched: " + other.gameObject.name);
        }

        TryHitAcorn(other, other.transform.position, minimumHitSpeed);
    }

    void TryHitAcorn(Collider hitCollider, Vector3 hitPosition, float hitSpeed)
    {
        AcornReturnState acornState = hitCollider.GetComponentInParent<AcornReturnState>();
        bool isAcorn = hitCollider.CompareTag("Acorn") || acornState != null;

        if (!isAcorn)
        {
            return;
        }

        if (hitSpeed < minimumHitSpeed)
        {
            if (logHits)
            {
                Debug.Log("Branch touched acorn, but hit was too slow: " + hitSpeed);
            }

            return;
        }

        if (acornState != null)
        {
            acornState.MarkHitBack();
        }

        Rigidbody acornRb = hitCollider.attachedRigidbody;

        if (acornRb == null && acornState != null)
        {
            acornRb = acornState.GetComponent<Rigidbody>();
        }

        if (acornRb == null)
        {
            acornRb = hitCollider.GetComponentInParent<Rigidbody>();
        }

        if (acornRb != null)
        {
            Vector3 swingDirection = transform.forward;
            swingDirection.y = 0f;
            swingDirection.Normalize();
            Vector3 bunnyDirection = (GetReturnTargetPosition() - hitPosition).normalized;

            Vector3 finalDirection = Vector3.Lerp(swingDirection, bunnyDirection, 0.7f).normalized;
            finalDirection = (finalDirection + Vector3.up * upwardAim).normalized;

            if (clearExistingVelocity)
            {
                acornRb.linearVelocity = Vector3.zero;
                acornRb.angularVelocity = Vector3.zero;
            }

            acornRb.AddForce(finalDirection * hitForce, ForceMode.Impulse);
        }

        if (logHits)
        {
            Debug.Log("Branch hit acorn back.");
        }
    }

    Vector3 GetReturnTargetPosition()
    {
        if (returnTarget != null)
        {
            return returnTarget.position;
        }

        if (bunny != null)
        {
            Transform catchArea = bunny.Find("Bunny Catch Area");

            if (catchArea != null)
            {
                return catchArea.position;
            }

            return bunny.position + Vector3.up;
        }

        return transform.position + transform.forward;
    }
}
