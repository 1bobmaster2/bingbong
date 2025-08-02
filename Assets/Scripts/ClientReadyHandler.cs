using System.Collections.Generic;
using UnityEngine;

public class ClientReadyHandler : MonoBehaviour
{
    public static ClientReadyHandler instance;
    private HashSet<ulong> readyClients = new HashSet<ulong>();
    [SerializeField] private int expectedPlayers;
    [ReadOnly]
    public bool allClientsReady;
    
    void Awake() => instance = this;

    public void MarkClientReady(ulong clientId)
    {
        readyClients.Add(clientId);

        if (readyClients.Count == expectedPlayers)
        {
            allClientsReady = true;
            Debug.Log("All clients ready");
        }
    }
}
