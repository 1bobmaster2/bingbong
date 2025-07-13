using System;
using Unity.Netcode;
using UnityEngine;

public class RacketHitbox : NetworkBehaviour
{
    [SerializeField] private float force;
    private GameObject ballObject;
    private bool isHitting;
    
    private void Update()
    {
        if (!IsOwner) return;

        if (Input.GetMouseButtonDown(0))
        {
            isHitting = true;
            Debug.Log("set isHitting to true");
            Invoke("StopHitting", 1f);
        }
        
        if (ballObject != null && isHitting)
        {
            Debug.Log("hit the ball");
        }
    }

    private void StopHitting()
    {
        isHitting = false;
        Debug.Log("set isHitting to false");
    }
    
    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Ball"))
        {
            ballObject = other.gameObject;
            Rigidbody rb = ballObject.GetComponent<Rigidbody>();
            rb.AddTorque(Vector3.right * force , ForceMode.Impulse);
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Ball"))
        {
            ballObject = null;
        }
    }
}
