using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class MatchReadyManager : NetworkBehaviour
{
    private HashSet<ulong> readyPlayersHashSet = new HashSet<ulong>();
    [SerializeField] private GameObject ballPrefab;
    
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
    }

    [ServerRpc(RequireOwnership = false)]
    private void StartGameServerRpc() // TODO: this doesnt work when the host calls it first and then the client, so i need to fix it soon
    {
        GameObject hostPlayer = GameObject.FindWithTag("HostPlayer");
        PlayerScript playerScript = hostPlayer.GetComponent<PlayerScript>();
        playerScript.SpawnNetworkObject(ballPrefab);
    }
}
