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
    [SerializeField] private GameObject gameUIReference;

    public override void OnNetworkSpawn()
    {
        //startRoundButtonObject.SetActive(true); // we set the button active when the host and client load in
        //coinFlipButtonObject.SetActive(true);
        
        //TODO: fix this shit
        //TODO: set the onclick listeners
        if (IsServer) // we only execute this on the server as only the server should handle spawning objects
        {
            GameObject startRoundButtonObjectInstance = Instantiate(startRoundButtonObject);
            GameObject coinFlipButtonObjectInstance = Instantiate(coinFlipButtonObject);
            
            //startRoundButtonObjectInstance.transform.position = new Vector2(-532, 268);
            //coinFlipButtonObjectInstance.transform.position = new Vector2(-176, 268);
            //SetPositionOfNetworkObjectClientRpc(startRoundButtonObjectInstance.GetComponent<NetworkObject>(), new Vector3(-532, 268, 0));
            //SetPositionOfNetworkObjectClientRpc(coinFlipButtonObjectInstance.GetComponent<NetworkObject>(), new Vector3(-176, 268, 0));
            

            startRoundButtonObjectInstance.GetComponent<NetworkObject>().Spawn();
            TrySetParentOfReferenceServerRpc(startRoundButtonObjectInstance.GetComponent<NetworkObject>());
            startRoundButtonObjectInstance.GetComponent<NetworkObject>().TrySetParent(gameUIReference); // look for soemr eason this doesnt work and it deosnt work for no reason because it says that game uyi referebnce is not spawned
            // even though it is and its really driving me crazy ebcause literallyt this was working and now its not what else would i need to spawn on the netwrok every single gameObject? i think im gonna switch to fucking local because look i cant
            // with this shit like bro im literally doing everything correcltyt and its saying no im gonna fucking delete all this shit or something because i litarally cant any more fucking 200 days of coding only for me to fucking loose it on a simple
            // problem that i should be able to do in 5 minutes and yet im sitting on it for 3 days now because i fucking literally work 30 minutes per day and i call myself a coder im fr crahing out anwsohehoklujaehlg
            
            coinFlipButtonObjectInstance.GetComponent<NetworkObject>().Spawn();
            TrySetParentOfReferenceServerRpc(coinFlipButtonObjectInstance.GetComponent<NetworkObject>());
            coinFlipButtonObjectInstance.GetComponent<NetworkObject>().TrySetParent(gameUIReference);

            SetActiveSpecificGameObjectClientRpc(startRoundButtonObjectInstance.GetComponent<NetworkObject>(), true);
            SetActiveSpecificGameObjectClientRpc(coinFlipButtonObjectInstance.GetComponent<NetworkObject>(), true);
        }
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

    [ServerRpc(RequireOwnership = false)]
    private void TrySetParentOfReferenceServerRpc(NetworkObjectReference reference)
    {
        if (reference.TryGet(out NetworkObject networkObject))
        {
            bool result = networkObject.TrySetParent(gameUIReference);
            Debug.LogWarning(result.ToString());
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
