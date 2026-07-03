using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.InputSystem;

public class ForceGrab : MonoBehaviour
{
    public float grabRange = 5f;
    public float pullSpeed = 10f;
    public float attachDistance = 0.1f;
    public Transform holdAnchor;
    public Vector3 heldLocalPosition = Vector3.zero;
    public Vector3 heldLocalEulerAngles = Vector3.zero;
    public bool keepAboveGroundOnRelease = true;
    public float groundReleaseHeight = 0.35f;
    public InputActionReference gripAction;

    private GameObject targetObject;
    private bool isPulling = false;
    private bool isHolding = false;
    private Rigidbody targetRigidbody;
    private UnityEngine.XR.Interaction.Toolkit.Interactables.XRGrabInteractable targetInteractable;
    private Transform originalParent;
    private bool originalUseGravity;
    private bool originalIsKinematic;
    private bool originalFreezeRotation;

    void OnEnable()
    {
        if (gripAction == null)
        {
            return;
        }

        gripAction.action.Enable();
        gripAction.action.performed += OnGrip;
        gripAction.action.canceled += OnGripReleased;
    }

    void OnDisable()
    {
        DropObject();

        if (gripAction == null)
        {
            return;
        }

        gripAction.action.performed -= OnGrip;
        gripAction.action.canceled -= OnGripReleased;
        gripAction.action.Disable();
    }

    void OnGrip(InputAction.CallbackContext context)
    {
        if (!isPulling && !isHolding)
        {
            FindTarget();
        }
    }

    void OnGripReleased(InputAction.CallbackContext context)
    {
        DropObject();
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
            originalParent = closest.transform.parent;

            if (targetRigidbody != null)
            {
                originalUseGravity = targetRigidbody.useGravity;
                originalIsKinematic = targetRigidbody.isKinematic;
                originalFreezeRotation = targetRigidbody.freezeRotation;

                // Take physics control while the object is being pulled to the hand.
                targetRigidbody.useGravity = false;
                targetRigidbody.isKinematic = true;
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
                GetHoldPosition(),
                pullSpeed * Time.deltaTime
            );

            float distanceToHand = Vector3.Distance(
                targetObject.transform.position,
                GetHoldPosition()
            );

            if (distanceToHand < attachDistance)
            {
                AttachToHand();
            }
        }
    }

    void AttachToHand()
    {
        isPulling = false;
        isHolding = true;

        if (targetRigidbody != null)
        {
            targetRigidbody.useGravity = false;
            targetRigidbody.isKinematic = true;
            targetRigidbody.linearVelocity = Vector3.zero;
            targetRigidbody.angularVelocity = Vector3.zero;
            targetRigidbody.freezeRotation = true;
        }

        Transform parent = holdAnchor != null ? holdAnchor : transform;
        targetObject.transform.SetParent(parent, false);
        targetObject.transform.localPosition = heldLocalPosition;
        targetObject.transform.localRotation = Quaternion.Euler(heldLocalEulerAngles);
    }

    void DropObject()
    {
        if (targetObject == null)
        {
            isPulling = false;
            isHolding = false;
            return;
        }

        targetObject.transform.SetParent(originalParent, true);

        if (keepAboveGroundOnRelease)
        {
            MoveAboveGroundIfNeeded();
        }

        if (targetRigidbody != null)
        {
            targetRigidbody.useGravity = originalUseGravity;
            targetRigidbody.isKinematic = originalIsKinematic;
            targetRigidbody.freezeRotation = originalFreezeRotation;
            targetRigidbody.linearVelocity = Vector3.zero;
            targetRigidbody.angularVelocity = Vector3.zero;
        }

        isPulling = false;
        isHolding = false;
        targetObject = null;
        targetRigidbody = null;
        targetInteractable = null;
        originalParent = null;
    }

    Vector3 GetHoldPosition()
    {
        Transform parent = holdAnchor != null ? holdAnchor : transform;
        return parent.TransformPoint(heldLocalPosition);
    }

    void MoveAboveGroundIfNeeded()
    {
        RaycastHit groundHit;

        if (!TryFindGroundBelow(targetObject.transform.position, out groundHit))
        {
            return;
        }

        float minimumY = groundHit.point.y + groundReleaseHeight;

        if (targetObject.transform.position.y < minimumY)
        {
            Vector3 correctedPosition = targetObject.transform.position;
            correctedPosition.y = minimumY;
            targetObject.transform.position = correctedPosition;
        }
    }

    bool TryFindGroundBelow(Vector3 position, out RaycastHit groundHit)
    {
        Vector3 origin = position + Vector3.up * 20f;
        RaycastHit[] hits = Physics.RaycastAll(origin, Vector3.down, 100f);
        float closestDistance = float.MaxValue;
        bool foundGround = false;
        groundHit = new RaycastHit();

        foreach (RaycastHit hit in hits)
        {
            if (hit.collider == null || hit.collider.transform.IsChildOf(targetObject.transform))
            {
                continue;
            }

            if (!hit.collider.CompareTag("Ground"))
            {
                continue;
            }

            if (hit.distance < closestDistance)
            {
                closestDistance = hit.distance;
                groundHit = hit;
                foundGround = true;
            }
        }

        return foundGround;
    }
}
