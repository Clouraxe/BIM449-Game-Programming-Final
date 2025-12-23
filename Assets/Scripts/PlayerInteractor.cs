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

    [Header("Trajectory Settings")]
    [SerializeField] private LineRenderer lineRenderer;
    [SerializeField] private int linePoints = 25;
    [SerializeField] private float timeBetweenPoints = 0.1f;
    [SerializeField] private float throwForce = 15f; // Must match your throw force

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
            DrawProjection();

            grabbedObject.transform.position = holdPoint.position;
            grabbedObject.transform.rotation = holdPoint.rotation;

            if (Input.GetMouseButtonDown(0)) 
            {
                ThrowObject();
            }

            // Drop with E
            if (Input.GetKeyDown(KeyCode.E)){ 
                DropObject();
            }
            return; 
        }
        else
        {
            // Hide line when not holding anything
            if(lineRenderer != null) lineRenderer.enabled = false;
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
                if ((Input.GetMouseButtonDown(0)) && hit.collider.CompareTag("grabbable"))
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
    
    void ThrowObject()
    {
        if (grabbedObject == null) return;

        Rigidbody rb = grabbedObject.GetComponent<Rigidbody>();
        Collider col = grabbedObject.GetComponent<Collider>();

        // 1. Re-enable Physics
        if (rb != null)
        {
            rb.useGravity = true;
            rb.isKinematic = false;
        }

        // 2. Re-enable Collider so it hits the teacher/desk
        if (col != null)
        {
            col.enabled = true;
        }

        // 3. Apply the Throw Force
        if (rb != null)
        {
            rb.AddForce(cam.transform.forward * throwForce, ForceMode.Impulse);
        }

        grabbedObject = null;
    }

    void DrawProjection()
    {
        if (lineRenderer == null) return; // Safety check

        lineRenderer.enabled = true;
        lineRenderer.positionCount = linePoints; // Set the exact array size first

        Vector3 startPosition = holdPoint.position;
        Vector3 startVelocity = cam.transform.forward * throwForce;

        // FIX: Loop using 'int i' (Integers) to prevent the crash
        for (int i = 0; i < linePoints; i++)
        {
            // Calculate time based on the index
            float time = i * timeBetweenPoints; 

            // Physics Formula: Origin + (Velocity * time) + (Gravity * time^2 / 2)
            Vector3 point = startPosition + (startVelocity * time) + (Physics.gravity * 0.5f * time * time);
            
            // Now 'i' is guaranteed to be safe
            lineRenderer.SetPosition(i, point);
            
            // Collision Check (Optional: Cut the line if it hits the floor)
            if(point.y < 0) 
            {
                lineRenderer.positionCount = i + 1; // Trim the excess points
                break; // Stop drawing
            }
        }
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