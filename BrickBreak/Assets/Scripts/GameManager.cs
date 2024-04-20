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
    public static bool InGame;

    public static Action OnBallLost;
    public static Action OnReset;
    public static Action OnReady;

    int lives = 0;
    bool requiresRestart = false;
    Routine startingGame;

    private void Start()
    {
        InGame = AllowInput = false;
        OnGameStart += SetLives;
        ReadyGameplayLoop += SetGameplayLoop;
        OnBallLost += LifeLost;

        Routine.Start(LateStart());
    }
    
    IEnumerator LateStart()
    {
        yield return new WaitForEndOfFrame();
        // do we need this?
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
        // don't lose if we won
        if (!InGame) return;

        lives--;
        if (lives <= 0)
        {
            // DEAD
            AllowInput = false;
            OnGameOver?.Invoke();
        }
        else
        {
            AudioManager.PlayClip?.Invoke("ballMiss");
            SetGameplayLoop();
        }
    }

    // button actions:
    public void PlayGameButton()
    {
        AudioManager.PlayClip?.Invoke("button");
        if (!startingGame.Exists()) 
            startingGame.Replace(DelayGameStart());
    }

    IEnumerator DelayGameStart()
    {
        ScreenManager.SetScreen?.Invoke("main", false);

        if (requiresRestart) OnGameReset?.Invoke();
        else requiresRestart = true;

        yield return 0.3F;        

        OnGameStart?.Invoke();
        InGame = true;
    }

    public void ToMenuButton()
    {
        AudioManager.PlayClip?.Invoke("button");
        InGame = AllowInput = false;
        ScreenManager.SetScreen?.Invoke("main", true);
    }
}
