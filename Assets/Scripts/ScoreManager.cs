using TMPro;
using Unity.Netcode;
using UnityEngine;

public class ScoreManager : NetworkBehaviour
{
    public TextMeshProUGUI scoreText;
    public static ScoreManager instance;
    public static NetworkVariable<int> HostScore  = new NetworkVariable<int>(); 
    public static NetworkVariable<int> ClientScore  = new NetworkVariable<int>();
    void Awake() => instance = this;

    public override void OnNetworkSpawn()
    {
        HostScore.OnValueChanged += (oldValue, newValue) =>
        {
            UpdateTextOnAllClientsServerRpc();
        };
        ClientScore.OnValueChanged += (oldValue, newValue) =>
        {
            UpdateTextOnAllClientsServerRpc();
        };
    }
    
    

    [ServerRpc(RequireOwnership = false)]
    public void AddScoreServerRpc(string winner, ServerRpcParams p = default)
    {
        ulong senderId = p.Receive.SenderClientId;

        if (winner == "Host")
        {
            HostScore.Value++;
            Debug.Log("Host got 1 score, host score is now: " + HostScore.Value);
        }
        else if (winner == "Client")
        {
            ClientScore.Value++;
            Debug.Log("Client got 1 score, client score is now: " + ClientScore.Value);
        }
    }
}
