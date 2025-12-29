using UnityEngine;

public class CheatPhone : CheatMethod
{
    private Color[] originalColors; // Orijinal renkleri saklar
    [SerializeField] private Transform phoneModel;

    [SerializeField] private Transform phoneHoldPoint;
    private Vector3 initialPosition;

    [Header("Detection Settings")]
    public TeacherMovement teacher;   // Hoca Scripti
    public Transform studentPlayer;   // Oyuncu karakterinin kendisi (Hocanın baktığı hedef)

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    new void Start()
    {
        base.Start();
        originalColors = new Color[phoneModel.childCount];
        initialPosition = transform.position;

        // Hocayı sahnede otomatik bul (Eğer elle atamadıysan)
        if (teacher == null) teacher = FindFirstObjectByType<TeacherMovement>();

        // Eğer öğrenci atanmadıysa, bu scriptin en üst ebeveynini (Root) öğrenci varsay
        if (studentPlayer == null) studentPlayer = transform.root;
    }

    // Update is called once per frame
    new void Update()
    {
        base.Update();

        // 🟥 YAKALANMA MANTIĞI BURADA 🟥
        // 1. Eğer şu an kopya çekiyorsak (Base class'taki değişken true ise)
        // 2. Ve hoca bizi görebiliyorsa
        if (isPlayerCheating && teacher != null)
        {
            if (teacher.CanSeeStudent(studentPlayer))
            {
                Debug.Log("Telefona bakarken yakalandın!");

                // Hocayı şüpheli moduna sok ve bizi yakalasın
                teacher.SetSuspicious(studentPlayer);

                // İsteğe bağlı: Yakalanınca telefon hemen kapansın
                OnUnclick();
            }
        }
    }

    public override void OnLook()
    {
        // Eğer zaten kopya çekiyorsak highlight yapmaya gerek yok
        if (isPlayerCheating) return;

        for (int i = 0; i < phoneModel.childCount; i++)
        {
            Transform child = phoneModel.GetChild(i);
            if (child.TryGetComponent(out Renderer rend))
            {
                originalColors[i] = rend.material.color;
                Color highlightColor = Color.yellow;
                rend.material.color = highlightColor;
            }
        }
    }

    public override void OnUnlook()
    {
        for (int i = 0; i < phoneModel.childCount; i++)
        {
            Transform child = phoneModel.GetChild(i);
            if (child.TryGetComponent(out Renderer rend))
            {
                // Renkleri kaydettiğimiz diziden geri yükle
                // (Hata almamak için dizi kontrolü ekledim)
                if (originalColors != null && i < originalColors.Length)
                {
                    Color originalColor = originalColors[i];
                    rend.material.color = originalColor;
                }
            }
        }
    }

    public override void OnClick()
    {
        transform.position = phoneHoldPoint.position;
        transform.eulerAngles = phoneHoldPoint.eulerAngles;
        StartCheating(); // Base class'taki isPlayerCheating'i true yapar
    }

    public override void OnUnclick()
    {
        StopCheating(); // Base class'taki isPlayerCheating'i false yapar
        transform.position = initialPosition;
        transform.eulerAngles = Vector3.zero;
    }
}