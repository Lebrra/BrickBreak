using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class GameProperties
{
    public static float TrackSize => 10F;
    public static int MaxBrickHealth => 5;

    static int[] BrickDistributions => new int[3] { 6, 7, 8 };
    public static int GetBrickDistribution => BrickDistributions[Random.Range(0, BrickDistributions.Length)];

    public static int StartingBricksAmount = 50;
    public static int StartingHealthAmount = 3;

    // I'll allow 0.01-0.07
    public static float BallSpeed = 0.04F;
}
