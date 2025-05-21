using System;
using Unity.Services.Authentication;
using Unity.Services.Core;
using Unity.Services.Lobbies;
using Unity.Services.Lobbies.Models;
using UnityEngine;
using UnityEngine.UI;

public class TestLobby : MonoBehaviour
{

    private Lobby hostLobby;
    private float heartbeatTimer;
    private string lobbyCode;

    [SerializeField] private InputField codeInputField;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    async void Start()
    {
        await UnityServices.InitializeAsync();

        AuthenticationService.Instance.SignedIn += () =>
        {
            Debug.Log("SignedIn: " + AuthenticationService.Instance.PlayerId);
        };
        await AuthenticationService.Instance.SignInAnonymouslyAsync();
    }

    void Update()
    {
        HandleLobbyHeartbeat();
    }

    async void HandleLobbyHeartbeat()
    {
        if (hostLobby == null) return;
        
        heartbeatTimer -= Time.deltaTime;
        if (heartbeatTimer < 0f)
        {
            float heartbeatTimerMax = 15f;
            heartbeatTimer = heartbeatTimerMax;

            await LobbyService.Instance.SendHeartbeatPingAsync(hostLobby.Id);
        }
    }

    public async void CreateLobby()
    {
        try
        {
            string lobbyName = "TestLobby";
            int maxPlayers = 2;

            CreateLobbyOptions options = new CreateLobbyOptions
            {
                IsPrivate = true,
                
            };
            
            Lobby lobby = await LobbyService.Instance.CreateLobbyAsync(lobbyName, maxPlayers, options);
        
            hostLobby = lobby;
            
            Debug.Log("Created Lobby: " + lobby.Name + " " + lobby.MaxPlayers + " " + lobby.Id + " " + lobby.LobbyCode);
        }
        catch (LobbyServiceException e)
        {
            Debug.Log(e.Message);
        }
    }

    public async void ListLobbies()
    {
        try
        {
            QueryResponse queryResponse =  await LobbyService.Instance.QueryLobbiesAsync();
         
            Debug.Log("Amount of lobbies found: " + queryResponse.Results.Count);
            foreach (Lobby lobby in queryResponse.Results)
            {
                Debug.Log(lobby.Name + " " + lobby.MaxPlayers);
            }
        }
        catch (LobbyServiceException e)
        {
            Debug.Log(e.Message);
        }
    }

    public async void JoinLobbyByCode()
    {
        try
        {
            await LobbyService.Instance.JoinLobbyByCodeAsync(lobbyCode);
            
            Debug.Log("Joined lobby by this code: " + lobbyCode);
        }
        catch (LobbyServiceException e)
        {
            Debug.Log(e);
        }
    }

    public void SetLobbyCode(string lobbyCode)
    {
        this.lobbyCode = lobbyCode;
    }
}
