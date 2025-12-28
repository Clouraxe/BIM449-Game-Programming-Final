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
    private Renderer clickedObject;

    [Header("Trajectory Settings")]
    [SerializeField] private LineRenderer lineRenderer;
    [SerializeField] private int linePoints = 25;
    [SerializeField] private float timeBetweenPoints = 0.1f;
    [SerializeField] private float throwForce = 15f; 

    private Color[] originalColors; 

    void Awake()
    {
        cam = GetComponent<Camera>();
    }

    void Update()
    {
        // --- 1. Holding Logic (Eşya Tutma Mantığı) ---
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
            if (Input.GetKeyDown(KeyCode.E))
            {
                DropObject();
            }
            return;
        }
        else
        {
            if (lineRenderer != null) lineRenderer.enabled = false;
        }

        // --- 2. Raycast Logic (Bakış Mantığı) ---
        Ray ray = new Ray(cam.transform.position, cam.transform.forward);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, interactRange, interactLayer))
        {
            if (hit.collider.TryGetComponent<Renderer>(out Renderer target))
            {
                if (selectedObject != target)
                {
                    SelectObject(target);
                    if (target.TryGetComponent(out CheatMethod cheatMethodd)) cheatMethodd.OnLook(); 
                }

                // --- TUTMA (GRAB) KISMI ---
                if ((Input.GetMouseButtonDown(0) || Input.GetKeyDown(KeyCode.E)) && hit.collider.CompareTag("grabbable"))
                {
                    GrabObject(hit.collider.gameObject);
                }

                if (target.TryGetComponent(out CheatMethod cheatMethod))
                {
                    if (Input.GetMouseButtonDown(0))
                    {
                        clickedObject = target;
                        cheatMethod.OnClick(); 
                    }
                }
            }
            else ClearSelection();
        }
        else ClearSelection();

        if (Input.GetMouseButtonUp(0) && clickedObject != null) 
        {
            if (clickedObject.TryGetComponent(out CheatMethod cheatMethod))
            {
                cheatMethod.OnUnclick(); 
                clickedObject = null;
            }
        }
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
        ClearSelection(); 

        // --- ALMA SESİ KISMI ---
        PlayInteractionSound(obj);
        // -----------------------
    }

    void DropObject()
    {
        if (grabbedObject == null) return;

        // --- BIRAKMA SESİ KISMI (Yeni Eklenen) ---
        PlayInteractionSound(grabbedObject);
        // -----------------------------------------

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

        // --- FIRLATMA SESİ KISMI (Yeni Eklenen) ---
        PlayInteractionSound(grabbedObject);
        // ------------------------------------------

        Rigidbody rb = grabbedObject.GetComponent<Rigidbody>();
        Collider col = grabbedObject.GetComponent<Collider>();

        if (rb != null)
        {
            rb.useGravity = true;
            rb.isKinematic = false;
        }

        if (col != null)
        {
            col.enabled = true;
        }

        if (rb != null)
        {
            rb.AddForce(cam.transform.forward * throwForce, ForceMode.Impulse);
        }

        grabbedObject = null;
    }

    // Kod tekrarını önlemek için sesi çalan özel bir yardımcı fonksiyon yazdım
    void PlayInteractionSound(GameObject obj)
    {
        InteractionSound soundScript = obj.GetComponent<InteractionSound>();
        
        if (soundScript != null && soundScript.soundClip != null)
        {
            AudioSource.PlayClipAtPoint(soundScript.soundClip, obj.transform.position, soundScript.volume);
        }
    }

    void DrawProjection()
    {
        if (lineRenderer == null) return; 

        lineRenderer.enabled = true;
        lineRenderer.positionCount = linePoints; 

        Vector3 startPosition = holdPoint.position;
        Vector3 startVelocity = cam.transform.forward * throwForce;

        for (int i = 0; i < linePoints; i++)
        {
            float time = i * timeBetweenPoints;
            Vector3 point = startPosition + (startVelocity * time) + (Physics.gravity * 0.5f * time * time);
            lineRenderer.SetPosition(i, point);

            if (point.y < 0)
            {
                lineRenderer.positionCount = i + 1; 
                break; 
            }
        }
    }

    void SelectObject(Renderer obj)
    {
        ClearSelection(); 

        selectedObject = obj;

        originalColors = new Color[selectedObject.materials.Length];
        for (int i = 0; i < selectedObject.materials.Length; i++)
        {
            originalColors[i] = selectedObject.materials[i].color;
        }

        foreach (Material mat in selectedObject.materials)
        {
            mat.color = new Color(1f, 1f, 0f, 0.5f); 
        }
    }

    void ClearSelection()
    {
        if (selectedObject == null) return;

        for (int i = 0; i < selectedObject.materials.Length; i++)
        {
            if (originalColors != null && i < originalColors.Length)
            {
                selectedObject.materials[i].color = originalColors[i];
            }
        }

        if (selectedObject.TryGetComponent(out CheatMethod cheatMethod)) cheatMethod.OnUnlook(); 

        selectedObject = null;
    }   
}