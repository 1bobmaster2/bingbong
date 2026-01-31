using Unity.Netcode;

public class ClientLoadedScript : NetworkBehaviour
{
    public override void OnNetworkSpawn()//
    {
        if (!IsOwner) return; // only the owner should run this script
        
        NotifyHandlerOnReadyServerRpc(); // notifies the host when the client has joined
    }

    [ServerRpc(RequireOwnership = false)]
    private void NotifyHandlerOnReadyServerRpc(ServerRpcParams p = default)
    {
        ulong clientId = p.Receive.SenderClientId; // gets the client id of the client who sent the rpc
        ClientLoadedHandler.instance.MarkClientReady(clientId); // marks the client ready
    }
}
