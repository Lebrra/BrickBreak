using BeauRoutine;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Track : MonoBehaviour
{
    public static Func<int, BrickProperty> GetBrickProp;
    public static Action<Brick> OnBrickDestroyed;

    [SerializeField]
    Transform backWall;

    [SerializeField]
    BrickProperties brickProps;

    List<Brick> activeBricks = new List<Brick>();
    List<Brick> disabledBricks = new List<Brick>(); // object pooling!

    private void Start()
    {
        GenerateShape();

        GameManager.OnGameStart += GenerateMap;
        GameManager.OnGameReset += ResetMap;
        GetBrickProp += brickProps.GetBrickProps;
        OnBrickDestroyed += RemoveBrick;
    }

    void GenerateShape()
    {
        backWall.position = Vector3.forward * GameProperties.TrackSize * 2F;
        backWall.localScale = new Vector3(GameProperties.TrackSize * 1.1F, GameProperties.TrackSize * 1.1F, 0.01F);
    }

    void GenerateMap()
    {
        Routine.Start(GenerateBricks());
    }

    IEnumerator GenerateBricks()
    {
        var brickCount = 0;
        var brickDistributions = new List<int>();
        brickDistributions.Add(GameProperties.GetBrickDistribution);
        int disk = 0;
        float point = 0F;

        while (brickCount < GameProperties.StartingBricksAmount && disk < 8)
        {
            if (UnityEngine.Random.Range(0, 10) > 2)
            {
                var depth = GameProperties.TrackSize * 2F - 2F * (disk + 1);

                var brick = GetBrick();
                var health = Mathf.Clamp(UnityEngine.Random.Range(1, 10), 1, GameProperties.MaxBrickHealth);
                activeBricks.Add(brick);
                brick.InitializeBrick(GetBrickProp.Invoke(health), health, new Vector2(point, depth));
                brick.gameObject.SetActive(true);
                brickCount++;

                yield return 0.01F;
            }

            // update values:
            point += (Mathf.PI / brickDistributions[disk]);
            if (point > (Mathf.PI * 1.99F))
            {
                point = 0F;
                disk = (disk + 1);
                brickDistributions.Add(GameProperties.GetBrickDistribution);
            }
        }

        if (brickCount < GameProperties.StartingBricksAmount)
            Debug.LogWarning("Invalid brick count!");

        GameManager.ReadyGameplayLoop?.Invoke();
    }

    Brick GetBrick()
    {
        Brick brick = null;
        if (disabledBricks.Count > 0)
        {
            brick = disabledBricks[0];
            disabledBricks.RemoveAt(0);
        }
        else
        {
            brick = Instantiate(brickProps.GetBrick);
        }

        return brick;
    }

    void ResetMap()
    {
        while (activeBricks.Count > 0)
        {
            var brick = activeBricks[0];
            brick.DisableAllParticles();
            brick.gameObject.SetActive(false);

            activeBricks.Remove(brick);
            disabledBricks.Add(brick);
        }
    }

    void RemoveBrick(Brick brick)
    {
        brick.gameObject.SetActive(false);

        activeBricks.Remove(brick);
        disabledBricks.Add(brick);

        if (activeBricks.Count == 0)
        {
            Debug.LogWarning("YOU WIN WOAH");
            GameManager.AllowInput = false;
            GameManager.OnGameWin?.Invoke();
        }
    }
}
