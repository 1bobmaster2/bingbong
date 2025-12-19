using Unity.Netcode;
using UnityEngine;

public class PlayerSoundManager : NetworkBehaviour
{
    private void Update()
    {
        if (ClientLoadedHandler.instance.allClientsLoaded)
        {
            if (!IsOwner) return;
            
            
            if (IsHost) // this script disables the audio listener of the other player, also sometimes doesnt work
            {
                GameObject otherPlayer = GameObject.FindGameObjectWithTag("ClientPlayer");
                AudioListener otherPlayerListener = otherPlayer.GetComponentInChildren<AudioListener>();
                otherPlayerListener.enabled = false;
            }
            else if (IsClient)
            {
                GameObject otherPlayer = GameObject.FindGameObjectWithTag("HostPlayer");
                AudioListener otherPlayerListener = otherPlayer.GetComponentInChildren<AudioListener>();
                otherPlayerListener.enabled = false;
            }
        }
    }
}
