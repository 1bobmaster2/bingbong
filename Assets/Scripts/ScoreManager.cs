using Unity.Netcode;
using UnityEngine;

public class ScoreManager : NetworkBehaviour
{
    public static ScoreManager instance;
    public static NetworkVariable<int> HostScore  = new NetworkVariable<int>(); 
    public static NetworkVariable<int> ClientScore  = new NetworkVariable<int>(); 
    void Awake() => instance = this;


    [ServerRpc(RequireOwnership = false)]
    public void AddScoreServerRpc(ServerRpcParams p = default)
    {
        ulong senderId = p.Receive.SenderClientId;

        if (senderId == 0)
        {
            HostScore.Value++;
            Debug.Log("Host got 1 score, host score is now: " + HostScore.Value);
        }
        else
        {
            ClientScore.Value++;
            Debug.Log("Client got 1 score, client score is now: " + ClientScore.Value);
        }
    }
}
