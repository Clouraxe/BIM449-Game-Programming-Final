using System;
using UnityEngine;

public class ProgressBar : MonoBehaviour
{

    [SerializeField] private float progress, max;
    private float width, height;


    [SerializeField] private RectTransform _fillRect;
    
    public event EventHandler ProgressCompleted;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        width = _fillRect.parent.GetComponent<RectTransform>().sizeDelta.x;
        height = _fillRect.parent.GetComponent<RectTransform>().sizeDelta.y;
        UpdateBar();
    }
    
    public void SetMax(float value)
    {
        max = value;
        UpdateBar();
    }

    public void SetValue(float value)
    {
        progress = value;
        UpdateBar();
    }
    
    public float GetValue()
    {
        return progress;
    }

    private void UpdateBar()
    {
        if (max > 0 && progress <= max)
        {
            _fillRect.sizeDelta = new Vector2(progress / max * width, height);
        }

        if (progress >= max)
        {
            ProgressCompleted?.Invoke(this, EventArgs.Empty);
        }
    }
}