using System.Collections.Generic;
using TMPro;
using Unity.Services.Authentication;
using Unity.Services.Core;
using Unity.Services.Lobbies;
using Unity.Services.Lobbies.Models;
using UnityEngine;
using UnityEngine.UI;

public class TestLobby : MonoBehaviour
{

    private Lobby hostLobby;
    private Lobby joinedLobby;
    private float heartbeatTimer;
    private float lobbyUpdateTimer;
    [SerializeField] private string lobbyCode;
    [SerializeField] private InputField codeInputField;
    [SerializeField] private GameObject lobbyUI, nameNullErrorLabel, codeNullErrorLabel, lobbyNullErrorLabel, joinedLobbyNullErrorLabel, loadPlayersButton, joinSucesfullyLabel;
    [SerializeField] private RelayScript relayScript;
    [SerializeField] private TextMeshProUGUI lobbyCodeText;
    [SerializeField] private GameObject copiedNotificationObject;
    private string playerName;
    private string startGame = "startGame";
    private bool isLobbyHost;
    private bool alreadyConnected;
    
    
    async void Start()
    {
        await UnityServices.InitializeAsync();

        AuthenticationService.Instance.SignedIn += () =>
        {
            if (Application.isEditor)
            {
                Debug.Log("SignedIn: " + AuthenticationService.Instance.PlayerId);
            }
        };
        await AuthenticationService.Instance.SignInAnonymouslyAsync();
        
    }

    void Update()
    {
        HandleLobbyHeartbeat();
        HandleLobbyPollForUpdates();
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

    async void HandleLobbyPollForUpdates()
    {
        if (joinedLobby == null) return;
        
        lobbyUpdateTimer -= Time.deltaTime;
        if (lobbyUpdateTimer < 0f)
        {
            float lobbyUpdateTimerMax = 3f;
            lobbyUpdateTimer = lobbyUpdateTimerMax;

            Lobby lobby = await LobbyService.Instance.GetLobbyAsync(joinedLobby.Id);

            if (lobby.Data.TryGetValue("startGame", out var dataObject))
            {
                if (dataObject.Value != "0")
                {
                    Debug.Log("start game got changed");
                    if (!isLobbyHost && !alreadyConnected)
                    {
                        relayScript.JoinRelay(dataObject.Value);
                        alreadyConnected = true;
                        lobbyUI.SetActive(false);
                    }
                }
            }
            
            joinedLobby = lobby;
        }
    }

    public async void CreateLobby()
    {
        try
        {
            if (isTextNull(playerName, nameNullErrorLabel))
            {
                return;
            }
            
            
            string lobbyName = "TestLobby";
            int maxPlayers = 2;

            CreateLobbyOptions options = new CreateLobbyOptions
            {
                IsPrivate = true,
                Player = GetPlayer(),
                Data = new Dictionary<string, DataObject>
                {
                    { startGame, new DataObject(DataObject.VisibilityOptions.Member, "0") }
                }
            };
            
            Lobby lobby = await LobbyService.Instance.CreateLobbyAsync(lobbyName, maxPlayers, options);
        
            hostLobby = lobby;
            joinedLobby = hostLobby;
            isLobbyHost = true;
            
            Debug.Log("Created Lobby: " + lobby.Name + " " + lobby.MaxPlayers + " " + lobby.Id + " " + lobby.LobbyCode);
            lobbyCodeText.gameObject.SetActive(true);
            lobbyCodeText.text = lobby.LobbyCode;
            GUIUtility.systemCopyBuffer = lobby.LobbyCode;
            Debug.Log("the buffer now has: " + GUIUtility.systemCopyBuffer);
            copiedNotificationObject.SetActive(true);
            PrintPlayers(hostLobby);
            
            
            DisableJoinCreateLobbyButtons();
        }
        catch (LobbyServiceException e)
        {
            Debug.Log(e.Message);
        }
    }

    [ContextMenu("Start Game Lobby")]
    public async void StartGame()
    {
        if (hostLobby == null)
        {
            lobbyNullErrorLabel.SetActive(true);
            return;
        }
        
        string joinCode = await relayScript.CreateRelay();
        var UpdateOptions = new UpdateLobbyOptions
        {
            Data = new Dictionary<string, DataObject>
            {
                { startGame, new DataObject(DataObject.VisibilityOptions.Member, joinCode) }
            }
        };
        
        await LobbyService.Instance.UpdateLobbyAsync(joinedLobby.Id, UpdateOptions);
        alreadyConnected = true;
        
        lobbyUI.SetActive(false);
        
        Debug.Log("set it correctly i think");
    }

    [ContextMenu("Print key")]
    private void PrintstartGameKey()
    {
        if (joinedLobby.Data.TryGetValue("startGame", out var DataObject))
        {
            Debug.Log(DataObject.Value);
        }
        else
        {
            Debug.Log("something went wrong");//
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
            bool isNameNull = isTextNull(playerName, nameNullErrorLabel);
            bool isCodeNull = isTextNull(lobbyCode, codeNullErrorLabel);
            if (isNameNull || isCodeNull)
            {
                return;
            }
            
            JoinLobbyByCodeOptions joinLobbyByCodeOptions = new JoinLobbyByCodeOptions
            {
                Player = GetPlayer()
            };
            
            Lobby lobby = await LobbyService.Instance.JoinLobbyByCodeAsync(lobbyCode, joinLobbyByCodeOptions);
            joinedLobby = lobby;
            isLobbyHost = false;
            
            Destroy(loadPlayersButton); // the player that joined a lobby cannot load the players in
            joinSucesfullyLabel.SetActive(true); 
            
            PrintPlayers(joinedLobby);
            
            Debug.Log("Joined lobby by this code: " + lobbyCode);
            
            DisableJoinCreateLobbyButtons();
        }
        catch (LobbyServiceException e)
        {
            Debug.Log(e);
        }
    }

    public async void LeaveLobby()
    {
        if (joinedLobby == null)
        {
            joinedLobbyNullErrorLabel.SetActive(true);
            return;
        }
        
        try
        {
            await LobbyService.Instance.RemovePlayerAsync(joinedLobby.Id, AuthenticationService.Instance.PlayerId);
        }
        catch (LobbyServiceException e)
        {
            Debug.Log(e);
        }
    }

    public void PrintPlayers()
    {
        PrintPlayers(joinedLobby);
    }

    private void DisableJoinCreateLobbyButtons()
    {
        GameObject lobbyCreateButton = GameObject.Find("LobbyCreateButton");
        lobbyCreateButton.SetActive(false);
            
        GameObject lobbyJoinButton = GameObject.Find("JoinFirstLobbyButton");
        lobbyJoinButton.SetActive(false);
    }
    
    private void PrintPlayers(Lobby lobby)
    {
        foreach (Player player in lobby.Players)
        {
            Debug.Log(player.Data["PlayerName"].Value);
        }
    }

    public void SetLobbyCode(string lobbyCode)
    {
        this.lobbyCode = lobbyCode;
    }

    public void SetPlayerName(string name)
    {
        this.playerName = name;
    }

    private Player GetPlayer()
    {
        return new Player
        {
            Data = new Dictionary<string, PlayerDataObject>
            {
                { "PlayerName", new PlayerDataObject(PlayerDataObject.VisibilityOptions.Member, playerName) }
            }
        };
    }
    
    private bool isTextNull(string text, GameObject objectToEnable)
    {
        if (string.IsNullOrEmpty(text))
        {
            objectToEnable.SetActive(true);
            
            return true;
        }
        
        objectToEnable.SetActive(false);
        return false;
    }
}
