using Unity.Netcode;
using UnityEngine;

public class ReadyButtonScript : NetworkBehaviour
{
    void Update() //TODO: fix this
    {
        if (!IsHost)
        {
            Destroy(gameObject);
        }
    }
}
