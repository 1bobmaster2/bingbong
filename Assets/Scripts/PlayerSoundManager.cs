using Unity.Netcode;

public class PlayerSoundManager : NetworkBehaviour
{
    private void Update()
    {
        if (ClientLoadedHandler.instance.allClientsLoaded)
        {
            if (IsHost)
            {
                
            }
            else if (IsClient)
            {
                
            }
        }
    }
}
