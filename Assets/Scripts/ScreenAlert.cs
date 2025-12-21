using UnityEngine;
using UnityEngine.UI;

public class ScreenAlert : MonoBehaviour
{
    Image img;

    public float fadeSpeed = 2f;
    float targetAlpha = 0f;

    void Awake()
    {
        img = GetComponent<Image>();
        img.color = new Color(0.7f, 0.7f, 0.7f, 0f);
    }

    void Update()
    {
        Color c = img.color;
        c.a = Mathf.MoveTowards(c.a, targetAlpha, fadeSpeed * Time.deltaTime);
        img.color = c;
    }

    public void ShowAlert()
    {
        targetAlpha = 0.4f;
    }

    public void HideAlert()
    {
        targetAlpha = 0f;
    }
}
