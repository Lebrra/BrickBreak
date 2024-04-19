using BeauRoutine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Brick : MonoBehaviour
{
    [SerializeField]
    GameObject brick;
    [SerializeField]
    ParticleSystem[] particles;

    int health = 0;
    Routine brickDestroying;
    Collider brickCollider;

    public void InitializeBrick(BrickProperty startingProp, int startingHealth, Vector2 startingPos)
    {
        transform.localScale = new Vector3(startingProp.scale, startingProp.scale, 1F);
        brick.GetComponent<MeshRenderer>().material = startingProp.color;
        transform.position = Utility.GetWorldPosition(startingPos);
        transform.eulerAngles = Utility.GetWorldRotation(startingPos.x);
        health = startingHealth;

        brickCollider = brick.GetComponent<Collider>() ?? null;
        if (brickCollider) brickCollider.enabled = true;
    }

    public void HitBrick()
    {
        // don't do anything if the game ended
        if (!GameManager.InGame) return;

        health--;

        if (health <= 0)
        {
            // KILL IT
            if (!brickDestroying.Exists())
                brickDestroying.Replace(DelayDestroyBrick());
        }
        else
        {
            particles[health].gameObject.SetActive(true);
            var newProps = Track.GetBrickProp(health);
            transform.localScale = new Vector3(newProps.scale, newProps.scale, 1F);
            brick.GetComponent<MeshRenderer>().material = newProps.color;
        }
    }

    IEnumerator DelayDestroyBrick()
    {
        if (brickCollider) brickCollider.enabled = false;

        particles[0].gameObject.SetActive(true);
        yield return transform.ScaleTo(0F, particles[0].main.duration + 0.1F);

        DisableAllParticles();
        Track.OnBrickDestroyed?.Invoke(this);
    }

    public void DisableAllParticles()
    {
        foreach (var particle in particles)
            particle.gameObject.SetActive(false);
    }
}
