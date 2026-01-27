using System.Threading.Tasks;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using Unity.Services.Relay;
using Unity.Services.Relay.Models;
using UnityEngine;

public class RelayScript : MonoBehaviour
{
    public string joinCode;
    [SerializeField] private GameObject gameUI;

    public async Task<string> CreateRelay()
    {
        try
        {
            Allocation allocation; //
            if (ClientLoadedHandler.instance.expectedPlayers <= 1)
            {
                allocation = await RelayService.Instance.CreateAllocationAsync(ClientLoadedHandler.instance.expectedPlayers + 1); // the amount of connections in an allocation cannot be 0,
                                                                                                                                               // so if the expected players is less than 1 we add a 1 to prevent the error
            }
            else // this is also here to prevent an error
            {
                allocation = await RelayService.Instance.CreateAllocationAsync(ClientLoadedHandler.instance.expectedPlayers - 1);    
            }
            
            joinCode = await RelayService.Instance.GetJoinCodeAsync(allocation.AllocationId);
            
            NetworkManager.Singleton.GetComponent<UnityTransport>().SetHostRelayData( // this is an old method
                allocation.RelayServer.IpV4,
                (ushort)allocation.RelayServer.Port,
                allocation.AllocationIdBytes,
                allocation.Key,
                allocation.ConnectionData
                );

            NetworkManager.Singleton.StartHost();
            
            return joinCode;
        }
        catch (RelayServiceException e)
        {
            Debug.Log(e);
        }

        return null;
    }

    public async void JoinRelay(string joinCode)
    {
        try
        {
            JoinAllocation allocation = await RelayService.Instance.JoinAllocationAsync(joinCode);
            
            NetworkManager.Singleton.GetComponent<UnityTransport>().SetClientRelayData(
                allocation.RelayServer.IpV4,
                (ushort)allocation.RelayServer.Port,
                allocation.AllocationIdBytes,
                allocation.Key,
                allocation.ConnectionData,
                allocation.HostConnectionData
            );
            
            NetworkManager.Singleton.StartClient();
        }
        catch (RelayServiceException e)
        {
            Debug.Log(e);
        }
    }
}
