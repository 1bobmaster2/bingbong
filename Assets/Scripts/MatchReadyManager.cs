using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class MatchReadyManager : NetworkBehaviour
{
    private HashSet<ulong> readyPlayersHashSet = new();
    [SerializeField] private GameObject ballPrefab;
    [SerializeField] private GameObject buttonObject;

    public override void OnNetworkSpawn()
    {
        buttonObject.SetActive(true); // we set the button active when the host and client load in
    }
    
    public void SetReady() // we call this from the button
    {
        NotifyManagerOnReadyServerRpc();
    }

    private void AddReadyPlayer(ulong playerId)
    {
        readyPlayersHashSet.Add(playerId);
    }
    
    [ServerRpc(RequireOwnership = false)]
    private void NotifyManagerOnReadyServerRpc(ServerRpcParams p = default)
    {
        ulong clientId = p.Receive.SenderClientId;
        AddReadyPlayer(clientId); // marks the client ready
        
        if (readyPlayersHashSet.Count == 2)
        {
            StartGameServerRpc();
        }
    }

    [ServerRpc(RequireOwnership = false)]
    private void StartGameServerRpc()
    {
        GameObject hostPlayer = GameObject.FindWithTag("HostPlayer");
        PlayerScript playerScript = hostPlayer.GetComponent<PlayerScript>();
        playerScript.SpawnNetworkObject(ballPrefab);
        buttonObject.SetActive(false); // TODO: make this sync on the client
    }
}
