using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class MatchReadyManager : NetworkBehaviour
{
    [SerializeField] private GameObject ballPrefab;

    public void SetReady()
    {
        GameObject caller = UnityEngine.EventSystems.EventSystem.current.currentSelectedGameObject;
        Destroy(caller);
        
        GameObject hostPlayer = GameObject.FindWithTag("HostPlayer");
        PlayerScript playerScript = hostPlayer.GetComponent<PlayerScript>();
        playerScript.SpawnNetworkObject(ballPrefab);
    }
}
