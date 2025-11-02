using Unity.Netcode;
using UnityEngine;

public class ScoreCheck : NetworkBehaviour
{
    public GameObject lastHit; // the GameObject that last hit the ball (either host or client)
    [SerializeField] private bool moveToClient;
    private BallScript ballScript;

    [ReadOnly] public bool shouldCheck;

    public override void OnNetworkSpawn()
    {
        ballScript = GetComponent<BallScript>();
    }

    void OnCollisionEnter(Collision collision)
    {
        Debug.Log(collision.gameObject.name);

        int snapshotAmountOfBounces = ballScript.amountOfBounces;
        
        if (!shouldCheck)
        {
            Debug.Log("should check is false, so we dont check");
            return;
        }
        
        shouldCheck = false;
        
        Debug.Log("should check is true, so we check for score");
        
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
        else if (collision.gameObject.CompareTag("Ground")) 
        {
            if (lastHit == null)
            {
                Debug.Log("we hit " + collision.gameObject.name + " but last hit was null so we return");
                return;
            }



            if (snapshotAmountOfBounces is > 0 and < 2 && ballScript.isGettingServed) // since the ball bounced once but the player is still serving it, it means that the serving player failed to serve it correctly
            {
                Debug.Log("unsuccesfull serve");
                if (lastHit.CompareTag("HostPlayer"))
                {
                    ScoreManager.instance.AddScoreServerRpc("Client");
                }
                else if (lastHit.CompareTag("ClientPlayer"))
                {
                    ScoreManager.instance.AddScoreServerRpc("Client");
                }
            }
            else
            {
                if (lastHit.CompareTag("HostPlayer"))
                {
                    ScoreManager.instance.AddScoreServerRpc("Host");
                }
                else if (lastHit.CompareTag("ClientPlayer"))
                {
                    ScoreManager.instance.AddScoreServerRpc("Client");
                }
            }
        }
    }
}
