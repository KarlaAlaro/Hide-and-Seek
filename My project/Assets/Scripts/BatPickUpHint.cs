// Animates the bat's glowing pickup cue and hand hint until the bat is held.
using UnityEngine;

public class BatPickupHint : MonoBehaviour
{
    public Renderer[] glowRenderers;
    public GameObject handHint;

    public Color glowColor = new Color(1f, 0.8f, 0.25f);
    public float minGlowStrength = 0.6f;
    public float maxGlowStrength = 2.5f;
    public float pulseSpeed = 3f;

    public float handBobHeight = 0.08f;
    public float handBobSpeed = 2f;

    private Material[] glowMaterials;
    private Vector3 handStartLocalPosition;
    private bool isHeld;

    void Start()
    {
        CacheGlowMaterials();

        if (handHint != null)
        {
            handStartLocalPosition = handHint.transform.localPosition;
            handHint.SetActive(true);
        }
    }

    void Update()
    {
        if (isHeld)
        {
            return;
        }

        PulseGlow();
        AnimateHandHint();
    }

    public void SetHeld(bool held)
    {
        isHeld = held;

        if (handHint != null)
        {
            handHint.SetActive(!held);
        }

        if (held)
        {
            TurnGlowOff();
        }
    }

    void CacheGlowMaterials()
    {
        if (glowRenderers == null || glowRenderers.Length == 0)
        {
            glowRenderers = GetComponentsInChildren<Renderer>();
        }

        glowMaterials = new Material[glowRenderers.Length];

        for (int i = 0; i < glowRenderers.Length; i++)
        {
            glowMaterials[i] = glowRenderers[i].material;
            glowMaterials[i].EnableKeyword("_EMISSION");
        }
    }

    void PulseGlow()
    {
        float pulse = Mathf.PingPong(Time.time * pulseSpeed, 1f);
        float strength = Mathf.Lerp(minGlowStrength, maxGlowStrength, pulse);
        Color finalColor = glowColor * strength;

        foreach (Material material in glowMaterials)
        {
            if (material != null)
            {
                material.SetColor("_EmissionColor", finalColor);
            }
        }
    }

    void AnimateHandHint()
    {
        if (handHint == null)
        {
            return;
        }

        float bob = Mathf.Sin(Time.time * handBobSpeed) * handBobHeight;
        handHint.transform.localPosition = handStartLocalPosition + Vector3.up * bob;
    }

    void TurnGlowOff()
    {
        foreach (Material material in glowMaterials)
        {
            if (material != null)
            {
                material.SetColor("_EmissionColor", Color.black);
            }
        }
    }
}
