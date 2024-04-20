using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StatsManager : MonoBehaviour
{
    public static Action AddBounce;
    public static Action CalculateTime;
    public static Func<Stats> GetStats;

    Stats stats;
    float startTime;
    // stats are locked when time is calculated. reset on ResetStats()
    bool statsLocked = false;

    private void Awake()
    {
        Track.OnBrickDestroyed += (_) => IncrementBricks();
        AddBounce += IncrementBounce;
        GameManager.OnGameReset += ResetStats;
        GameManager.OnGameStart += SetTime;

        GameManager.OnGameOver += StopTime;
        GameManager.OnGameWin += StopTime;
        GetStats += () => stats;
    }

    void ResetStats()
    {
        stats.bricksBroken = 0;
        stats.bounces = 0;
        // time set on play
        statsLocked = false;
    }

    void IncrementBricks()
    {
        if (!statsLocked)
            stats.bricksBroken++;
    }

    void IncrementBounce()
    {
        if (!statsLocked)
            stats.bounces++;
    }

    void SetTime()
    {
        // in seconds
        startTime = Time.time;
    }

    void StopTime()
    {
        if (!statsLocked)
        {
            statsLocked = true;
            var endTime = Time.time;
            stats.timePlayed = endTime - startTime;
        }
    }
}


public struct Stats
{
    public int bricksBroken;
    public float timePlayed;
    public int bounces;
}
