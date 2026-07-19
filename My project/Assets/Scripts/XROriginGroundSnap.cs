using UnityEngine;

[DefaultExecutionOrder(-10000)]
[RequireComponent(typeof(CharacterController))]
public class XROriginGroundSnap : MonoBehaviour
{
    public LayerMask groundLayers = Physics.DefaultRaycastLayers;
    public float raycastStartHeight = 8f;
    public float raycastDistance = 30f;
    public float groundedSkin = 0.02f;
    public bool preferTaggedGround = true;
    public string groundTag = "Ground";

    private CharacterController characterController;

    void Awake()
    {
        SnapToGround();
    }

    public void SnapToGround()
    {
        characterController = GetComponent<CharacterController>();

        if (!TryFindGround(out RaycastHit groundHit))
        {
            return;
        }

        float bottomOffset = characterController.center.y - characterController.height * 0.5f;
        Vector3 position = transform.position;
        position.y = groundHit.point.y - bottomOffset + groundedSkin;
        transform.position = position;
    }

    bool TryFindGround(out RaycastHit groundHit)
    {
        Vector3 rayOrigin = transform.position + Vector3.up * raycastStartHeight;
        RaycastHit[] hits = Physics.RaycastAll(
            rayOrigin,
            Vector3.down,
            raycastDistance,
            groundLayers,
            QueryTriggerInteraction.Ignore
        );

        if (preferTaggedGround && TryGetClosestHit(hits, true, out groundHit))
        {
            return true;
        }

        return TryGetClosestHit(hits, false, out groundHit);
    }

    bool TryGetClosestHit(RaycastHit[] hits, bool requireGroundTag, out RaycastHit closestHit)
    {
        bool foundHit = false;
        float closestDistance = float.MaxValue;
        closestHit = new RaycastHit();

        foreach (RaycastHit hit in hits)
        {
            if (hit.collider == null || hit.collider.transform.IsChildOf(transform))
            {
                continue;
            }

            if (requireGroundTag && !hit.collider.CompareTag(groundTag))
            {
                continue;
            }

            if (hit.distance < closestDistance)
            {
                foundHit = true;
                closestDistance = hit.distance;
                closestHit = hit;
            }
        }

        return foundHit;
    }
}
