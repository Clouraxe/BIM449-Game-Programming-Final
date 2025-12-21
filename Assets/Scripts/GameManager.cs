using UnityEngine;
using System;

public class GameManager : MonoBehaviour
{

    public Timer gameTimer;
    public TaskBarUI taskBarUI;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        gameTimer.TimeExpired += OnTimeExpired;
        taskBarUI.TaskBarCompleted += OnTaskBarCompleted;
        gameTimer.StartTimer();
    }
    
    void FinishTask()
    {
        taskBarUI.SetValue(taskBarUI.GetValue() + 1);
    }

    private void OnTimeExpired(object sender, EventArgs e)
    {
        Debug.Log("Game Over! Time has expired.");
    }
    
    void OnTaskBarCompleted(object sender, EventArgs e)
    {
        Debug.Log("You won!");
    }
}
