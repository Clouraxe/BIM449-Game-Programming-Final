using UnityEngine;

public class CameraController : MonoBehaviour
{

    public float lookSpeed = 3.0f;
    Vector2 rotation;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rotation = transform.eulerAngles;
        
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    // Update is called once per frame
    void Update()
    {
        rotation.y += Input.GetAxis("Mouse X") * lookSpeed;
        rotation.x += -Input.GetAxis("Mouse Y") * lookSpeed;
        transform.eulerAngles = new Vector3(rotation.x, rotation.y, 0);
    }
}
