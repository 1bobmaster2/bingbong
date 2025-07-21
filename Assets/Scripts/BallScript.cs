using System;
using Unity.Netcode;
using UnityEngine;

public class BallScript : NetworkBehaviour
{
    [SerializeField, ReadOnly] private float force;
    [SerializeField] private float forceMultiplier;
    [SerializeField] private float outlineSizeModifier;
    [SerializeField] private float posPowMult;
    [Space]
    [SerializeField] private Rigidbody rb;
    [Space]
    [SerializeField] GameObject hostPlayerObject, clientPlayerObject;

    private Vector3 lastVelocity;
    private Material outlineMaterial;
    private int widthProperty;
    private bool hasBeenHit;
    
    void Start()
    {
        outlineMaterial = gameObject.transform.GetChild(0).GetChild(0).GetComponent<MeshRenderer>().material;
        widthProperty = Shader.PropertyToID("_OutlineWidth");

        hasBeenHit = false;
        rb.isKinematic = true;
    }
    void FixedUpdate()
    {
        lastVelocity = rb.linearVelocity;
        force = rb.linearVelocity.magnitude * forceMultiplier;
        
        rb.linearVelocity = Vector3.ClampMagnitude(rb.linearVelocity, 25f);

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
            outlineMaterial.SetFloat(widthProperty, (float)Math.Pow(Vector3.Distance(hostPlayerObject.transform.position, gameObject.transform.position), posPowMult) * outlineSizeModifier);
        }
        else
        {
            outlineMaterial.SetFloat(widthProperty, (float)Math.Pow(Vector3.Distance(clientPlayerObject.transform.position, gameObject.transform.position), posPowMult) * outlineSizeModifier);
        }
    }
    
    void OnCollisionEnter(Collision collision)
    {
        if(!IsServer) return;

        if (!hasBeenHit)
        {
            hasBeenHit = true;
            rb.isKinematic = false;
        }
        
        GameObject hit = collision.gameObject;


        ContactPoint contact = collision.contacts[0];

        Vector3 incomingVelocity = lastVelocity;

        
        Vector3 reflectedVelocity = Vector3.Reflect(incomingVelocity, contact.normal);

        float adjustedMagnitude = Mathf.Max(0f, incomingVelocity.magnitude - force);
        Vector3 forceToAdd = reflectedVelocity.normalized * adjustedMagnitude;
        

        rb.AddForce(forceToAdd, ForceMode.VelocityChange);
    }

    GameObject WaitForGameObject(string tag)
    {
        return GameObject.FindGameObjectWithTag(tag);
    }
}
