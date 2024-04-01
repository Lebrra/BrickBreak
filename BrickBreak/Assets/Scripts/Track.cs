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
        if (activeBricks.Count > 0)
        {
            Debug.LogError("Bricks found in active bricks! Removing...");
            foreach (var brick in activeBricks)
            {
                if (!disabledBricks.Contains(brick))
                    disabledBricks.Add(brick);
            }
            activeBricks = new List<Brick>();
        }

        for (int i = 0; i < 5; i++)
        {
            var depth = GameProperties.TrackSize * 2F - 2F * (i + 1);
            var distribution = GameProperties.GetBrickDistribution;

            for (float j = 0; j < Math.PI * 2F; j += (MathF.PI / distribution))
            {
                if (UnityEngine.Random.Range(0, 10) > 3)
                {
                    var brick = GetBrick();
                    var health = Mathf.Clamp(UnityEngine.Random.Range(1, 10), 1, GameProperties.MaxBrickHealth);
                    activeBricks.Add(brick);
                    brick.InitializeBrick(GetBrickProp.Invoke(health), health, new Vector2(j, depth));
                    brick.gameObject.SetActive(true);
                }
                // else no brick here
            }
        }
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
        // todo: get rid of all active bricks
    }

    void RemoveBrick(Brick brick)
    {
        // todo: make this look cooler
        brick.gameObject.SetActive(false);

        activeBricks.Remove(brick);
        disabledBricks.Add(brick);

        if (activeBricks.Count == 0)
        {
            Debug.LogWarning("YOU WIN WOAH");
        }
    }
}
