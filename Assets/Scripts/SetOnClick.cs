using System;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class SetOnClick : NetworkBehaviour
{
    [SerializeField] private Button button;
    private void Update() // doesnt work btw
    {
        if (IsHost)
        {
            GameObject player = GameObject.FindGameObjectWithTag("HostPlayer");
            PlayerScript playerScript = player.GetComponent<PlayerScript>();
            button.onClick.AddListener(playerScript.LeaveGame);
        }
        else // when client
        {
            GameObject player = GameObject.FindGameObjectWithTag("ClientPlayer");
            PlayerScript playerScript = player.GetComponent<PlayerScript>();
            button.onClick.AddListener(playerScript.LeaveGame);
        }
    }
}
