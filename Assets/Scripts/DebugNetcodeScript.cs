using UnityEngine;
using Unity.Netcode;
public class DebugNetcodeScript : MonoBehaviour
{
    // this is unused (the whole script)

    public void StartHost()
    {
        NetworkManager.Singleton.StartHost();
    }

    public void StartClient()
    {
        NetworkManager.Singleton.StartClient();
    }
}
