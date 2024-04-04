using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BeauRoutine;

public class GameManager : MonoBehaviour
{
    // 'bigger' actions:
    public static Action OnGameStart;
    public static Action OnGameReset;
    public static Action ReadyGameplayLoop;
    public static Action OnGameWin;
    public static Action OnGameOver;

    // OnBallLost: reset actions, ready actions, allow player input
    public static bool AllowInput;
    public static Action OnBallLost;
    public static Action OnReset;
    public static Action OnReady;

    int lives = 0;

    private void Start()
    {
        AllowInput = false;
        OnGameStart += SetLives;
        ReadyGameplayLoop += SetGameplayLoop;
        OnBallLost += LifeLost;

        Routine.Start(LateStart());
    }
    
    IEnumerator LateStart()
    {
        yield return new WaitForEndOfFrame();

        OnGameStart?.Invoke();
    }

    void SetLives()
    {
        lives = GameProperties.StartingHealthAmount;
    }

    void SetGameplayLoop()
    {
        AllowInput = false;
        OnReset?.Invoke();
        OnReady?.Invoke();
        AllowInput = true;
    }

    void LifeLost()
    {
        lives--;
        if (lives <= 0)
        {
            // DEAD
            AllowInput = false;
            OnGameOver?.Invoke();
        }
        else SetGameplayLoop();
    }
}
