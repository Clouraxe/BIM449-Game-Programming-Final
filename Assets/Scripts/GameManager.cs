using UnityEngine;
using System;

public class GameManager : MonoBehaviour
{
    
    public static GameManager Instance { get; private set; } //Singleton
        

    public Timer gameTimer;
    public ProgressBar taskBar;
    public ProgressBar cheatBar;
    public RectTransform gameOverPanel;
    
    public static bool isGameOver = false;
    
    
    void Awake() //Singleton creation
    {
        if (Instance == null)
        {
            Instance = this;
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
        
        gameOverPanel.gameObject.SetActive(false);
        cheatBar.transform.parent.gameObject.SetActive(false);
        isGameOver = false;
    }
    
    void FinishTask(int points = 1)
    {
        taskBar.SetValue(taskBar.GetValue() + points);
    }

    private void OnTimeExpired(object sender, EventArgs e)
    {
        Debug.Log("Game Over! Time has expired.");
        Gameover();
    }

    void OnTaskBarCompleted(object sender, EventArgs e)
    {
        Debug.Log("You won!");
    }

    public void RestartGame() //Called by UI Button
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene(UnityEngine.SceneManagement.SceneManager.GetActiveScene().buildIndex);
    }
    
    public void OnBackToMenu() //Called by UI Button
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene("MainMenu");
    }
    
    public void Gameover()
    {
        gameOverPanel.gameObject.SetActive(true);
        Cursor.lockState = CursorLockMode.None; //FREE THE MOUSE
        Cursor.visible = true;
        isGameOver = true;
        
        gameTimer.StopTimer();
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
        
        FinishTask(((CheatMethod)sender).cheatPoints);
    }
}
