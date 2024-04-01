using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Brick : MonoBehaviour
{
    [SerializeField]
    GameObject brick;

    int health = 0;


    public void InitializeBrick(BrickProperty startingProp, int startingHealth, Vector2 startingPos)
    {
        // todo: on brick creation set properties here
        transform.localScale = new Vector3(startingProp.scale, startingProp.scale, 1F);
        brick.GetComponent<MeshRenderer>().material = startingProp.color;
        // startingPos = (angle, z)
        transform.position = Utility.GetWorldPosition(startingPos);
        transform.eulerAngles = Utility.GetWorldRotation(startingPos.x);
        health = startingHealth;
    }

    public void HitBrick()
    {
        health--;

        if (health <= 0)
        {
            // KILL IT
            Debug.Log("BRICK DEAD: " + gameObject.name);
            Track.OnBrickDestroyed?.Invoke(this);
        }
        else
        {
            var newProps = Track.GetBrickProp(health);
            transform.localScale = new Vector3(newProps.scale, newProps.scale, 1F);
            brick.GetComponent<MeshRenderer>().material = newProps.color;
        }
    }
}
