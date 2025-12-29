using UnityEngine;

public class EraserDistraction : MonoBehaviour
{
    private TeacherMovement teacher;
    private bool isThrown = false;
    private bool hasLanded = false;

    void Start()
    {
        // 2023 ve sonrası için FindFirstObjectByType, eskisi için FindObjectOfType
        teacher = Object.FindFirstObjectByType<TeacherMovement>();
    }

    public void Throw()
    {
        isThrown = true;
        hasLanded = false;
        Debug.Log("✅ SİLGİ FIRLATILDI! (Çarpışma bekleniyor...)");
    }

    void OnCollisionEnter(Collision collision)
    {
        // ÖNCE BUNU GÖRELİM: Neye çarptı?
        Debug.Log("💥 ÇARPIŞMA OLDU! Çarpılan Obje: " + collision.gameObject.name);

        if (!isThrown) return; // Fırlatılmadıysa işlem yapma

        // Eğer Hoca yoksa
        if (teacher == null)
        {
            Debug.LogError("❌ HATA: Sahnede TeacherMovement bulunamadı!");
            return;
        }

        // Eğer zaten yere değdiyse
        if (hasLanded) return;

        // EĞER OYUNCUYA ÇARPTIYSAN (Sorun muhtemelen bu)
        if (collision.gameObject.CompareTag("Player"))
        {
            Debug.LogWarning("⚠️ UYARI: Silgi kendi karakterine çarptı! HoldPoint'i biraz ileri al.");
            // Return ETMİYORUM ki çalıştığını gör. Normalde return; olmalı.
            // Test için hocayı yine de çağıralım:
        }

        hasLanded = true;
        isThrown = false;

        Debug.Log("📢 HOCA ÇAĞIRILIYOR -> Pozisyon: " + transform.position);
        teacher.InvestigateNoise(transform.position);
    }
}