using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class LifeUIController : MonoBehaviour
{
    [Header("Center Card")]
    public RectTransform centerCard;   // Ortada çıkan kart
    public Image centerCross;           // Ortadaki çarpı

    [Header("Top Right UI")]
    public Image[] hearts;              // Sağ üstteki 3 kalp / bar
    public Image[] crosses;             // Sağ üstteki 3 çarpı

    [Header("Animation")]
    public float scaleInTime = 0.25f;
    public float showTime = 0.4f;
    public float moveTime = 0.6f;
    public float endScale = 0.6f;

    int currentLife = 3;
    int lostCount = 0;

    void Start()
    {
        // Ortadaki kart görünmez başlasın
        centerCard.localScale = Vector3.zero;
        centerCross.enabled = false;

        // Sağ üstteki çarpılar kapalı başlasın
        foreach (var c in crosses)
            c.enabled = false;
    }

    // 🧠 DIŞARIDAN ÇAĞIRACAĞIN FONKSİYON
    public void LoseLife()
    {
        if (currentLife <= 0) return;

        StartCoroutine(LifeLostRoutine(lostCount));

        lostCount++;
        currentLife--;
    }

    IEnumerator LifeLostRoutine(int lifeIndex)
    {
        // 1️⃣ Ortada büyüyerek gelsin
        yield return ScaleTo(centerCard, Vector3.one, scaleInTime);

        // 2️⃣ Ortadaki çarpıyı göster
        centerCross.enabled = true;
        yield return new WaitForSeconds(showTime);

        // 3️⃣ Sağ üstteki hedef pozisyon
        RectTransform targetHeart = hearts[lifeIndex].rectTransform;

        yield return MoveTo(centerCard, targetHeart.anchoredPosition, moveTime);

        // 4️⃣ Küçül
        yield return ScaleTo(centerCard, Vector3.one * endScale, 0.2f);

        // 5️⃣ Sağ üstte kalıcı çarpıyı aç
        crosses[lifeIndex].enabled = true;

        // 6️⃣ Ortadaki kartı sıfırla
        centerCross.enabled = false;
        centerCard.localScale = Vector3.zero;
    }

    IEnumerator ScaleTo(RectTransform rt, Vector3 target, float duration)
    {
        Vector3 start = rt.localScale;
        float t = 0f;

        while (t < 1f)
        {
            t += Time.deltaTime / duration;
            rt.localScale = Vector3.Lerp(start, target, Mathf.SmoothStep(0, 1, t));
            yield return null;
        }
    }

    IEnumerator MoveTo(RectTransform rt, Vector2 target, float duration)
    {
        Vector2 start = rt.anchoredPosition;
        float t = 0f;

        while (t < 1f)
        {
            t += Time.deltaTime / duration;
            rt.anchoredPosition =
                Vector2.Lerp(start, target, Mathf.SmoothStep(0, 1, t));
            yield return null;
        }
    }
}
