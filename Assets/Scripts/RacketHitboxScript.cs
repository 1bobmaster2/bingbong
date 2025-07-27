using Unity.Netcode;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class RacketHitbox : NetworkBehaviour
{
    [SerializeField] private float torqueForce, impulseForce, hitTime;
    [SerializeField] private Volume volume;
    private GameObject ballObject, midPoint;
    private bool isHitting, canHit;
    private Rigidbody ballRb;
    
    private void Update()
    {
        if (!IsOwner) return;

        if (Input.GetMouseButtonDown(0))
        {
            isHitting = true;
            Debug.Log("set isHitting to true");
            Invoke(nameof(StopHitting), hitTime);
        }
        
        if (ballObject != null && isHitting && canHit)
        {
            Debug.Log("hit the ball");
            RequestHitServerRpc();
        }
    }

    private void StopHitting()
    {
        isHitting = false;
        Debug.Log("set isHitting to false");
    }
    
    void OnTriggerEnter(Collider other)
    {
        canHit = true;
        ChangeVignette(true);
        if (other.CompareTag("Ball"))
        {
            ballObject = other.gameObject;
            ballRb = ballObject.GetComponent<Rigidbody>();
        }
    }
    
    void OnTriggerExit(Collider other)
    {
        canHit = false;
        ChangeVignette(false);
    }

    public override void OnNetworkSpawn()
    {
        midPoint = GameObject.FindWithTag("MidPoint");
    }

    private void ChangeVignette(bool input)
    {
        if (volume.profile.TryGet(out Vignette vignette))
        {
            vignette.active = input;
        }
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
}
