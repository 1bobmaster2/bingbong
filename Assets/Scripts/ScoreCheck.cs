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
            
        }
    }
}
