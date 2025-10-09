using Unity.Netcode;
using UnityEngine;

public class ScoreCheck : NetworkBehaviour
{
    public GameObject lastHit; // the GameObject that last hit the ball (either host or client)
    [SerializeField] private bool moveToClient;
    private BallScript ballScript;

    public override void OnNetworkSpawn()
    {
        ballScript = GetComponent<BallScript>();
    }

    void OnCollisionEnter(Collision collision)
    {
        Debug.Log(collision.gameObject.name);
        
        if (collision.gameObject.CompareTag("Untagged"))
        {
            return; // we have no interest in untagged GameObjects
        }
        if (collision.gameObject.CompareTag("HostTableSide"))
        {
            ScoreManager.instance.AddScoreServerRpc("Client");
        }
        else if (collision.gameObject.CompareTag("ClientTableSide"))
        {
            ScoreManager.instance.AddScoreServerRpc("Host");
        }
        else if (collision.gameObject.CompareTag("Ground")) // this here doesnt work for some reason
        {
            if (lastHit == null) return;

            if (lastHit.CompareTag("HostPlayer"))
            {
                ScoreManager.instance.AddScoreServerRpc("Client");
            }
            else if (lastHit.CompareTag("ClientPlayer"))
            {
                ScoreManager.instance.AddScoreServerRpc("Host");
            }
        }
    }
}
