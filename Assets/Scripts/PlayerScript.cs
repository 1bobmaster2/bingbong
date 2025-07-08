using System.Collections;
using Unity.Collections;
using Unity.Netcode;
using UnityEngine;
public class NewMonoBehaviourScript : NetworkBehaviour
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
    [SerializeField] private Rigidbody rb;

    private bool isOtherCamDisabled;
    private string otherPlayerTag;
    
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
        
        // temporary solution but better
        InvokeRepeating("SyncNameAndTag", 0f, 1f);
        Invoke("StopRepeatingSyncNameAndTag", 10f);
        
        if (!IsOwner) return;

        if (IsHost)
        {
            InvokeRepeating("SetNameAndTagToPlayerHost", 0f, 1f);
            Invoke("StopRepeatingSetNameAndTagToPlayerHost", 10f);
        }
        
        if (cam == null)
        {
            Debug.LogError("No camera exists.");
        }

        if (NetworkManager.Singleton.IsClient && !NetworkManager.Singleton.IsHost)
        {
            clientSpawnpoint = GameObject.FindGameObjectWithTag("ClientSpawnPoint").transform;
            
            gameObject.transform.position = clientSpawnpoint.position;
            gameObject.transform.rotation = Quaternion.Euler(0f, 180f, 0f);

            playerName.Value = "ClientPlayer";
            playerTag.Value = "ClientPlayer";
            otherPlayerTag = "HostPlayer";
        }
        else if (NetworkManager.Singleton.IsHost)
        {
            hostSpawnpoint = GameObject.FindGameObjectWithTag("HostSpawnPoint").transform;
            
            gameObject.transform.position = hostSpawnpoint.position;
            
            playerName.Value = "temporary name"; // temporary name needed for it to work
            playerTag.Value = "temporaryDebugTag";
            otherPlayerTag = "ClientPlayer";
        }
        else
        {
            playerName.Value = "something's fucked up";
        }
        
        StartCoroutine(WaitForGameObject(otherPlayerTag));
        
        SpawnNetworkObject(ballPrefab);
    }
    
    void Update()
    {
        if (!IsOwner) return;
        
        MovePlayer();

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

        if (Input.GetMouseButtonDown(0))
        {
            Swing();
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

    void SpawnNetworkObject(GameObject prefab)
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
