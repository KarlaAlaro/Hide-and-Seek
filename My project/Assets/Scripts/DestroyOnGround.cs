using UnityEngine;

public class DestroyOnGround : MonoBehaviour
{
    void Start()
    {
        
    }
    void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.CompareTag("Ground"))
        {
            Destroy(gameObject);
        }
    
    }
}
