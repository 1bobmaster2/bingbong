using Unity.Netcode;
using UnityEngine;

public class ScoreManager : NetworkBehaviour
{
    public static ScoreManager instance;
    public static NetworkVariable<int> HostScore  = new NetworkVariable<int>(); 
    public static NetworkVariable<int> ClientScore  = new NetworkVariable<int>(); 
    void Awake() => instance = this;
    
}
