using UnityEngine;

public class ProjectileLauncher : MonoBehaviour
{
    public Transform launchPoint;
    public GameObject projectile;
    public Transform player;
    public float launchSpeed = 8f;
    public float maxSpreadAngle = 35f;
    private int noOfAcorns;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        InvokeRepeating("SpawnAcorn", 2, 2f);
       

        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    void SpawnAcorn()
    {
         /*if(noOfAcorns == 10)
        {
           CancelInvoke(); 
           return;
        }*/
        
        noOfAcorns += 1;
        var _projectile = Instantiate(projectile, launchPoint.position, launchPoint.rotation);

        // Get direction toward player
        Vector3 directionToPlayer = (player.position - launchPoint.position).normalized;
        
        // Add random angle spread
        float randomAngle = Random.Range(-maxSpreadAngle, maxSpreadAngle);
        Vector3 spreadDirection = Quaternion.Euler(0, randomAngle, 0) * directionToPlayer;
        
        // Add slight upward arc
        spreadDirection.y += 0.3f;
        spreadDirection.Normalize();
        
        _projectile.GetComponent<Rigidbody>().linearVelocity = spreadDirection * launchSpeed;
        
    }
}
