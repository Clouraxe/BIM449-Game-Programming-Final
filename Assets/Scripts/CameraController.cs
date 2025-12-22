using System.Collections;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public float rotateSpeed = 2f;
    bool isLocked = false;

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
        if (isLocked) return; // 👈 KİLİT

        rotation.y += Input.GetAxis("Mouse X") * lookSpeed;
        rotation.x += -Input.GetAxis("Mouse Y") * lookSpeed;
        transform.eulerAngles = new Vector3(rotation.x, rotation.y, 0);
    }

    public void LookAtTeacher(Transform teacher)
    {
        if (!isLocked)
            StartCoroutine(LookRoutine(teacher));
    }

    IEnumerator LookRoutine(Transform teacher)
    {
        isLocked = true;

        Vector3 dir = teacher.position - transform.position;
        dir.y = 0f;

        Quaternion targetRot = Quaternion.LookRotation(dir.normalized);
        Quaternion startRot = transform.rotation;

        float t = 0f;
        while (t < 1f)
        {
            t += Time.deltaTime * rotateSpeed;
            transform.rotation = Quaternion.Slerp(startRot, targetRot, t);
            yield return null;
        }

        transform.rotation = targetRot;

        yield return new WaitForSeconds(3f); // 👈 3 SN KİLİTLİ KALSIN

        rotation = transform.eulerAngles; // mouse kontrolü bozulmasın
        isLocked = false;
    }

}
