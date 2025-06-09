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
    [SerializeField] private GameObject otherPlayer;

    private bool isOtherCamDisabled;
    
    private NetworkVariable<string> playerName = new(writePerm: NetworkVariableWritePermission.Owner);
    
    public void Start()
    {
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
        }
        else if (NetworkManager.Singleton.IsHost)
        {
            playerName.Value = "HostPlayer";
        }
        else
        {
            playerName.Value = "something's fucked up";
        }
        

        playerName.OnValueChanged += OnNameChanged;


        gameObject.name = playerName.Value; // apply instantly
        
    }
    void Update()
    {
        if (!IsOwner) return;
        
        MovePlayer();
        
        if (NetworkManager.Singleton.IsHost && !isOtherCamDisabled)
        {
            Camera otherCamera = GameObject.FindGameObjectWithTag("Player2Cam").GetComponent<Camera>();
            otherCamera.enabled = false;
            isOtherCamDisabled = true;
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
