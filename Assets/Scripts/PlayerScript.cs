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
    
    public void Start()
    {
        if (cam == null)
        {
            Debug.LogError("No camera exists.");
        }

        if (NetworkManager.Singleton.IsClient && !NetworkManager.Singleton.IsHost)
        {
            gameObject.transform.position = new Vector3(0f, 0f, 10f);
            gameObject.transform.rotation = Quaternion.Euler(0f, 180f, 0f);

            gameObject.name = "ClientPlayer";
        }
        else
        {
            gameObject.name = "HostPlayer";
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
