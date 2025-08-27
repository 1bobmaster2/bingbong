using Unity.Netcode;
using UnityEngine;

public class ReadyButtonScript : NetworkBehaviour
{
    public void AddReadyPlayerWhenClicked()
    {
        MatchReadyManager.instance.NotifyManagerOnReadyServerRpc();
    }
}
