using UnityEngine;

public class MatchReadyManager : MonoBehaviour
{
    [SerializeField] private GameObject ballPrefab;

    public void SetReady()
    {
        GameObject caller = UnityEngine.EventSystems.EventSystem.current.currentSelectedGameObject;
        Destroy(caller);
        
        GameObject hostPlayer = GameObject.FindWithTag("HostPlayer");
        PlayerScript playerScript = hostPlayer.GetComponent<PlayerScript>();
        playerScript.SpawnNetworkObject(ballPrefab);
    }
}
