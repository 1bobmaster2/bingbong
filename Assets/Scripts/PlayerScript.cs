using System;
using UnityEngine;

public class NewMonoBehaviourScript : MonoBehaviour
{
    [SerializeField] Camera cam; // TODO: make this set dynamically, not force set in editor ok???
    private Vector3? lastHitPoint = null;

    private int layerMask;
    // Update is called once per frame
    void Update()
    {
        // TODO: return if not owner, only do after adding multiplayer!!!!!
        Ray ray = cam.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        
        if (Physics.Raycast(ray, out hit, layerMask))
        {
            Vector3 worldPosition = hit.point;
            Debug.Log("Mouse is pointing at: " + worldPosition);
            transform.position = worldPosition;
        }      
    }

    private void Start()
    {
        int ignoreLayer = LayerMask.GetMask("Ignore Raycast");
        layerMask = ~(1 << ignoreLayer);
    }
}
