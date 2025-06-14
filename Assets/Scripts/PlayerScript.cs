using System;
using Unity.Collections;
using Unity.Netcode;
using UnityEngine;


public class NewMonoBehaviourScript : NetworkBehaviour
{
    [SerializeField] Camera cam; 
    [SerializeField] Vector3 screenPosition;
    [SerializeField] Vector3 worldPosition;
    [SerializeField] private int nearClipAddAmount;
    [SerializeField] private GameObject palletObject;
    [SerializeField] private GameObject otherPlayer;

    private bool isOtherCamDisabled;
    
    private NetworkVariable<FixedString32Bytes> playerName = new( "",  NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner );
    private NetworkVariable<FixedString32Bytes> playerTag = new( "",  NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner );
    

    
    public override void OnNetworkSpawn()
    {
        playerName.OnValueChanged += (oldValue, newValue) =>
        {
            Debug.Log($"{name}'s name changed from '{oldValue}' to '{newValue}'");
            gameObject.name = newValue.ToString();
        };
    }
    
    void Update()
    {
        if (!IsOwner) return;
        
        MovePlayer();
        
        //if (NetworkManager.Singleton.IsHost && !isOtherCamDisabled)
        //{
            //Camera otherCamera = GameObject.FindGameObjectWithTag("Player2Cam").GetComponent<Camera>();
            //otherCamera.enabled = false;
            //isOtherCamDisabled = true;
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
