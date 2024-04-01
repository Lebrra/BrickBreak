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
    public static Action OnGameWin;
    public static Action OnGameOver;

    // OnBallLost: reset actions, ready actions, allow player input
    public static bool AllowInput;
    public static Action OnBallLost;
    public static Action OnReset;
    public static Action OnReady;

    private void Start()
    {
        AllowInput = false;
        OnBallLost += SetGameplayLoop;

        Routine.Start(LateStart());
    }
    
    IEnumerator LateStart()
    {
        yield return new WaitForEndOfFrame();

        OnGameStart?.Invoke();
        SetGameplayLoop();
    }

    void SetGameplayLoop()
    {
        AllowInput = false;
        OnReset?.Invoke();
        OnReady?.Invoke();
        AllowInput = true;
    }
}
