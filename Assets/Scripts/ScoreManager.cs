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
        HostScore.OnValueChanged += (oldValue, newValue) => // when either ClientScore or HostScore change, we send an rpc to every user in order to update the score
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

        if (winner == "Host") // winner of the play, not the whole match
        {
            HostScore.Value++;
            Debug.Log("Host got 1 score, host score is now: " + HostScore.Value);
        }
        else if (winner == "Client")
        {
            ClientScore.Value++;
            Debug.Log("Client got 1 score, client score is now: " + ClientScore.Value);
        }

        if (ClientScore.Value == 11 || HostScore.Value == 11)
        {
            EndGameClientRpc();
        }
    }

    [ServerRpc(RequireOwnership = false)]
    private void UpdateTextOnAllClientsServerRpc()
    {
        UpdateTextClientRpc();
    }

    [ClientRpc(RequireOwnership = false)]
    private void UpdateTextClientRpc()
    {
        scoreText.text = HostScore.Value + " - " + ClientScore.Value;
    }

    [ClientRpc(RequireOwnership = false)]
    private void EndGameClientRpc()
    {
        Application.Quit();
    }
    
}
