using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Track : MonoBehaviour
{
    public static float TrackSize => 10F;

    [SerializeField]
    Transform backWall;

    private void Start()
    {
        GenerateShape();

        GameManager.OnGameStart += GenerateMap;
        GameManager.OnGameReset += ResetMap;
    }

    void GenerateShape()
    {
        backWall.position = Vector3.forward * TrackSize * 2F;
        backWall.localScale = new Vector3(TrackSize * 1.1F, TrackSize * 1.1F, 0.01F);
    }

    void GenerateMap()
    {
        // todo: generate bricks whenever this is called
    }

    void ResetMap()
    {
        // todo: get rid of all active bricks
    }
}
