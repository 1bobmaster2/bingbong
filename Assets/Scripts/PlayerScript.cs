using System.Collections;
using Unity.Collections;
using Unity.Netcode;
using UnityEngine;
public class PlayerScript : NetworkBehaviour
{
    [SerializeField] private float zoomFactor;
    [Space]
    [SerializeField] Camera cam; 
    [SerializeField] Vector3 screenPosition;
    [SerializeField] Vector3 worldPosition;
    [SerializeField] private float nearClipAddAmount;
    [SerializeField] private GameObject palletObject;
    [SerializeField] private GameObject otherPlayer;
    [SerializeField] private GameObject otherPlayerCamObject;
    [SerializeField] private GameObject ballPrefab;
    [SerializeField] private Transform hostSpawnpoint, clientSpawnpoint;
    [SerializeField] private Material hostMaterial, clientMaterial;

    private bool isOtherCamDisabled;
    private string otherPlayerTag;
    private bool materialSynchronized;
    
    private NetworkVariable<FixedString32Bytes> playerName = new( "",  NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner );
    private NetworkVariable<FixedString32Bytes> playerTag = new( "",  NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner );
    
    public override void OnNetworkSpawn()
    {
        playerName.OnValueChanged += (oldValue, newValue) =>
        {
            Debug.Log($"{name}'s name changed from '{oldValue}' to '{newValue}'");
            gameObject.name = newValue.ToString();
        };
        playerTag.OnValueChanged += (oldValue, newValue) =>
        {
            Debug.Log($"{tag}'s name changed from '{oldValue}' to '{newValue}'");
            gameObject.tag = newValue.ToString();
        };

        if (!ClientLoadedHandler.instance.allClientsLoaded)
        {
            SyncNameAndTag();
        }
        
        if (!IsOwner) return;

        if (IsHost)
        {
            if (!ClientLoadedHandler.instance.allClientsLoaded)
            {
                SetNameAndTagToPlayerHost();
            }
        }
        
        if (cam == null)
        {
            Debug.LogError("No camera exists.");
        }

        Renderer rend = transform.GetChild(2).GetComponent<Renderer>(); // we get the renderer of the game object, the component with a renderer is the third child (the racket model)
        
        if (NetworkManager.Singleton.IsClient && !NetworkManager.Singleton.IsHost)
        {
            clientSpawnpoint = GameObject.FindGameObjectWithTag("ClientSpawnPoint").transform;
            
            gameObject.transform.position = clientSpawnpoint.position;
            gameObject.transform.rotation = Quaternion.Euler(0f, 180f, 0f);

            playerName.Value = "ClientPlayer";
            playerTag.Value = "ClientPlayer";
            otherPlayerTag = "HostPlayer";
            rend.material = clientMaterial;
        }
        else if (NetworkManager.Singleton.IsHost)
        {
            hostSpawnpoint = GameObject.FindGameObjectWithTag("HostSpawnPoint").transform;
            
            gameObject.transform.position = hostSpawnpoint.position;
            
            playerName.Value = "HostPlayer";
            playerTag.Value = "HostPlayer";
            otherPlayerTag = "ClientPlayer";
            rend.material = hostMaterial;
        }
        else
        {
            playerName.Value = "somethings fucked up";
        }
        
        StartCoroutine(WaitForGameObject(otherPlayerTag));
    }
    
    void Update()
    {
        if (!IsOwner) return;
        
        MovePlayer();

        if (!materialSynchronized)
        {
            Renderer otherRend = otherPlayer.transform.GetChild(2).GetComponent<Renderer>(); // we get the renderer of the other player, and then set the correct material
            if (otherRend == null)
            {
                Debug.LogError("couldn't find otherRend");
                return;
            }
            if (IsHost)
            {
                otherRend.material = clientMaterial;
            }
            else
            {
                otherRend.material = hostMaterial;
            }
            materialSynchronized = true;
        }

        if (otherPlayer != null && otherPlayerCamObject == null)
        {
            otherPlayerCamObject = FindChildObject(otherPlayer, "Camera");
        }

        if (otherPlayerCamObject != null && !isOtherCamDisabled)
        {
            otherPlayerCamObject.GetComponent<Camera>().enabled = false;
            isOtherCamDisabled = true;
        }

        if (Input.GetKey(KeyCode.W))
        {
            gameObject.transform.position += transform.forward * (1 * zoomFactor * Time.deltaTime);
        }
        else if (Input.GetKey(KeyCode.S))
        {
            gameObject.transform.position += transform.forward * (-1 * zoomFactor * Time.deltaTime);
        }
        else if (Input.mouseScrollDelta.y > 0)
        {
            gameObject.transform.position += transform.forward * (3 * zoomFactor * Time.deltaTime);
        }
        else if (Input.mouseScrollDelta.y < 0)
        {
            gameObject.transform.position += transform.forward * (-3 * zoomFactor * Time.deltaTime);
        }
    }

    void MovePlayer()
    {
        screenPosition = Input.mousePosition;
        screenPosition.z = cam.nearClipPlane + nearClipAddAmount;
        
        worldPosition = cam.ScreenToWorldPoint(screenPosition);
        
        palletObject.transform.position = worldPosition;
    }
    
    void ZoomInAndOut(int direction) // if its 1 then its forward, if –1 then backwards.
    {
        nearClipAddAmount += direction * zoomFactor * Time.deltaTime;
    }

    public void SpawnNetworkObject(GameObject prefab)
    {
         if(!IsHost) return;
         
         GameObject instance = Instantiate(prefab, new Vector3(0,0,5), Quaternion.identity);
         instance.GetComponent<NetworkObject>().Spawn();
    }

    void SyncNameAndTag()
    {
        gameObject.name = playerName.Value.ToString();
        gameObject.tag = playerTag.Value.ToString();
    }

    void SetNameAndTagToPlayerHost()
    {
        playerName.Value = "HostPlayer";
        playerTag.Value = "HostPlayer";
    }

    void StopRepeatingSyncNameAndTag()
    {
        CancelInvoke("SyncNameAndTag");
    }

    void StopRepeatingSetNameAndTagToPlayerHost()
    {
        CancelInvoke("SetNameAndTagToPlayerHost");
    }

    IEnumerator WaitForGameObject(string tag)
    {
        while (otherPlayer == null)
        {
            otherPlayer = GameObject.FindWithTag(tag);
            yield return null; 
        }
    }

    GameObject FindChildObject(GameObject parentObject, string childName)
    {
        foreach (Transform child in parentObject.transform)
        {
            if (child.name == childName)
            {
                return child.gameObject;
            }
        }
        return null; // return null if it didnt find it
    }
}
