using System;
using UnityEngine;
using UnityEngine.AI;

public class PlayerInteractor : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private float interactRange = 3f;
    [SerializeField] private LayerMask interactLayer;
    [SerializeField] private Transform holdPoint;
    
    private Camera cam;
    private Renderer selectedObject;
    private GameObject grabbedObject;

    private Color[] originalColors; // Stores the clean colors

    void Awake()
    {
        cam = GetComponent<Camera>();
    }

    void Update()
    {
        // --- 1. Holding Logic ---
        if (grabbedObject != null)
        {
            grabbedObject.transform.position = holdPoint.position;
            grabbedObject.transform.rotation = holdPoint.rotation;

            // Drop with E
            if (Input.GetKeyDown(KeyCode.E)) DropObject();
            return; 
        }

        // --- 2. Raycast Logic ---
        Ray ray = new Ray(cam.transform.position, cam.transform.forward);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, interactRange, interactLayer))
        {
            if (hit.collider.TryGetComponent<Renderer>(out Renderer target))
            {
                // Only process if looking at a NEW object
                if (selectedObject != target)
                {
                    ClearSelection(); // Reset the previous object first
                    selectedObject = target;

                    // STEP 1: Save the ORIGINAL colors FIRST (Before changing anything!)
                    originalColors = new Color[selectedObject.materials.Length];
                    for (int i = 0; i < selectedObject.materials.Length; i++)
                    {
                        originalColors[i] = selectedObject.materials[i].color;
                    }

                    // STEP 2: Apply Yellow Highlight
                    // We loop through all materials to make the whole object glow
                    foreach (Material mat in selectedObject.materials)
                    {
                        // Use 0.5f alpha for a softer highlight
                        mat.color = new Color(1f, 1f, 0f, 0.5f); 
                    }

                    selectedObject.GetComponent<CheatMethod>()?.StartCheating();
                }

                // GRAB LOGIC (Left Click or E)
                // Added "OR E" because your drop uses E
                if ((Input.GetMouseButtonDown(0) || Input.GetKeyDown(KeyCode.E)) && hit.collider.CompareTag("grabbable"))
                {
                    GrabObject(hit.collider.gameObject);
                }
            }
            else ClearSelection();
        }
        else ClearSelection();
    }

    void GrabObject(GameObject obj)
    {
        grabbedObject = obj;
        Rigidbody rb = grabbedObject.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.useGravity = false;
            rb.isKinematic = true; 
        }
        ClearSelection(); // Reset color so it looks normal in hand
    }

    void DropObject()
    {
        if (grabbedObject == null) return;

        Rigidbody rb = grabbedObject.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.useGravity = true;
            rb.isKinematic = false;
        }
        grabbedObject = null;
    }
    
    void ClearSelection()
    {
        if (selectedObject == null) return;

        // RESTORE original colors
        for (int i = 0; i < selectedObject.materials.Length; i++)
        {
            // Check if we have saved colors to restore
            if (originalColors != null && i < originalColors.Length)
            {
                selectedObject.materials[i].color = originalColors[i];
            }
        }

        // Stop cheating BEFORE we set selectedObject to null
        selectedObject.GetComponent<CheatMethod>()?.StopCheating();
        
        selectedObject = null;
    }
}