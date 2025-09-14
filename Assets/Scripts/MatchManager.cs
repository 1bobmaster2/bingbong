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
        NotifyManagerOnReadyServerRpc();
    }

    private void AddReadyPlayer(ulong playerId)
    {
        readyPlayersHashSet.Add(playerId);
    }

    public void CoinFlip() // this method decides who should serve, this method is called from a button
    {
        int decision = Random.Range(1, 3);
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
        CoinFlip();
        GameObject hostPlayer = GameObject.FindWithTag("HostPlayer");
        PlayerScript playerScript = hostPlayer.GetComponent<PlayerScript>();
        playerScript.SpawnNetworkObject(ballPrefab);
        SetActiveSpecificGameObjectClientRpc(startRoundButtonObject.GetComponent<NetworkObject>(), false);
        SetActiveSpecificGameObjectClientRpc(coinFlipButtonObject.GetComponent<NetworkObject>(), false);
    }
    [ClientRpc(RequireOwnership = false)]
    private void SetActiveSpecificGameObjectClientRpc(NetworkObjectReference reference, bool active)
    {
        if (reference.TryGet(out NetworkObject networkObject))
        {
            networkObject.gameObject.SetActive(active); // this sets a specific network object to a specific state through a NetworkObjectReference.
        }
    }
}
