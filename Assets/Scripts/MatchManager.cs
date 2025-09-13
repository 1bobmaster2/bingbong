using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class MatchManager : NetworkBehaviour
{
    private HashSet<ulong> readyPlayersHashSet = new();
    private int playerServing = 0; // 0 means undecided, 1 means host and 2 means client
    [SerializeField] private GameObject ballPrefab;
    [SerializeField] private GameObject startRoundButtonObject;
    [SerializeField] private GameObject coinFlipButtonObject;

    public override void OnNetworkSpawn()
    {
        startRoundButtonObject.SetActive(true); // we set the button active when the host and client load in
        coinFlipButtonObject.SetActive(true);
    }
    
    public void SetReady() // we call this from the button
    {
        if (playerServing == 0)
        {
            return; // we return here if it's not decided what player should serve
        }
        NotifyManagerOnReadyServerRpc();
    }

    private void AddReadyPlayer(ulong playerId)
    {
        readyPlayersHashSet.Add(playerId);
    }

    private void CoinFlip() // this method decides who should serve, this method is called from a button
    {
        int decision = Random.Range(1, 2);
        playerServing = decision;
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
    }
    [ClientRpc(RequireOwnership = false)]
    private void SetActiveSpeificGameObjectClientRpc(NetworkObjectReference reference, bool active)
    {
        if (reference.TryGet(out NetworkObject networkObject))
        {
            networkObject.gameObject.SetActive(active); // this sets a specific network object to a specific state through a NetworkObjectReference.
        }
    }
}
