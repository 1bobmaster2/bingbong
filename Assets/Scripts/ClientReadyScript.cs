using Unity.Netcode;

public class ClientReadyScript : NetworkBehaviour
{
    public override void OnNetworkSpawn()
    {
        if (!IsOwner) return;
        
        NotifyHandlerOnReadyServerRpc(); // notifies the host when the client has joined
    }

    [ServerRpc(RequireOwnership = false)]
    private void NotifyHandlerOnReadyServerRpc(ServerRpcParams p = default)
    {
        ulong clientId = p.Receive.SenderClientId;
        ClientReadyHandler.instance.MarkClientReady(clientId); // marks the client ready
    }
}
