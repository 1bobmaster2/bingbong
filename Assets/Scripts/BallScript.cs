using System;
using Unity.Netcode;
using UnityEngine;

public class BallScript : NetworkBehaviour
{
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
        rb.isKinematic = true; // we set isKinematic to false to prevent the ball from falling to the table, which would make the player loose a point
    }
    void FixedUpdate()
    {
        lastVelocity = rb.linearVelocity;
        //force = rb.linearVelocity.magnitude * forceMultiplier;
        
        rb.linearVelocity = Vector3.ClampMagnitude(rb.linearVelocity, 25f); // we clamp the velocity to prevent uncontrollable speed

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
        if(!IsServer) return; // only the server should handle physics

        if (!hasBeenHit)
        {
            hasBeenHit = true;
            rb.isKinematic = false; // once the ball gets hit we disable isKinematic
        }


        ContactPoint contact = collision.contacts[0];

        Vector3 incomingVelocity = lastVelocity;
        
        Vector3 reflectedVelocity = Vector3.Reflect(incomingVelocity, contact.normal);
        
        Vector3 forceToAdd = reflectedVelocity.normalized;
        
        rb.AddForce(forceToAdd, ForceMode.VelocityChange);
    }

    public void MoveBallToSpawn(Vector3 pos)
    {
        gameObject.transform.position = pos;
        rb.linearVelocity = Vector3.zero;
        SwitchIsKinematic(); // we switch the kinematic state from false to true here to allow the user to hit the ball in time (instead it would just fall and teleport back)
    }

    void SwitchIsKinematic()
    {
        rb.isKinematic = !rb.isKinematic;
    }

    GameObject WaitForGameObject(string tag)
    {
        return GameObject.FindGameObjectWithTag(tag);
    }
}
