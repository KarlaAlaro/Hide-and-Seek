using UnityEngine;

public class HammerHit : MonoBehaviour
{
    public float hitForce = 10f;
    public Transform bunny;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.CompareTag("Acorn"))
        {
            if(collision.relativeVelocity.magnitude > 1f) //check if swing was fast enough
            {
                Rigidbody acornRb = collision.gameObject.GetComponent<Rigidbody>();
                Vector3 directionToBunny = (bunny.position - collision.transform.position).normalized;
                acornRb.AddForce(directionToBunny * hitForce, ForceMode.Impulse);
            }
        }
    }

}
