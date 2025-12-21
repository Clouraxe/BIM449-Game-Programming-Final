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
            Vector3.forward * (leanX * maxHorizontal) +   // A-D → X
            -Vector3.right * (leanY * maxVertical);     // W-S → Y

        transform.localPosition = Vector3.Lerp(
            transform.localPosition,
            targetLocalPos,
            0.25f
        );
    }

    // Teacher için
    public float LeanAmount01 =>
        Mathf.Clamp01(Mathf.Abs(leanX) + Mathf.Abs(leanY));
}
