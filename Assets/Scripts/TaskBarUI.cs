using System;
using UnityEngine;

public class TaskBarUI : MonoBehaviour
{

    [SerializeField] private float tasksCompleted, totalTasks;
    private float width, height;


    [SerializeField] private RectTransform _barRect;
    
    public event EventHandler TaskBarCompleted;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        width = _barRect.sizeDelta.x;
        height = _barRect.sizeDelta.y;
        UpdateBar();
    }
    
    public void SetMax(float tasks)
    {
        totalTasks = tasks;
        UpdateBar();
    }

    public void SetValue(float tasks)
    {
        tasksCompleted = tasks;
        UpdateBar();
    }
    
    public float GetValue()
    {
        return tasksCompleted;
    }

    private void UpdateBar()
    {
        if (totalTasks > 0 && tasksCompleted <= totalTasks)
        {
            _barRect.sizeDelta = new Vector2(tasksCompleted / totalTasks * width, height);
        }

        if (tasksCompleted >= totalTasks)
        {
            TaskBarCompleted?.Invoke(this, EventArgs.Empty);
        }
    }
}