using System;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class SetOnClick : NetworkBehaviour
{
    [SerializeField] private Button button;
    [SerializeField] private GameObject parent;
    private void Update()
    {
        GameObject player = NetworkManager.LocalClient.PlayerObject.gameObject;
        PlayerScript script = player.GetComponent<PlayerScript>();
        button.onClick.AddListener(script.LeaveGame);
        enabled = false;
    }
}
