using System;
using Unity.Netcode;
using UnityEngine;

public class RacketHitbox : NetworkBehaviour
{
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
