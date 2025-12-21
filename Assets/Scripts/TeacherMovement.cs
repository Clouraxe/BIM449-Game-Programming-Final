using UnityEngine;
using System.Collections.Generic;

public class TeacherMovement : MonoBehaviour
{
    public RouteCalculator routeGrid;
    public Transform patrolPointsParent;

    public float moveSpeed = 2f;
    public float turnSpeed = 8f;
    public float arriveDistance = 0.2f;
    public float waitTimeAtPoint = 1f;

    public bool canMove = true;

    Animator anim;

    Transform[] points;
    Queue<Vector3> segmentTargets = new Queue<Vector3>();
    float waitTimer = 0f;

    public bool IsWaiting { get; private set; }
    public int CurrentWaypoint1Based { get; private set; }  // 1..12

    void Start()
    {
        IsWaiting = true;
        CurrentWaypoint1Based = 1; // başlangıç varsayımı
    }

    void Awake()
    {
        anim = GetComponent<Animator>();

        int count = patrolPointsParent.childCount;
        points = new Transform[count];
        for (int i = 0; i < count; i++)
            points[i] = patrolPointsParent.GetChild(i);

        System.Array.Sort(points, (a, b) => a.name.CompareTo(b.name));
    }

    void Update()
    {
        if (!canMove)
        {
            SetWalking(false);
            return;
        }

        //Sleep timer
        if (segmentTargets.Count == 0)
        {
            IsWaiting = true;
            SetWalking(false);

            if (CurrentWaypoint1Based % 3 == 1)
            {
                RotateToBoard();
            }

            waitTimer += Time.deltaTime;
            if (waitTimer < waitTimeAtPoint)
                return;

            waitTimer = 0f;

            int nextIdx0 = routeGrid.PickNextPointIndex0Based();
            CurrentWaypoint1Based = nextIdx0 + 1;

            Vector3 final = points[nextIdx0].position;
            BuildLPath(final);

            IsWaiting = false;
            return;
        }

        //Movement Control
        Vector3 target = segmentTargets.Peek();
        target.y = transform.position.y;

        float dist = Vector3.Distance(transform.position, target);
        if (dist <= arriveDistance)
        {
            segmentTargets.Dequeue();
            return;
        }

        IsWaiting = false;
        SetWalking(true);

        Vector3 dir = target - transform.position;
        dir.y = 0f;

        if (dir.sqrMagnitude > 0.0001f)
        {
            Quaternion rot = Quaternion.LookRotation(dir.normalized);
            transform.rotation = Quaternion.Slerp(transform.rotation, rot, turnSpeed * Time.deltaTime);
        }

        transform.position = Vector3.MoveTowards(
            transform.position,
            target,
            moveSpeed * Time.deltaTime
        );
    }
    //L path
    void BuildLPath(Vector3 final)
    {
        Vector3 start = transform.position;
        final.y = start.y;

        bool sameX = Mathf.Abs(final.x - start.x) < 0.001f;
        bool sameZ = Mathf.Abs(final.z - start.z) < 0.001f;

        if (sameX || sameZ)
        {
            segmentTargets.Enqueue(final);
            return;
        }

        bool horizFirst = Random.value < 0.5f;

        Vector3 cornerA = new Vector3(final.x, start.y, start.z);
        Vector3 cornerB = new Vector3(start.x, start.y, final.z);

        if (horizFirst)
        {
            segmentTargets.Enqueue(cornerA);
            segmentTargets.Enqueue(final);
        }
        else
        {
            segmentTargets.Enqueue(cornerB);
            segmentTargets.Enqueue(final);
        }
    }

    //Animation and Rotation
    void SetWalking(bool walking)
    {
        if (anim != null)
            anim.SetBool("IsWalking", walking);
    }

    //Rotates to the students
    void RotateToBoard()
{
    Quaternion targetRot = Quaternion.Euler(0f, 90f, 0f);
    transform.rotation = Quaternion.RotateTowards(
        transform.rotation,
        targetRot,
        360f * Time.deltaTime  // derece/sn
    );
}

}
