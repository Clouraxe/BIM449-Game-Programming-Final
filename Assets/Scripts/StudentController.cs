using UnityEngine;

public class StudentLean : MonoBehaviour
{
    [Header("Limits")]
    public float maxHorizontal = 0.18f; // A-D
    public float maxVertical = 0.12f;   // W-S

    [Header("Speed")]
    public float moveSpeed = 6f;
    public float returnSpeed = 10f;

    float leanX; // A-D  (-1..1)
    float leanY; // W-S  (-1..1)

    Vector3 startLocalPos;

    public TeacherMovement teacher;

    public bool isLeaning;

    void Start()
    {
        startLocalPos = transform.localPosition;
    }

    void Update()
    {
        

        float inputX = 0f;
        if (Input.GetKey(KeyCode.A)) inputX -= 1f;
        if (Input.GetKey(KeyCode.D)) inputX += 1f;

        float inputY = 0f;
        if (Input.GetKey(KeyCode.W)) inputY += 1f;
        if (Input.GetKey(KeyCode.S)) inputY -= 1f;

        

        // Yumuşatma
        leanX = Mathf.MoveTowards(
            leanX,
            inputX,
            ((Mathf.Abs(inputX) > 0.01f) ? moveSpeed : returnSpeed) * Time.deltaTime
        );

        leanY = Mathf.MoveTowards(
            leanY,
            inputY,
            ((Mathf.Abs(inputY) > 0.01f) ? moveSpeed : returnSpeed) * Time.deltaTime
        );

        // 🔥 DOĞRU eksenler
        Vector3 targetLocalPos =
            startLocalPos +
            Vector3.forward * (leanX * maxHorizontal) +   
            -Vector3.right * (leanY * maxVertical);     

        transform.localPosition = Vector3.Lerp(
            transform.localPosition,
            targetLocalPos,
            0.25f
        );

        if (Mathf.Abs(leanX) > 0.1f || Mathf.Abs(leanY) > 0.1f) isLeaning = true;
        else isLeaning = false;

        if (isLeaning && teacher.CanSeeStudent(transform))
        {
            teacher.SetSuspicious(transform);
        }
    }

    // Teacher için
    public float LeanAmount01 =>
        Mathf.Clamp01(Mathf.Abs(leanX) + Mathf.Abs(leanY));
}
