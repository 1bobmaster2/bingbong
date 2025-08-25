using Unity.Netcode;
using UnityEngine;

public class ReadyButtonScript : NetworkBehaviour
{
    void Update() //TODO: fix this
    {
        if (!IsHost)
        {
            gameObject.SetActive(false); // TODO: fix this, again!
        }
    }
}
