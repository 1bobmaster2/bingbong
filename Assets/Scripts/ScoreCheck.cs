using Unity.Netcode;
using UnityEngine;

public class ScoreCheck : NetworkBehaviour
{
    public GameObject lastHit;
    private BallScript ballScript;

    public override void OnNetworkSpawn()
    {
        ballScript = GetComponent<BallScript>();
    }

    void OnCollisionEnter(Collision collision)
    {
        ballScript.MoveBallToSpawn();
        if (collision.gameObject.CompareTag("HostTableSide"))
        {
            ScoreManager.instance.AddScoreServerRpc("Client");
        }
        else if (collision.gameObject.CompareTag("ClientTableSide"))
        {
            ScoreManager.instance.AddScoreServerRpc("Host");
        }
        else if (collision.gameObject.CompareTag("Ground"))
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
