using Unity.Netcode;
using UnityEngine;

public class ScoreCheck : NetworkBehaviour
{
    public GameObject lastHit;
    [SerializeField] private bool moveToClient;
    private BallScript ballScript;

    public override void OnNetworkSpawn()
    {
        ballScript = GetComponent<BallScript>();
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Untagged"))
        {
            return;
        }

        if (moveToClient)
        {
            ballScript.MoveBallToSpawn(new Vector3(0.77640003f,0f,7.6322999f));
        }
        else
        {
            ballScript.MoveBallToSpawn(new Vector3(0,0,5));
        }
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
