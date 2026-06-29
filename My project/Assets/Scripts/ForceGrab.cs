using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.InputSystem;

public class ForceGrab : MonoBehaviour
{
    public float grabRange = 5f;
    public float pullSpeed = 10f;
    public InputActionReference gripAction;

    private GameObject targetObject;
    private bool isPulling = false;
    private Rigidbody targetRigidbody;
    private UnityEngine.XR.Interaction.Toolkit.Interactables.XRGrabInteractable targetInteractable;

    void OnEnable()
    {
        gripAction.action.Enable();
        gripAction.action.performed += OnGrip;
    }

    void OnDisable()
    {
        gripAction.action.performed -= OnGrip;
        gripAction.action.Disable();
    }

    void OnGrip(InputAction.CallbackContext context)
    {
        if (!isPulling)
        {
            FindTarget();
        }
    }

    void FindTarget()
    {
        Collider[] colliders = Physics.OverlapSphere(
            transform.position,
            grabRange
        );

        float closestDistance = grabRange;
        GameObject closest = null;

        foreach (Collider col in colliders)
        {
            if (col.GetComponent<UnityEngine.XR.Interaction.Toolkit.Interactables.XRGrabInteractable>() != null)
            {
                float distance = Vector3.Distance(
                    transform.position,
                    col.transform.position
                );

                if (distance < closestDistance)
                {
                    closestDistance = distance;
                    closest = col.gameObject;
                }
            }
        }

        if (closest != null)
        {
            targetObject = closest;
            targetRigidbody = closest.GetComponent<Rigidbody>();
            targetInteractable = closest.GetComponent<UnityEngine.XR.Interaction.Toolkit.Interactables.XRGrabInteractable>();

            // Disable gravity and freeze rotation so it flies straight to hand
            if (targetRigidbody != null)
            {
                targetRigidbody.useGravity = false;
                targetRigidbody.linearVelocity = Vector3.zero;
                targetRigidbody.angularVelocity = Vector3.zero;
                targetRigidbody.freezeRotation = true;
            }

            isPulling = true;
        }
    }

   void Update()
    {
        if (isPulling && targetObject != null)
        {
            // Fly object toward hand
            targetObject.transform.position = Vector3.MoveTowards(
                targetObject.transform.position,
                transform.position,
                pullSpeed * Time.deltaTime
            );

            float distanceToHand = Vector3.Distance(
                targetObject.transform.position,
                transform.position
            );

            // Once it reaches the hand, trigger the actual grab
            if (distanceToHand < 0.1f)
            {
                AttachToHand();
            }
        }
    }

    void AttachToHand()
    {
        isPulling = false;

        // Re-enable physics settings
        if (targetRigidbody != null)
        {
            targetRigidbody.useGravity = true;
            targetRigidbody.freezeRotation = false;
        }

        // Attach object directly to hand
        targetObject.transform.position = transform.position;
        targetObject.transform.SetParent(transform);

        targetObject = null;
        targetRigidbody = null;
        targetInteractable = null;
    }
}