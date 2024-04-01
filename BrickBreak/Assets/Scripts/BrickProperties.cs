using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Brick Properties")]
public class BrickProperties : ScriptableObject
{
    [SerializeField]
    BrickProperty[] bricks;
    [SerializeField]
    Brick brickPref;

    public Brick GetBrick => brickPref;

    public BrickProperty GetBrickProps(int level)
    {
        if (level > 0 && level <= bricks.Length)
            return bricks[level - 1];
        else return new BrickProperty();
    }
}

[System.Serializable]
public struct BrickProperty
{
    public Material color;
    public float scale;
    // todo: add effects if time
}