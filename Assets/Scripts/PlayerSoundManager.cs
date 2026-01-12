using Unity.Netcode;
using UnityEngine;//

public class PlayerSoundManager : NetworkBehaviour
{
    private void Update()
    {
        if (ClientLoadedHandler.instance.allClientsLoaded)
        {
            if (!IsOwner) return;//
            
            
            if (IsHost) // this script disables the audio listener of the other player, also sometimes doesnt work (doesnt work if the host is hosting from an instance not from the unity editor)
            {
                
                
                GameObject otherPlayer = GameObject.FindGameObjectWithTag("ClientPlayer");
                if (otherPlayer == null)
                {
                    Debug.Log("lookforthis otherPlayer is null");
                }
                AudioListener otherPlayerListener = otherPlayer.GetComponentInChildren<AudioListener>();
                if (otherPlayerListener == null)
                {
                    Debug.Log("lookforthis otherPlayerListener is null");
                }
                otherPlayerListener.enabled = false;
            }
            else if (IsClient) // this should work
            {
                GameObject otherPlayer = GameObject.FindGameObjectWithTag("HostPlayer");
                if (otherPlayer == null)
                {
                    Debug.Log("lookforthis otherPlayer is null");
                }
                AudioListener otherPlayerListener = otherPlayer.GetComponentInChildren<AudioListener>();
                if (otherPlayerListener == null)
                {
                    Debug.Log("lookforthis otherPlayerListener is null");
                }
                otherPlayerListener.enabled = false;
            }
        }
    }
}
