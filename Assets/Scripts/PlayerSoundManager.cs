using Unity.Netcode;

public class PlayerSoundManager : NetworkBehaviour
{
    private void Update()
    {
        if (ClientLoadedHandler.instance.allClientsLoaded)
        {
            if (IsHost)
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
