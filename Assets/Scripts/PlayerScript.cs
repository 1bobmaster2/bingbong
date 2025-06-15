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
    [SerializeField] private GameObject otherPlayerCamObject;

    private bool isOtherCamDisabled;
    private string otherPlayerTag;
    
    private NetworkVariable<FixedString32Bytes> playerName = new( "",  NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner );
    private NetworkVariable<FixedString32Bytes> playerTag = new( "",  NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner );
    

    
    public override void OnNetworkSpawn()
    {
        playerName.OnValueChanged += (oldValue, newValue) =>
        {
            Debug.Log($"{name}'s name changed from '{oldValue}' to '{newValue}'");
            gameObject.name = newValue.ToString();
        };
        playerTag.OnValueChanged += (oldValue, newValue) =>
        {
            Debug.Log($"{tag}'s name changed from '{oldValue}' to '{newValue}'");
            gameObject.tag = newValue.ToString();
        };
        
        if (!IsOwner) return;
        
        if (cam == null)
        {
            Debug.LogError("No camera exists.");
        }

        if (NetworkManager.Singleton.IsClient && !NetworkManager.Singleton.IsHost)
        {
            gameObject.transform.position = new Vector3(0f, 0f, 10f);
            gameObject.transform.rotation = Quaternion.Euler(0f, 180f, 0f);

            playerName.Value = "ClientPlayer";
            playerTag.Value = "ClientPlayer";
            otherPlayerTag = "HostPlayer";
        }
        else if (NetworkManager.Singleton.IsHost)
        {
            playerName.Value = "HostPlayer";
            playerTag.Value = "HostPlayer";
        }
        else
        {
            playerName.Value = "something's fucked up";
        }
    }
    
    void Update()
    {
        if (!IsOwner) return;
        
        MovePlayer();

        if (otherPlayer != null && otherPlayerCamObject != null)
        {
            otherPlayerCamObject = FindChildObject(otherPlayer, "Camera");
        }
    }

    void MovePlayer()
    {
        screenPosition = Input.mousePosition;
        screenPosition.z = cam.nearClipPlane + nearClipAddAmount;
        
        worldPosition = cam.ScreenToWorldPoint(screenPosition);
        
        palletObject.transform.position = worldPosition;
    }
}
