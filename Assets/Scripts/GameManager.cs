using UnityEngine;
using System;

public class GameManager : MonoBehaviour
{

    public Timer gameTimer;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        gameTimer.TimeExpired += OnTimeExpired;
        gameTimer.StartTimer();
    }

    // Update is called once per frame
    void Update()
    {

    }
    
    private void OnTimeExpired(object sender, EventArgs e)
    {
        Debug.Log("Game Over! Time has expired.");
    }
}
