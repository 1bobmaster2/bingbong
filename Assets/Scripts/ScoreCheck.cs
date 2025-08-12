using UnityEngine;

public class ScoreCheck : MonoBehaviour
{
    public GameObject lastHit;
    void OnCollisionEnter(Collision collision)
    {
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
