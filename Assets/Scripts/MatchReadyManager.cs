using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class MatchReadyManager : NetworkBehaviour
{
    public static MatchReadyManager instance;
    private HashSet<ulong> readyPlayersHashSet = new HashSet<ulong>();
    [SerializeField] private GameObject ballPrefab;


    void Awake() => instance = this;
    
    public void SetReady()
    {
        GameObject caller = UnityEngine.EventSystems.EventSystem.current.currentSelectedGameObject;
        Destroy(caller);
        
        GameObject hostPlayer = GameObject.FindWithTag("HostPlayer");
        PlayerScript playerScript = hostPlayer.GetComponent<PlayerScript>();
        playerScript.SpawnNetworkObject(ballPrefab);
        NotifyManagerOnReadyServerRpc();

        NotifyManagerOnReadyServerRpc();

    }

    private void AddReadyPlayer(ulong playerId)
    {
        readyPlayersHashSet.Add(playerId);
    }
    
    [ServerRpc(RequireOwnership = false)]
    public void NotifyManagerOnReadyServerRpc(ServerRpcParams p = default)
    {
        ulong clientId = p.Receive.SenderClientId;
        AddReadyPlayer(clientId); // marks the client ready
    }
}
