using System;
using Unity.Netcode;
using UnityEngine;

public class NewMonoBehaviourScript : NetworkBehaviour
{
    [SerializeField] Camera cam; 
    [SerializeField] Vector3 screenPosition;
    [SerializeField] Vector3 worldPosition;
    [SerializeField] private int nearClipAddAmount;
    [SerializeField] private GameObject palletObject;
    
    //private NetworkVariable<int> debugNumber = new NetworkVariable<int>(0, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);
    
    public void Start()
    {
        if (cam == null)
        {
            Debug.LogError("No camera exists.");
        }

        if (NetworkManager.Singleton.IsClient && !NetworkManager.Singleton.IsHost)
        {
            gameObject.transform.position = new Vector3(6.70f, 0f, 9.15f);
            gameObject.transform.rotation = Quaternion.Euler(0f, 180f, 0f);
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
        
        palletObject.transform.position = worldPosition;
    }
}
