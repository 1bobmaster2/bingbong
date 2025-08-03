using System.Collections;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class RacketHitbox : NetworkBehaviour
{
    [SerializeField] private float torqueForce, impulseForce, hitTime;
    [SerializeField] private Volume volume;
    [SerializeField, ReadOnly] private GameObject ballObject, midPoint, otherPlayer;
    private bool isHitting, canHit;
    
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
        ChangeVignette(true);
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

    public override void OnNetworkSpawn()
    {
        if (!IsOwner) return;
        
        StartCoroutine(InitializeVariables());

        DisableOtherVolume();
    }

    private void ChangeVignette(bool input)
    {
        if (volume.profile.TryGet(out Vignette vignette))
        {
            vignette.active = input;
        }
    }

    private void DisableOtherVolume()
    {
        Volume otherVolume = otherPlayer.GetComponentInChildren<Volume>();
        otherVolume.enabled = false;
    }
    
    [ServerRpc]
    private void RequestHitServerRpc(ServerRpcParams rpcParams = default)
    {
        if (ballObject == null) return;

        Rigidbody rb = ballObject.GetComponent<Rigidbody>();
        if (rb == null) return;

        if (rb.isKinematic)
        {
            rb.isKinematic = false;
        }

        Vector3 direction = (midPoint.transform.position - transform.position).normalized;
        
        rb.AddTorque(direction * torqueForce, ForceMode.Impulse);
        rb.AddForce(direction * impulseForce, ForceMode.Impulse);
    }

    IEnumerator InitializeVariables()
    {
        while (true)
        {
            midPoint = GameObject.FindWithTag("MidPoint");
            if (IsClient && !IsHost)
            {
                otherPlayer = GameObject.FindWithTag("HostPlayer");
            }
            
            if (IsHost)
            {
                otherPlayer = GameObject.FindWithTag("ClientPlayer");
            }

            if (ClientReadyHandler.instance.allClientsReady)
            {
                yield break;
            }

            yield return null;
        }
    }
}
