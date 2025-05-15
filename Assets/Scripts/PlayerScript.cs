using System;
using UnityEngine;

public class NewMonoBehaviourScript : MonoBehaviour
{
    [SerializeField] Camera cam; // TODO: make this set dynamically, not force set in editor ok???
    public Vector3 screenPosition;
    public Vector3 worldPosition;
    
    // Update is called once per frame
    void Start()
    {
        GameObject camObj = GameObject.FindWithTag("Player1Cam");
        cam = camObj.GetComponent<Camera>();
        if (cam == null)
        {
            Debug.LogError("No camera found with the tag 'Player1Cam'");
        }
    }
    void Update()
    {
        // TODO: return if not owner, only do after adding multiplayer!!!!!
        
        screenPosition = Input.mousePosition;
        /*screenPosition.z = cam.nearClipPlane + 2f;
        
        worldPosition = cam.ScreenToWorldPoint(screenPosition);*/
        
        Ray ray = cam.ScreenPointToRay(screenPosition);

        if (Physics.Raycast(ray, out RaycastHit hitData))
        {
            worldPosition = hitData.point;
        }
        
        transform.position = worldPosition;
    }
}
