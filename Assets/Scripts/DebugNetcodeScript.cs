using UnityEngine;
using Unity.Netcode;
public class DebugNetcodeScript : MonoBehaviour
{
    void Update()
    {
        
    }

    public void StartHost()
    {
        NetworkManager.Singleton.StartHost();
    }

    public void StartClient()
    {
        NetworkManager.Singleton.StartClient();
    }
}
