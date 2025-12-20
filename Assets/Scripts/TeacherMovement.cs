using UnityEngine;

public class TeacherMovement : MonoBehaviour
{
    public Transform patrolPointsParent;

    [Header("Route order (1-based)")]

    public float moveSpeed = 2f;
    public float turnSpeed = 8f;
    public float arriveDistance = 0.3f;
    public float waitTimeAtPoint = 1f;

    public RouteCalculator routeGrid;

    Transform[] points;     // P1,P2,P3...
    int routeIdx = 0;
    float waitTimer = 0f;

    Animator anim;



    void Awake()
    {
        anim = GetComponent<Animator>();

        int count = patrolPointsParent.childCount;
        points = new Transform[count];
        for (int i = 0; i < count; i++)
            points[i] = patrolPointsParent.GetChild(i);

    }

    void Update()
    {
        if (points == null || points.Length == 0) return;

        SetWalking(false);

        Vector3 target = points[routeIdx].position;
        target.y = transform.position.y;

        float dist = Vector3.Distance(transform.position, target);

        if (dist <= arriveDistance)
        {
            SetWalking(false);


            waitTimer += Time.deltaTime;
            if (waitTimer >= waitTimeAtPoint)
            {
                routeIdx = routeGrid.PickNextPointIndex0Based();
                waitTimer = 0f;
            }
            return;
        }

        SetWalking(true);

        Vector3 dir = (target - transform.position).normalized;
        if (dir.sqrMagnitude > 0.0001f)
        {
            Quaternion rot = Quaternion.LookRotation(dir);
            transform.rotation = Quaternion.Slerp(transform.rotation, rot, turnSpeed * Time.deltaTime);
        }

        transform.position = Vector3.MoveTowards(transform.position, target, moveSpeed * Time.deltaTime);
    }

    void SetWalking(bool walking)
    {
        if (anim != null) anim.SetBool("IsWalking", walking);
    }


}
