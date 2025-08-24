using Unity.Netcode;
using UnityEngine;

public class ReadyButtonScript : NetworkBehaviour
{
    void Start()
    {
        if (!IsHost)
        {
            Destroy(gameObject);
        }
    }
}
