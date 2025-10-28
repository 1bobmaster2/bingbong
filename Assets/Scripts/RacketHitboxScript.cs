using System.Collections;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class RacketHitboxScript : NetworkBehaviour
{
    [SerializeField] private float torqueForce, impulseForce, hitTime;
    [SerializeField] private Volume volume;
    [SerializeField, ReadOnly] private GameObject ballObject, midPoint, otherPlayer;
    private bool isHitting, canHit, alreadyInitialized;
    
    private void Update()
    {
        if (!IsOwner) return;

        if (Input.GetMouseButtonDown(0))
        {
            isHitting = true;
            if (Application.isEditor)
            {
                Debug.Log("set isHitting to true");
            }
            Invoke(nameof(StopHitting), hitTime);
        }
        
        if (ballObject != null && isHitting && canHit)
        {
            if (Application.isEditor)
            {
                Debug.Log("hit the ball");
            }
            RequestHitServerRpc();
        }
    }

    private void StopHitting()
    {
        isHitting = false;
        if (Application.isEditor)
        {
            Debug.Log("set isHitting to false");
        }
    }
    
    void OnTriggerEnter(Collider other)
    {
        canHit = true;
        ChangeVignette(true); // once the racket gets close to the ball we enable the vignette
        if (other.CompareTag("Ball"))
        {
            ballObject = other.gameObject;
        }
    }
    
    void OnTriggerExit(Collider other)
    {
        canHit = false;
        ChangeVignette(false);
    }

    private void ChangeVignette(bool input)
    {
        if (volume.profile.TryGet(out Vignette vignette))
        {
            vignette.active = input;
        }
    }

    private void DisableOtherVolume() // isnt related with audio
    {
        Volume otherVolume = otherPlayer.GetComponentInChildren<Volume>();
        otherVolume.enabled = false;
    }
    
    [ServerRpc]
    private void RequestHitServerRpc(ServerRpcParams rpcParams = default)
    {
        if (ballObject == null) return;

        Rigidbody rb = ballObject.GetComponent<Rigidbody>();
        BallScript ballScript = ballObject.GetComponent<BallScript>();
        if (rb == null || ballScript == null) return;
        
        ScoreCheck scoreCheck = ballObject.GetComponent<ScoreCheck>();
        scoreCheck.lastHit = gameObject.transform.parent.gameObject.transform.parent.gameObject;

        if (rb.isKinematic)
        {
            rb.isKinematic = false;
            ballScript.isGettingServed = true; // since we are hitting the ball when it is kinematic, the player must be serving it.
        }

        Vector3 direction = (midPoint.transform.position - transform.position).normalized;
        
        rb.AddTorque(direction * torqueForce, ForceMode.Impulse); // we add spin
        rb.AddForce(direction * impulseForce, ForceMode.Impulse); // we propel the ball forward
    }

    
    void InitializeVariables()
    {
        Debug.Log("initializing variables");
            
            midPoint = GameObject.FindWithTag("MidPoint");
            if (IsClient && !IsHost)
            {
                otherPlayer = GameObject.FindWithTag("HostPlayer");
            }
            
            if (IsHost)
            {
                Debug.Log("setting variables for host");
                otherPlayer = GameObject.FindWithTag("ClientPlayer");
                RacketHitboxScript otherPlayerRacketScript = otherPlayer.GetComponentInChildren<RacketHitboxScript>();
                if (otherPlayerRacketScript == null)
                {
                    Debug.Log("something went wrong with setting RacketHitboxScript");
                }
                Debug.Log(otherPlayerRacketScript.gameObject.name + " is the name of the gameObject that hold the racket script of the other player");
                otherPlayerRacketScript.midPoint = GameObject.FindWithTag("MidPoint");
            }

            if (otherPlayer != null)
            {
                DisableOtherVolume();
                yield break;
            }

            yield return null;
        }
    }
}
