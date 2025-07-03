using Unity.Netcode;
using UnityEngine;

public class BallScript : NetworkBehaviour
{
    [SerializeField] private float force;
    [SerializeField] private float outlineSizeModifier;
    [Space]
    [SerializeField] private Rigidbody rb;
    [Space]
    [SerializeField] GameObject hostPlayerObject, clientPlayerObject;

    private Vector3 lastVelocity;
    private Material outlineMaterial;
    private int WidthProperty;
    void Start()
    {
        if (!IsOwner) return;
        
        outlineMaterial = gameObject.transform.GetChild(0).GetChild(0).GetComponent<MeshRenderer>().material;
        WidthProperty = Shader.PropertyToID("_OutlineWidth");
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

    void Update()
    {
        if (IsHost)
        {
            outlineMaterial.SetFloat(WidthProperty, Vector3.Distance(hostPlayerObject.transform.position, gameObject.transform.position) * 0.1f);
        }
    }
    
    void OnCollisionEnter(Collision collision)
    {
        if(!IsServer) return;
        
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
