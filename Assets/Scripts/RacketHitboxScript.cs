using Unity.Netcode;
using UnityEngine;

public class RacketHitbox : NetworkBehaviour
{
    [SerializeField] private float torqueForce, impulseForce, hitTime;
    private GameObject ballObject;
    private bool isHitting, canHit;
    private Rigidbody ballRb;
    
    private void Update()
    {
        if (!IsOwner) return;

        if (Input.GetMouseButtonDown(0))
        {
            isHitting = true;
            Debug.Log("set isHitting to true");
            Invoke("StopHitting", hitTime);
        }
        
        if (ballObject != null && isHitting && canHit)
        {
            Debug.Log("hit the ball");
            if (ballRb.isKinematic)
            {
                ballRb.isKinematic = false;
            }
            ballRb.AddTorque(Vector3.forward * torqueForce , ForceMode.Impulse);
            ballRb.AddForce(Vector3.forward * impulseForce, ForceMode.Impulse);
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
        if (other.CompareTag("Ball"))
        {
            ballObject = other.gameObject;
            ballRb = ballObject.GetComponent<Rigidbody>();
        }
    }
    
    void OnTriggerExit(Collider other)
    {
        canHit = false;
    }
}
