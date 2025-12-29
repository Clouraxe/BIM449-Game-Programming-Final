using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class TeacherMovement : MonoBehaviour
{
    // --- YENİ EKLENEN DEĞİŞKENLER (EN BAŞA) ---
    [Header("Investigation Settings")]
    public float distractionTime = 5f; // Silgiye kaç saniye baksın?
    private bool isInvestigating = false;
    private Vector3 distractionPoint;
    private float investigationTimer = 0f;
    // ------------------------------------------

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

        // 🟨 SİLGİ (INVESTIGATION) MANTIĞI [BURASI YENİ EKLENDİ]
        // -----------------------------------------------------
        if (isInvestigating)
        {
            // Yürürken bile silginin olduğu yere dönmeye çalış
            RotateTowardsPosition(distractionPoint);

            // Eğer hedefe vardıysak (yol bittiyse)
            if (segmentTargets.Count == 0)
            {
                SetWalking(false);
                investigationTimer += Time.deltaTime;

                // Belirlenen süre kadar bekle
                if (investigationTimer > distractionTime)
                {
                    // Süre bitti, devriyeye dön
                    isInvestigating = false;
                    investigationTimer = 0f;
                    IsWaiting = true; // Devriye kaldığı yerden devam etsin
                }

                return; // Aşağıdaki normal devriye kodlarını çalıştırma
            }
        }
        // -----------------------------------------------------

        // 🟦 NORMAL DEVRİYE MANTIĞI
        // (Buraya '&& !isInvestigating' ekledik ki silgiye gidince araya girmesin)
        if (segmentTargets.Count == 0 && !isInvestigating)
        {
            int nextIdx0 = routeGrid.PickNextPointIndex0Based();
            CurrentWaypoint1Based = nextIdx0 + 1;

            IsWaiting = true;
            SetWalking(false);

            waitTimer += Time.deltaTime;

            // 🟨 YENİ EKLENEN KISIM BAŞLANGICI 🟨
            if (waitTimer < waitTimeAtPoint)
            {
                // Şu an hangi noktada duruyoruz?
                int currentPointIndex = GetCurrentClosestPointIndex();

                // Pointler isme göre sıralı olduğu için
                if (currentPointIndex != 1 || currentPointIndex != 4 || currentPointIndex != 7 || currentPointIndex != 10)
                {
                    RotateTowards(student); // Beklerken öğrenciye dön
                }

                return; // Bekleme süresi bitmediyse çık
            }
            // 🟨 YENİ EKLENEN KISIM SONU 🟨

            waitTimer = 0f;

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

        // Hoca yürürken hedefe dönsün (Hem silgiye hem devriyeye giderken çalışır)
        RotateTowardsPosition(target);

        transform.position = Vector3.MoveTowards(
            transform.position,
            target,
            moveSpeed * Time.deltaTime
        );
    }

    // --- YENİ EKLENEN FONKSİYON: SİLGİ SESİNİ DUYUNCA ÇAĞRILIR ---
    public void InvestigateNoise(Vector3 targetPos)
    {
        if (State == TeacherState.Suspicious) return; // Zaten kovalıyorsa gelmesin

        Debug.Log("Hoca ses duydu!");
        distractionPoint = targetPos;
        isInvestigating = true;
        investigationTimer = 0f;

        IsWaiting = false; // Beklemeyi boz
        waitTimer = 0f;

        // Mevcut rotayı sil ve silgiye yeni yol çiz
        segmentTargets.Clear();
        BuildLPath(distractionPoint);
        SetWalking(true);
    }
    // -------------------------------------------------------------

    // --- ROTASYON YARDIMCISI (Vector3 alan versiyonu) ---
    void RotateTowardsPosition(Vector3 targetPos)
    {
        Vector3 dir = targetPos - transform.position;
        dir.y = 0f;

        if (dir.sqrMagnitude < 0.001f) return;

        Quaternion rot = Quaternion.LookRotation(dir.normalized);

        // ÖNEMLİ: Eğer modelin yan dönüyorsa buradaki -90'ı kullan. 
        // Yan dönmüyorsa aşağıdaki satırı silip sadece 'rot' kullanabilirsin.
        // Quaternion correctedRot = rot * Quaternion.Euler(0, -90, 0); 

        transform.rotation = Quaternion.Slerp(transform.rotation, rot, turnSpeed * Time.deltaTime);
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
        if (angle > 30f) return false; // görüş açısı
        return true;
    }

    void LookAtStudent()
    {
        if (suspiciousTarget == null) return;
        RotateTowardsPosition(suspiciousTarget.position);
    }

    int GetCurrentClosestPointIndex()
    {
        float minDistance = Mathf.Infinity;
        int closestIndex = -1;

        for (int i = 0; i < points.Length; i++)
        {
            float dist = Vector3.Distance(transform.position, points[i].position);
            if (dist < 0.5f && dist < minDistance) // 0.5f tolerans
            {
                minDistance = dist;
                closestIndex = i;
            }
        }
        return closestIndex;
    }

    void RotateTowards(Transform targetObj)
    {
        if (targetObj == null) return;
        RotateTowardsPosition(targetObj.position);
    }
}