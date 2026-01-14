using System;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class MatchManager : NetworkBehaviour
{
    private HashSet<ulong> readyPlayersHashSet = new();
    [ReadOnly] public int playerServing; // 0 means undecided, 1 means host and 2 means client
    [SerializeField] private GameObject ballPrefab;
    [SerializeField] private GameObject startRoundButtonObject;
    [SerializeField] private GameObject coinFlipButtonObject;
    [SerializeField] private GameObject gameUIReference;

    public override void OnNetworkSpawn()
    {
        startRoundButtonObject.SetActive(true); // we set the button active when the host and client load in
        coinFlipButtonObject.SetActive(true);
        Debug.LogWarning("tried setting the 2 buttons to active"); // this doesn't actually say if the 2 operations went smoothly
    }
    
    public void SetReady() // we call this from the button
    {
        NotifyManagerOnReadyServerRpc();
    }

    private void AddReadyPlayer(ulong playerId)
    {
        readyPlayersHashSet.Add(playerId);
    }

    public void CoinFlip() // this method decides who should serve
    {
        int decision = Random.Range(1, 3);
        playerServing = decision; //
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
        if (playerServing == 1)
        {
            playerScript.SpawnNetworkObject(ballPrefab, new Vector3(0, 0, 5));
        }
        else if (playerServing == 2)
        {
            playerScript.SpawnNetworkObject(ballPrefab, new Vector3(0.7f,0,7.6f));
        }
        
        DisableTheButtonsServerRpc();
        //SetActiveSpecificGameObjectClientRpc(startRoundButtonObject.GetComponent<NetworkObject>(), false);
        //SetActiveSpecificGameObjectClientRpc(coinFlipButtonObject.GetComponent<NetworkObject>(), false);
    }

    [ServerRpc(RequireOwnership = false)]
    private void TrySetParentOfReferenceServerRpc(NetworkObjectReference reference)
    {
        if (reference.TryGet(out NetworkObject networkObject))
        {
            bool result = networkObject.TrySetParent(gameUIReference);
            Debug.LogWarning(result.ToString());
        }
    }

    [ServerRpc(RequireOwnership = false)]
    private void DisableTheButtonsServerRpc()
    {
        DisableTheButtonsClientRpc();
    }

    [ClientRpc(RequireOwnership = false)]
    private void DisableTheButtonsClientRpc()
    {
        GameObject[] buttons = GameObject.FindGameObjectsWithTag("ButtonsToBeDisabled");

        foreach (GameObject button in buttons)
        {
            button.SetActive(false);
        }
    }
    [ClientRpc(RequireOwnership = false)]
    private void SetActiveSpecificGameObjectClientRpc(NetworkObjectReference reference, bool active)
    {
        if (reference.TryGet(out NetworkObject networkObject))
        {
            networkObject.gameObject.SetActive(active); // this sets a specific network object to a specific state through a NetworkObjectReference.
        }
    }

    [ClientRpc(RequireOwnership = false)]
    private void SetPositionOfNetworkObjectClientRpc(NetworkObjectReference reference, Vector3 position)
    {
        if (reference.TryGet(out NetworkObject networkObject))
        {
            networkObject.gameObject.transform.position = position;
        }
    }
}
