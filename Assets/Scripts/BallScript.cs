using UnityEngine;

public class BallScript : MonoBehaviour
{
    [SerializeField] private float force;
    [Space]
    [SerializeField] private Rigidbody rb;

    private Vector3 lastVelocity;
    
    void FixedUpdate()
    {
        lastVelocity = rb.linearVelocity;
    }
    
    void OnCollisionEnter(Collision collision)
    {
        GameObject hit = collision.gameObject;


        ContactPoint contact = collision.contacts[0];

        Vector3 incomingVelocity = GetComponent<Rigidbody>().linearVelocity;

        
        Vector3 reflectedVelocity = Vector3.Reflect(incomingVelocity, contact.normal);

        
        Vector3 forceToAdd = reflectedVelocity.normalized * incomingVelocity.magnitude * force;
        
        Rigidbody rb = GetComponent<Rigidbody>();
        rb.linearVelocity = Vector3.zero;

        
        rb.AddForce(forceToAdd, ForceMode.VelocityChange);
        
        if (hit.CompareTag("HostPlayer") || hit.CompareTag("ClientPlayer"))
        {

        }
    }
}
