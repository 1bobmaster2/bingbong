using Unity.Netcode;
using UnityEngine;//

public class PlayerSoundManager : NetworkBehaviour
{
    private bool done;
    
    private void Update()
    {
        if (!IsOwner) return;//

        if (!done)
        {
            if (IsHost) // this script disables the audio listener of the other player
            {
                
                
                GameObject otherPlayer = GameObject.FindGameObjectWithTag("ClientPlayer");
                if (otherPlayer == null)
                {
                    Debug.Log("lookforthis otherPlayer is null");
                    return;
                }
                AudioListener otherPlayerListener = otherPlayer.GetComponentInChildren<AudioListener>();
                if (otherPlayerListener == null)
                {
                    Debug.Log("lookforthis otherPlayerListener is null");
                    return;
                }
                otherPlayerListener.enabled = false;
                done = true;
            }
            else if (IsClient) // this should work
            {
                GameObject otherPlayer = GameObject.FindGameObjectWithTag("HostPlayer");
                if (otherPlayer == null)
                {
                    Debug.Log("lookforthis otherPlayer is null");
                    return;
                }
                AudioListener otherPlayerListener = otherPlayer.GetComponentInChildren<AudioListener>();
                if (otherPlayerListener == null)
                {
                    Debug.Log("lookforthis otherPlayerListener is null");
                    return;
                }
                otherPlayerListener.enabled = false;
                done = true;
            }
        }
    }
}
