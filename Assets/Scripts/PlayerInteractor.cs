using System;
using UnityEngine;
using UnityEngine.AI;

public class PlayerInteractor : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private float interactRange = 3f;
    [SerializeField] private LayerMask interactLayer;
    
    private Camera cam;
    
    private Renderer selectedObject;

    void Awake()
    {
        cam = GetComponent<Camera>();
    }

    void Update()
    {
        Ray ray = new Ray(cam.transform.position, cam.transform.forward);
        RaycastHit hit;

        // 1. Raycast detection
        if (Physics.Raycast(ray, out hit, interactRange, interactLayer))
        {
            // 2. Try to get the interface
            if (hit.collider.TryGetComponent<Renderer>(out Renderer target))
            {
                

                // ALWAYS call OnLook (for highlights, crosshair changes, etc.)

                if (selectedObject != target)
                {
                    ClearSelection();
                    target.materials[^1].color = new Color(1f, 1f, 0f, 0.75f);
                    selectedObject = target;
                    target.GetComponent<CheatMethod>()?.StartCheating();
                }

                // 3. Check for Click (Left Mouse Button)
                if (Input.GetMouseButtonDown(0))
                {
                    //target.OnClick();
                }
            }
            else ClearSelection();
        }
        else ClearSelection();
    }

    void ClearSelection()
    {
        if (selectedObject == null) return;
        
        selectedObject.materials[^1].color = new Color(1f, 1f, 0f, 0f);
        selectedObject.GetComponent<CheatMethod>()?.StopCheating();
        selectedObject = null;
    }
}