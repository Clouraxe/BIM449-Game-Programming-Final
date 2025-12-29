using UnityEngine;
using System;
using System.Collections;

public class GameManager : MonoBehaviour
{
    
    public static GameManager Instance { get; private set; } //Singleton
        

    public Timer gameTimer;
    public ProgressBar taskBar;
    public ProgressBar cheatBar;
    public RectTransform gameOverPanel;
    public RectTransform winPanel;
    public AudioClip winClip;
    public AudioClip gameOverClip;
    public AudioClip caughtSfx;
    public AudioClip cheatSuccessSfx;
    public AudioClip cheatingSfx;

    public static bool isGameOver = false;
    
    private AudioSource audioSource;
    
    
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
        audioSource = GetComponent<AudioSource>();

        gameOverPanel.gameObject.SetActive(false);
        winPanel.gameObject.SetActive(false);
        cheatBar.transform.parent.gameObject.SetActive(false);
        isGameOver = false;
    }
    
    void FinishTask(int points = 1)
    {
        AudioSource.PlayClipAtPoint(cheatSuccessSfx, Camera.main.transform.position);
        taskBar.SetValue(taskBar.GetValue() + points);
    }

    private void OnTimeExpired(object sender, EventArgs e)
    {
        Debug.Log("Game Over! Time has expired.");
        Gameover();
    }

    public void RestartGame() //Called by UI Button
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene(UnityEngine.SceneManagement.SceneManager.GetActiveScene().buildIndex);
    }
    
    public void OnBackToMenu() //Called by UI Button
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene("MainMenu");
    }

    void OnTaskBarCompleted(object sender, EventArgs e) // Won the game
    {
        if (isGameOver) return;

        winPanel.gameObject.SetActive(true);
        Cursor.lockState = CursorLockMode.None; //FREE THE MOUSE
        Cursor.visible = true;
        audioSource.Stop();
        AudioSource.PlayClipAtPoint(winClip, Camera.main.transform.position);
        isGameOver = true;


        gameTimer.StopTimer();
    }
    
    public void Gameover()
    {
        if (isGameOver) return;
        gameOverPanel.gameObject.SetActive(true);
        Cursor.lockState = CursorLockMode.None; //FREE THE MOUSE
        Cursor.visible = true;
        audioSource.Stop();
        AudioSource.PlayClipAtPoint(gameOverClip, Camera.main.transform.position);
        isGameOver = true;
        
        gameTimer.StopTimer();
    }


    public void OnCheatStarted(object sender, EventArgs e)
    {
        StartCoroutine(PlayCheatingSoundRoutine());
        cheatBar.transform.parent.gameObject.SetActive(true);
    }

    public void OnCheatFailed(object sender, EventArgs e)
    {
        cheatBar.transform.parent.gameObject.SetActive(false);
        cheatBar.SetValue(0);
    }

    public void OnCheatCompleted(object sender, EventArgs e)
    {
        OnCheatFailed(this, EventArgs.Empty);

        FinishTask(((CheatMethod)sender).cheatPoints);
    }


    private IEnumerator PlayCheatingSoundRoutine()
    {
        while (CheatMethod.isPlayerCheating)
        {
            AudioSource.PlayClipAtPoint(cheatingSfx, Camera.main.transform.position);
            yield return new WaitForSeconds(cheatingSfx.length + 0.2f);
        }
    }
    
    public void PlayCaughtSound()
    {
        AudioSource.PlayClipAtPoint(caughtSfx, Camera.main.transform.position);
    }
}
