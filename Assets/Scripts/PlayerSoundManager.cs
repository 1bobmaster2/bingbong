using Unity.Netcode;
using UnityEngine;//

public class PlayerSoundManager : NetworkBehaviour
{
    private void Update()
    {
        if (ClientLoadedHandler.instance.allClientsLoaded)
        {
            if (!IsOwner) return;
            
            
            if (IsHost) // this script disables the audio listener of the other player, also sometimes doesnt work (doesnt work if the host is hosting from an instance not from the unity editor)
            {
                GameObject[] gameObjects = FindObjectsOfType<GameObject>();
                string message = "lookforthisaaa  ";
                foreach (GameObject go in gameObjects)
                { 
                    message += go.name;
                    message += "  and the tag is ";
                    message += go.tag;
                    message += " NEXT ";
                }
                
                Debug.Log(message);
                
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
            else if (IsClient)
            {
                GameObject otherPlayer = GameObject.FindGameObjectWithTag("HostPlayer");
                AudioListener otherPlayerListener = otherPlayer.GetComponentInChildren<AudioListener>();
                otherPlayerListener.enabled = false;
            }
        }
    }
}
