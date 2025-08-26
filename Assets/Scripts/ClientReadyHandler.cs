using System.Collections.Generic;
using UnityEngine;

public class ClientReadyHandler : MonoBehaviour
{
    public static ClientReadyHandler instance;
    private HashSet<ulong> readyClients = new HashSet<ulong>();
    [SerializeField] public int expectedPlayers;
    public bool allClientsLoaded;
    
    void Awake() => instance = this;

    public void MarkClientReady(ulong clientId)
    {
        readyClients.Add(clientId);

        if (readyClients.Count == expectedPlayers) // if the number of joined clients is equal to the expected players, we set allClientsReady to true
        {
            allClientsLoaded = true;
            Debug.Log("All clients ready");
        }
    }
}
