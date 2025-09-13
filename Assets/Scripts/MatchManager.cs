using System.Collections.Generic;
using Unity.Netcode;
using UnityEditor;
using UnityEngine;

public class MatchManager : NetworkBehaviour
{
    private HashSet<ulong> readyPlayersHashSet = new();
    private int playerServing = 0;
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
        SetActiveButtonClientRpc(false);
    }
    [ClientRpc(RequireOwnership = false)]
    private void SetActiveButtonClientRpc(bool active)
    {
        buttonObject.SetActive(active); // this sets the button to the specified state for all players
    }
}
