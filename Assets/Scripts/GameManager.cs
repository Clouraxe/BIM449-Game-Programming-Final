using UnityEngine;
using System;

public class GameManager : MonoBehaviour
{
    
    public static GameManager Instance { get; private set; } //Singleton
        

    public Timer gameTimer;
    public ProgressBar taskBar;
    public ProgressBar cheatBar;
    
    
    void Awake() //Singleton creation
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    } 
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        gameTimer.TimeExpired += OnTimeExpired;
        taskBar.ProgressCompleted += OnTaskBarCompleted;
        gameTimer.StartTimer();
        
        cheatBar.transform.parent.gameObject.SetActive(false);
    }
    
    void FinishTask()
    {
        taskBar.SetValue(taskBar.GetValue() + 1);
    }

    private void OnTimeExpired(object sender, EventArgs e)
    {
        Debug.Log("Game Over! Time has expired.");
    }

    void OnTaskBarCompleted(object sender, EventArgs e)
    {
        Debug.Log("You won!");
    }


    public void OnCheatStarted(object sender, EventArgs e)
    {
        cheatBar.transform.parent.gameObject.SetActive(true);
    }

    public void OnCheatFailed(object sender, EventArgs e)
    {
        cheatBar.transform.parent.gameObject.SetActive(false);
        cheatBar.SetValue(0);
    }
    
    public void OnCheatCompleted(object sender, EventArgs e)
    {
        Debug.Log("Cheat Completed Successfully!");
        OnCheatFailed(this, EventArgs.Empty);
        
        FinishTask();
    }
}
