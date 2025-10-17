using System;
using Unity.Netcode;
using UnityEngine;

public class BallScript : NetworkBehaviour
{
    [SerializeField] private float outlineSizeModifier;
    [SerializeField] private float posPowMult;
    [SerializeField, ReadOnly] public bool isGettingServed;
    [Space]
    [SerializeField] private Rigidbody rb;
    [Space]
    [SerializeField] GameObject hostPlayerObject, clientPlayerObject;
    [SerializeField] MatchManager matchManager;
    [SerializeField] ScoreCheck scoreCheck;

    private Vector3 lastVelocity;
    private Material outlineMaterial;
    private int widthProperty;
    private bool hasBeenHit;

    private int playerServing; // 1 means host 2 means client
    private int playerServingSwitchHelperVar; // helper variable for switching the serving player

    private int amountOfBounces;
    
    void Start()
    {
        outlineMaterial = gameObject.transform.GetChild(0).GetChild(0).GetComponent<MeshRenderer>().material;
        widthProperty = Shader.PropertyToID("_OutlineWidth");

        hasBeenHit = false;
        rb.isKinematic = true; // we set isKinematic to false, to prevent the ball from falling to the table, which would make the player loose a point
        
        matchManager = WaitForGameObject("GameManager").GetComponent<MatchManager>();
        playerServing = matchManager.playerServing;
    }
    void FixedUpdate()
    {
        lastVelocity = rb.linearVelocity;
        
        rb.linearVelocity = Vector3.ClampMagnitude(rb.linearVelocity, 25f); // we clamp the velocity to prevent uncontrollable speed

        if (hostPlayerObject == null || clientPlayerObject == null)
        {
            // we assign the player references here, using the WaitForGameObject method (made by me)
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

        amountOfBounces++; // something here doesnt work
        Debug.Log("increased amount of bounces by one, the amount of bounces is now: " + amountOfBounces);
        Vector3 vec;
        
        
        if (isGettingServed && amountOfBounces == 2)
        {
            Debug.Log("isGettingServed is true and there are two bounces so we are calling this if");
            
            isGettingServed = false;
            amountOfBounces = 0;
            Debug.Log("set isGettingServed to false, amountOfBounces to 0");
            
            scoreCheck.shouldCheck = true;
            Debug.Log("accesed score check and set should check to true");
            
            
            if (collision.gameObject.CompareTag("HostTableSide") || collision.gameObject.CompareTag("ClientTableSide") || collision.gameObject.CompareTag("Ground")) // this is called when the ball hits the table
            {
                scoreCheck.shouldCheck = false;
                Debug.Log("set shouldCheck to false");

                
                
                if (playerServing == 1)
                {
                    if (playerServingSwitchHelperVar == 1)
                    {
                        playerServingSwitchHelperVar = 0;
                        playerServing = 2;
                        
                        vec = new Vector3(0.7f, 0, 7.6f);
                        MoveBallToSpawn(vec);
                        Debug.Log($"moved ball to {vec.x}, {vec.y}, {vec.z}");
                        
                        return;
                    }
                
                    vec = new Vector3(0, 0, 5);
                    MoveBallToSpawn(vec);
                    Debug.Log($"moved ball to {vec.x}, {vec.y}, {vec.z}");
                    
                    playerServingSwitchHelperVar++;
                }
                else if (playerServing == 2)
                {
                    if (playerServingSwitchHelperVar == 1)
                    {
                        playerServingSwitchHelperVar = 0;
                        playerServing = 1;
                        vec = new Vector3(0, 0, 5);
                        MoveBallToSpawn(vec);
                        Debug.Log($"moved ball to {vec.x}, {vec.y}, {vec.z}");
                        return;
                    }

                    vec = new Vector3(0.7f, 0, 7.6f);
                    MoveBallToSpawn(vec);
                    Debug.Log($"moved ball to {vec.x}, {vec.y}, {vec.z}");
                    playerServingSwitchHelperVar++;
                }
            }
            else if (collision.gameObject.CompareTag("Net"))
            {
                scoreCheck.shouldCheck = false;
                
                
                if (playerServing == 1)
                {
                    vec = new Vector3(0, 0, 5);
                    MoveBallToSpawn(vec);
                    Debug.Log($"moved ball to {vec.x}, {vec.y}, {vec.z}");
                    return;
                }
                else if (playerServing == 2)
                {
                    vec = new Vector3(0.7f, 0, 7.6f);
                    MoveBallToSpawn(vec);
                    Debug.Log($"moved ball to {vec.x}, {vec.y}, {vec.z}");
                    return;
                }
            }
        }
        else if (amountOfBounces == 1)
        {
            amountOfBounces = 0;
            
            if (collision.gameObject.CompareTag("HostTableSide") || collision.gameObject.CompareTag("ClientTableSide") || collision.gameObject.CompareTag("Ground")) // this is called when the ball hits the table
            {
                if (playerServing == 1)
                {
                    if (playerServingSwitchHelperVar == 1)
                    {
                        playerServingSwitchHelperVar = 0;
                        playerServing = 2;
                        vec = new Vector3(0.7f, 0, 7.6f);
                        MoveBallToSpawn(vec);
                        Debug.Log($"moved ball to {vec.x}, {vec.y}, {vec.z}");
                        return;
                    }
                
                
                    MoveBallToSpawn(new Vector3(0, 0, 5));
                    playerServingSwitchHelperVar++;
                }
                else if (playerServing == 2)
                {
                    if (playerServingSwitchHelperVar == 1)
                    {
                        playerServingSwitchHelperVar = 0;
                        playerServing = 1;
                        MoveBallToSpawn(new Vector3(0, 0, 5));
                        return;
                    }
                
                
                    MoveBallToSpawn(new Vector3(0.7f, 0, 7.6f));
                    playerServingSwitchHelperVar++;
                }
            }
            else if (collision.gameObject.CompareTag("Net"))
            {
                if (playerServing == 1)
                {
                    MoveBallToSpawn(new Vector3(0, 0, 5));
                    return;
                }
                else if (playerServing == 2)
                {
                    MoveBallToSpawn(new Vector3(0.7f, 0, 7.6f));
                    return;
                }
            }
        }

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
