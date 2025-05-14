using System;
using UnityEngine;

public class NewMonoBehaviourScript : MonoBehaviour
{
    [SerializeField] Camera cam; // TODO: make this set dynamically, not force set in editor ok???
    public Vector3 screenPosition;
    public Vector3 worldPosition;
    
    // Update is called once per frame
    void Update()
    {
        // TODO: return if not owner, only do after adding multiplayer!!!!!
        
        screenPosition = Input.mousePosition;
        screenPosition.z = cam.nearClipPlane + 5;
        
        worldPosition = cam.ScreenToWorldPoint(screenPosition);
        
        transform.position = worldPosition;
    }
   
}
