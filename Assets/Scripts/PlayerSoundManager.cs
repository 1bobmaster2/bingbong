using UnityEngine;

public class PlayerSoundManager : NetworkBehaviour
{
    private void Update()
    {
        if (ClientLoadedHandler.instance.allClientsLoaded)
        {
            
        }
    }
}
