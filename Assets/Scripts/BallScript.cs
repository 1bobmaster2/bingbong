using Unity.Netcode;
using UnityEngine;

public class BallScript : NetworkBehaviour
{
    private static readonly int WidthProperty = Shader.PropertyToID("Outline Width");
    
    [SerializeField] private float force;
    [Space]
    [SerializeField] private Rigidbody rb;
    [Space]
    [SerializeField] GameObject hostPlayerObject, clientPlayerObject;

    private Vector3 lastVelocity;
    private Material outlineMaterial;

    void Start()
    {
        if (!IsOwner) return;
        
        outlineMaterial = gameObject.transform.GetChild(0).GetChild(0).GetComponent<MeshRenderer>().material;
    }
    void FixedUpdate()
    {
        if(!IsServer) return;
        
        lastVelocity = rb.linearVelocity;

        if (hostPlayerObject == null || clientPlayerObject == null)
        {
            hostPlayerObject = WaitForGameObject("HostPlayer");
            clientPlayerObject = WaitForGameObject("ClientPlayer");
        }
    }
    
    void OnCollisionEnter(Collision collision)
    {
        GameObject hit = collision.gameObject;


        ContactPoint contact = collision.contacts[0];

        Vector3 incomingVelocity = lastVelocity;

        
        Vector3 reflectedVelocity = Vector3.Reflect(incomingVelocity, contact.normal);

        
        Vector3 forceToAdd = reflectedVelocity.normalized * incomingVelocity.magnitude * force;
        

        rb.AddForce(forceToAdd, ForceMode.VelocityChange);
    }

    GameObject WaitForGameObject(string tag)
    {
        return GameObject.FindGameObjectWithTag(tag);
    }
}
