using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class TeacherMovement : MonoBehaviour
{
    public RouteCalculator routeGrid;
    public ScreenAlert screenAlert;
    public Transform student;
    public CameraController studentCamera;
    public LifeUIController lifeUI;
    public Transform patrolPointsParent;

    public float moveSpeed = 2f;
    public float turnSpeed = 8f;
    public float arriveDistance = 0.2f;
    public float waitTimeAtPoint = 1f;

    public int life = 3;
    public bool canMove = true;

    Animator anim;

    Transform[] points;
    Queue<Vector3> segmentTargets = new Queue<Vector3>();
    float waitTimer = 0f;

    public bool IsWaiting { get; private set; }
    public int CurrentWaypoint1Based { get; private set; }

    public enum TeacherState
    {
        Patrolling,
        Suspicious
    }

    public TeacherState State { get; private set; } = TeacherState.Patrolling;

    Transform suspiciousTarget;

    public bool alreadyCaught = false;     // öğrenci dokunulmazlık
    bool handlingCatch = false;     // aynı yakalamayı tekrar tetiklemesin

    void Awake()
    {
        anim = GetComponent<Animator>();

        int count = patrolPointsParent.childCount;
        points = new Transform[count];
        for (int i = 0; i < count; i++)
            points[i] = patrolPointsParent.GetChild(i);

        System.Array.Sort(points, (a, b) => a.name.CompareTo(b.name));
    }

    void Start()
    {
        IsWaiting = true;
        CurrentWaypoint1Based = 1;
    }

    void Update()
    {
        // 🟥 ŞÜPHELİ DURUM
        if (State == TeacherState.Suspicious)
        {
            LookAtStudent();
            studentCamera.LookAtTeacher(transform);

            if (!handlingCatch && !alreadyCaught && life > 0)
            {
                handlingCatch = true;
                StartCoroutine(CatchSequence());
            }

            return;
        }

        // 🟦 YÜRÜME KONTROLÜ
        if (!canMove)
        {
            SetWalking(false);
            return;
        }

        if (segmentTargets.Count == 0)
        {
            IsWaiting = true;
            SetWalking(false);

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

        Vector3 target = segmentTargets.Peek();
        target.y = transform.position.y;

        float dist = Vector3.Distance(transform.position, target);
        if (dist <= arriveDistance)
        {
            segmentTargets.Dequeue();
            return;
        }

        SetWalking(true);

        Vector3 dir = target - transform.position;
        dir.y = 0f;

        if (dir.sqrMagnitude > 0.001f)
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

    public bool studentFreeze = false;
    // 🧠 YAKALANMA AKIŞI
    IEnumerator CatchSequence()
    {
        studentFreeze = true;
        // can düş
        lifeUI.LoseLife();
        life--;

        StartCoroutine(screenAlert.AlertRoutine());

        // hoca bakarak 3 saniye beklesin
        canMove = false;
        yield return new WaitForSeconds(3f);
        studentFreeze = false;

        // tekrar yürüsün
        State = TeacherState.Patrolling;
        suspiciousTarget = null;
        canMove = true;

        // 3 saniye dokunulmazlık
        alreadyCaught = true;
        yield return new WaitForSeconds(3f);
        alreadyCaught = false;

        handlingCatch = false;
    }

    // 📐 L PATH
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

    void SetWalking(bool walking)
    {
        if (anim != null)
            anim.SetBool("IsWalking", walking);
    }

    public void SetSuspicious(Transform target)
    {
        if (alreadyCaught) return;

        State = TeacherState.Suspicious;
        suspiciousTarget = target;
        canMove = false;
        SetWalking(false);
    }
    public bool CanSeeStudent(Transform student)
    {
        Vector3 toStudent = student.position - transform.position;
        toStudent.y = 0f;

        float angle = Vector3.Angle(transform.forward, toStudent);
        if (angle > 45f) return false; // görüş açısı

        float dist = toStudent.magnitude;
        if (dist > 6f) return false; // görüş mesafesi

        return true;
    }
    void LookAtStudent()
    {
        if (suspiciousTarget == null) return;

        Vector3 dir = suspiciousTarget.position - transform.position;
        dir.y = 0f;

        if (dir.sqrMagnitude < 0.001f) return;

        Quaternion rot = Quaternion.LookRotation(dir.normalized);
        transform.rotation = Quaternion.Slerp(
            transform.rotation,
            rot,
            turnSpeed * Time.deltaTime
        );
    }
}

