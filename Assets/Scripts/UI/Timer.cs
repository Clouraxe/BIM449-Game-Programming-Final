using System;
using UnityEngine;
using UnityEngine.UI;

public class Timer : MonoBehaviour
{
    public float startTime = 60.0f;
    private float _timeLeft;
    public Text timerText;
    
    private bool isRunning = false;
    public bool IsRunning { get { return isRunning; } private set { isRunning = value; } }

    public event EventHandler TimeExpired;

    public float TimeLeft
    {
        get { return _timeLeft; }

        set
        {
            _timeLeft = value;
            timerText.text = "Time\n" + Mathf.CeilToInt(_timeLeft).ToString();
        }
    }
    
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        timerText = GetComponent<Text>();
        TimeLeft = startTime;
    }

    // Update is called once per frame
    void Update()
    {
        if (isRunning)
        {
            if (TimeLeft > 0)
            {
                TimeLeft -= Time.deltaTime;
            }
            else
            {
                TimeLeft = 0;
                isRunning = false;
                TimeOut();
            }
        }
    }


    private void TimeOut()
    {
        TimeExpired?.Invoke(this, EventArgs.Empty);
        
        timerText.color = Color.red;
    }

    public void StartTimer()
    {
        IsRunning = true;
    }
    
    public void StopTimer()
    {
        IsRunning = false;
    }
    
}
