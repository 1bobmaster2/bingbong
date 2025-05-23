using System;
using Unity.Netcode;
using UnityEngine;

public class NewMonoBehaviourScript : NetworkBehaviour
{
    [SerializeField] Camera cam; 
    [SerializeField] Vector3 screenPosition;
    [SerializeField] Vector3 worldPosition;
    [SerializeField] private int nearClipAddAmount;
    
    //private NetworkVariable<int> debugNumber = new NetworkVariable<int>(0, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);

// Update is called once per frame
    void Start()
    {
        GameObject camObj = GameObject.FindWithTag("Player1Cam");
        cam = camObj.GetComponent<Camera>();
        if (cam == null)
        {
            Debug.LogError("No camera found with the tag 'Player1Cam'");
        }
    }
    void Update()
    {
        //Debug.Log(OwnerClientId + ";" + debugNumber.Value);
        if (!IsOwner) return;
        
        MovePlayer();
        //if (Input.GetKeyDown(KeyCode.P))
        //{
        //    debugNumber.Value += 1;
        //}
    }

    void MovePlayer()
    {
        screenPosition = Input.mousePosition;
        screenPosition.z = cam.nearClipPlane + nearClipAddAmount;
        
        worldPosition = cam.ScreenToWorldPoint(screenPosition);
        
        transform.position = worldPosition;
    }
}
