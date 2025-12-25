using System;
using UnityEngine;

public abstract class CheatMethod : MonoBehaviour
{

    public float timeToCheat; //How long it takes to complete the cheat
    public int cheatPoints = 1; //How many points this cheat gives
    private float timePassed = 0f;
    private bool isCheating = false;
    private ProgressBar cheatBar;

    public static bool isPlayerCheating = false; // Static variable to track if the player is cheating overall

    public event EventHandler CheatStarted;
    public event EventHandler CheatFailed;
    public event EventHandler CheatCompleted;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    protected void Start()
    {
        // Show cheat UI on start cheating
        CheatStarted += GameManager.Instance.OnCheatStarted;
        CheatFailed += GameManager.Instance.OnCheatFailed;
        CheatCompleted += GameManager.Instance.OnCheatCompleted;
        cheatBar = GameManager.Instance.cheatBar;
        
    }

    // Update is called once per frame
    protected void Update()
    {
        if (isCheating)
        {
            timePassed += Time.deltaTime;

            cheatBar.SetValue(timePassed);
            
            if (timePassed >= timeToCheat)
            {
                isCheating = false;
                isPlayerCheating = false;
                timePassed = 0f;
                CheatCompleted?.Invoke(this, EventArgs.Empty);
            }
        }
    }

    public float GetTimePassed()
    {
        return timePassed;
    }

    public void StartCheating()
    {
        isCheating = true;
        isPlayerCheating = true;
        cheatBar.SetMax(timeToCheat);
        CheatStarted?.Invoke(this, EventArgs.Empty);
    }

    public void StopCheating()
    {
        if (!isCheating) return;

        isCheating = false;
        isPlayerCheating = false;
        timePassed = 0f;
        CheatFailed?.Invoke(this, EventArgs.Empty);
    }


    public abstract void OnLook();
    public abstract void OnUnlook();
    public abstract void OnClick();
    public abstract void OnUnclick();
}
