using UnityEngine;
using Unity.Netcode;
public class DebugNetcodeScript : MonoBehaviour
{
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.H))
        {
            StartHost();
        }
        else if (Input.GetKeyDown(KeyCode.C))
        {
            StartClient();
        }
    }

    void StartHost()
    {
        NetworkManager.Singleton.StartHost();
    }

    void StartClient()
    {
        NetworkManager.Singleton.StartClient();
    }
}
