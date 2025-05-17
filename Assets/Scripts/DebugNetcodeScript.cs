using UnityEngine;
using Unity.Netcode;
public class DebugNetcodeScript : MonoBehaviour
{
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.H))
        {
            NetworkManager.Singleton.StartHost();
        }
        else if (Input.GetKeyDown(KeyCode.C))
        {
            NetworkManager.Singleton.StartClient();
        }
    }
}
